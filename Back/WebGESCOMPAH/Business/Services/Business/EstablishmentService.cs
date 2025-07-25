    using Business.Interfaces.Implements.Business;
    using Business.Repository;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Data.Interfaz.DataBasic;
    using Entity.Domain.Models.Implements.Business;
    using Entity.Domain.Models.Implements.Utilities;
    using Entity.DTOs.Implements.Business.Establishment;
    using MapsterMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Utilities.Exceptions;

    public class EstablishmentService : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>, IEstablishmentService
    {
        private readonly IDataGeneric<Establishment> _repoEstablishment;
        private readonly Cloudinary _cloudinary;

        private const int MaxImageCount = 5;
        private static readonly HashSet<string> AllowedContentTypes = new() { "image/jpeg", "image/png" };

        public EstablishmentService(
            IDataGeneric<Establishment> repoEstablishment,
            IMapper mapper,
            Cloudinary cloudinary) : base(repoEstablishment, mapper)
        {
            _repoEstablishment = repoEstablishment;
            _cloudinary = cloudinary;
        }

        public override async Task<IEnumerable<EstablishmentSelectDto>> GetAllAsync()
        {
            var establishments = await _repoEstablishment
                .GetAllQueryable()
                .Include(e => e.Images)
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<EstablishmentSelectDto>>(establishments);
        }

        public override async Task<EstablishmentSelectDto?> GetByIdAsync(int id)
        {
            var est = await _repoEstablishment
                .GetAllQueryable()
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            return est != null ? _mapper.Map<EstablishmentSelectDto>(est) : null;
        }

        public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
        {
            try
            {
                var establishment = _mapper.Map<Establishment>(dto);
                await _repoEstablishment.AddAsync(establishment);

                // Subir imágenes después de tener el ID
                if (dto.Files?.Count > 0)
                {
                    var images = await UploadImagesAsync(dto.Files, establishment.Id);
                    establishment.Images.AddRange(images);
                    await _repoEstablishment.UpdateAsync(establishment);
                }

                return _mapper.Map<EstablishmentSelectDto>(establishment);
            }
            catch (Exception ex)
            {
                throw new BusinessException("No se pudo crear el establecimiento.", ex);
            }
        }

        public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
        {
            var establishment = await _repoEstablishment
                .GetAllQueryable()
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == dto.Id && !e.IsDeleted);

            if (establishment == null)
                throw new BusinessException("Establecimiento no encontrado.");

            _mapper.Map(dto, establishment);

            // Adjunta imágenes nuevas sin eliminar todas las anteriores
            if (dto.Files?.Count > 0)
            {
                if (establishment.Images.Count + dto.Files.Count > MaxImageCount)
                    throw new BusinessException($"Solo se permiten hasta {MaxImageCount} imágenes por establecimiento.");

                var newImages = await UploadImagesAsync(dto.Files, establishment.Id);
                establishment.Images.AddRange(newImages);
            }

            await _repoEstablishment.UpdateAsync(establishment);
            return _mapper.Map<EstablishmentSelectDto>(establishment);
        }

        //public async Task DeleteAsync(int id, bool forceDelete)
        //{
        //    var establishment = await _repoEstablishment
        //        .GetAllQueryable()
        //        .Include(e => e.Images)
        //        .IgnoreQueryFilters()
        //        .FirstOrDefaultAsync(e => e.Id == id);

        //    if (establishment == null || (!forceDelete && establishment.IsDeleted))
        //        throw new BusinessException("Establecimiento no encontrado.");

        //    // Eliminar imágenes de Cloudinary
        //    foreach (var image in establishment.Images)
        //    {
        //        await _cloudinary.DestroyAsync(new DeletionParams(image.FileName));
        //    }

        //    var result = forceDelete
        //        ? await _repoEstablishment.DeleteAsync(id)
        //        : await _repoEstablishment.DeleteLogicAsync(id);

        //    if (!result)
        //        throw new BusinessException("No se pudo eliminar el establecimiento.");
        //}

        //public async Task DeleteAsync(int establishmentId, int imageId)
        //{
        //    var establishment = await _repoEstablishment
        //        .GetAllQueryable()
        //        .Include(e => e.Images)
        //        .FirstOrDefaultAsync(e => e.Id == establishmentId && !e.IsDeleted);

        //    if (establishment == null)
        //        throw new BusinessException("Establecimiento no encontrado.");

        //    var image = establishment.Images.FirstOrDefault(i => i.Id == imageId);
        //    if (image == null)
        //        throw new BusinessException("Imagen no encontrada.");

        //    await _cloudinary.DestroyAsync(new DeletionParams(image.FileName));

        //    establishment.Images.Remove(image);
        //    await _repoEstablishment.UpdateAsync(establishment);
        //}

        private async Task<List<Images>> UploadImagesAsync(ICollection<IFormFile> files, int establishmentId)
        {
            var uploadedImages = new List<Images>();

            foreach (var file in files)
            {
                if (file.Length == 0 || !AllowedContentTypes.Contains(file.ContentType))
                    throw new BusinessException("Archivo inválido. Solo se permiten imágenes JPEG o PNG.");

                using var stream = file.OpenReadStream();

                var publicId = $"img_{Guid.NewGuid()}";
                var folder = $"establishments/{establishmentId}";

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,
                    PublicId = publicId
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                    throw new BusinessException($"Cloudinary error: {result.Error.Message}");

                uploadedImages.Add(new Images
                {
                    FileName = $"{folder}/{publicId}",       // Ruta completa usada para eliminar luego
                    FilePath = result.SecureUrl.ToString()
                });
            }

            return uploadedImages;
        }

        public async Task DeleteImageAsync(int establishmentId, int imageId)
        {
            try
            {
                var establishment = await _repoEstablishment
                    .GetAllQueryable()
                    .Include(e => e.Images)
                    .FirstOrDefaultAsync(e => e.Id == establishmentId && !e.IsDeleted);

                if (establishment == null)
                    throw new BusinessException("Establecimiento no encontrado.");

                var imageToDelete = establishment.Images.FirstOrDefault(img => img.Id == imageId);
                if (imageToDelete == null)
                    throw new BusinessException("Imagen no encontrada en el establecimiento.");

                // Eliminar de Cloudinary
                var deleteResult = await _cloudinary.DestroyAsync(new DeletionParams(imageToDelete.FileName));
                if (deleteResult.Result != "ok")
                    throw new BusinessException($"Error al eliminar la imagen en Cloudinary: {deleteResult.Error?.Message}");

                // Eliminar de la colección
                establishment.Images.Remove(imageToDelete);

                // Persistir cambios
                await _repoEstablishment.UpdateAsync(establishment);
            }
            catch (BusinessException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("No se pudo eliminar la imagen del establecimiento.", ex);
            }
        }

    }
