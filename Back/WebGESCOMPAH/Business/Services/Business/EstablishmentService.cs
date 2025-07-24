using Business.Interfaces.Implements;
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
            establishment.Images = await UploadImagesAsync(dto.Files);

            await _repoEstablishment.AddAsync(establishment);
            return _mapper.Map<EstablishmentSelectDto>(establishment);
        }
        catch (BusinessException) { throw; }
        catch (Exception ex)
        {
            throw new BusinessException("No se pudo crear el establecimiento.", ex);
        }
    }

    public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
    {
        try
        {
            var establishment = await _repoEstablishment
                .GetAllQueryable()
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == dto.Id && !e.IsDeleted);

            if (establishment == null)
                throw new BusinessException("Establecimiento no encontrado.");

            _mapper.Map(dto, establishment); // Asegúrate de configurar bien el mapeo

            if (dto.Files?.Count > 0)
            {
                var newImages = await UploadImagesAsync(dto.Files);

                foreach (var image in establishment.Images)
                    await _cloudinary.DestroyAsync(new DeletionParams(image.FileName));

                establishment.Images.Clear();
                establishment.Images.AddRange(newImages);
            }

            await _repoEstablishment.UpdateAsync(establishment);
            return _mapper.Map<EstablishmentSelectDto>(establishment);
        }
        catch (BusinessException) { throw; }
        catch (Exception ex)
        {
            throw new BusinessException("No se pudo actualizar el establecimiento.", ex);
        }
    }


    public async Task DeleteAsync(int id, bool forceDelete)
    {
        try
        {
            var establishment = await _repoEstablishment
                .GetAllQueryable()
                .Include(e => e.Images)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (establishment == null || (!forceDelete && establishment.IsDeleted))
                throw new BusinessException("Establecimiento no encontrado.");

            foreach (var image in establishment.Images)
                await _cloudinary.DestroyAsync(new DeletionParams(image.FileName));

            var result = forceDelete
                ? await _repoEstablishment.DeleteAsync(id)
                : await _repoEstablishment.DeleteLogicAsync(id);

            if (!result)
                throw new BusinessException("No se pudo eliminar el establecimiento.");
        }
        catch (BusinessException) { throw; }
        catch (Exception ex)
        {
            throw new BusinessException("Error al eliminar el establecimiento.", ex);
        }
    }

    private async Task<List<Images>> UploadImagesAsync(ICollection<IFormFile>? files)
    {
        if (files == null || files.Count == 0)
            return new List<Images>();

        if (files.Count > MaxImageCount)
            throw new BusinessException($"Solo se permiten hasta {MaxImageCount} imágenes por establecimiento.");

        var uploadedImages = new List<(Images image, string publicId)>();

        try
        {
            foreach (var file in files)
            {
                if (file.Length == 0 || !AllowedContentTypes.Contains(file.ContentType))
                    throw new BusinessException("Archivo inválido. Solo se permiten imágenes JPEG o PNG.");

                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "establishments",
                    PublicId = $"establishment_{Guid.NewGuid()}"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                    throw new BusinessException($"Cloudinary error: {result.Error.Message}");

                uploadedImages.Add((
                    new Images
                    {
                        FileName = result.PublicId,
                        FilePath = result.SecureUrl.ToString()
                    },
                    result.PublicId
                ));
            }

            return uploadedImages.Select(x => x.image).ToList();
        }
        catch
        {
            foreach (var (_, publicId) in uploadedImages)
                await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            throw;
        }
    }
}
