// ───────────────────────────────────────────────────────────────────────
// 1.  Servicio completo – operaciones CRUD sin UoW / transacción
// ───────────────────────────────────────────────────────────────────────
using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Business;
using Data.Interfaz.IDataImplemenent.Utilities;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.DTOs.Implements.Utilities.Images;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers.CloudinaryHelper;

public class EstablishmentService : BusinessGeneric<EstablishmentSelectDto,
                                                    EstablishmentCreateDto,
                                                    EstablishmentUpdateDto,
                                                    Establishment>,
                                    IEstablishmentService
{
    private readonly IEstablishmentsRepository _repo;
    private readonly IImagesRepository _imagesRepo;
    private readonly CloudinaryUtility _cloudinary;
    private readonly IMapper _mapper;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(IEstablishmentsRepository repo,
                                IImagesRepository imagesRepo,
                                IMapper mapper,
                                CloudinaryUtility cloudinary,
                                ILogger<EstablishmentService> logger)
        : base(repo, mapper)
    {
        _repo = repo;
        _imagesRepo = imagesRepo;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    // ───────────────────────────────────────
    public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
    {
        if (dto.Files?.Count > 5)
        {
            _logger.LogWarning("Intento de crear establecimiento con más de 5 imágenes ({Count})",
                               dto.Files.Count);
            throw new BusinessException("Solo se permiten hasta 5 imágenes por establecimiento");
        }

        var entity = _mapper.Map<Establishment>(dto);
        await _repo.AddAsync(entity);

        var images = await UploadAndMapImagesAsync(dto.Files, entity.Id);
        if (images.Any())
        {
            await _imagesRepo.AddAsync(images);
        }

        var result = _mapper.Map<EstablishmentSelectDto>(entity);
        result.Images = images.Adapt<List<ImageSelectDto>>();
        return result;
    }

    // ───────────────────────────────────────
    public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
    {
        // 1️⃣  Cargar la entidad con sus imágenes (para validaciones)
        var entity = await _repo.GetByIdAsync(dto.Id);      // incluye .Images
        if (entity == null)
            throw new NotFoundException("Establishment", $"No se encontró el establecimiento {dto.Id}");

        // 2️⃣  Actualizar los campos escalares
        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.AreaM2 = dto.AreaM2 != default ? dto.AreaM2 : entity.AreaM2;
        entity.RentValueBase = dto.RentValueBase != default ? dto.RentValueBase : entity.RentValueBase;
        entity.PlazaId = dto.PlazaId != default ? dto.PlazaId : entity.PlazaId;

        // 3️⃣  Persistir los scalars
        await _repo.UpdateAsync(entity);      // repo ya hizo SaveChanges

        // 4️⃣  Borrar imágenes marcadas
        if (dto.ImagesToDelete != null)
        {
            foreach (var publicId in dto.ImagesToDelete.Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                await _cloudinary.DeleteAsync(publicId);
                await _imagesRepo.DeleteByPublicIdAsync(publicId);
            }
        }

        // 5️⃣  Añadir nuevas imágenes (máx 5 en total)
        if (dto.Images?.Any() == true)
        {
            var validFiles = dto.Images.Where(f => f?.Length > 0).ToList();

            // Validar que no superemos el límite (5 imágenes)
            var currentCount = await _imagesRepo.GetByEstablishmentIdAsync(dto.Id);
            if (validFiles.Count + currentCount.Count > 5)
                throw new BusinessException(
                    $"Solo puede subir {5 - currentCount.Count} imágenes adicionales. Máximo 5 por establecimiento.");

            var newImages = await UploadAndMapImagesAsync(validFiles, entity.Id);
            await _imagesRepo.AddAsync(newImages);
        }

        // 6️⃣  Devolver DTO con la nueva lista de imágenes
        var updated = await _repo.GetByIdAsync(dto.Id);
        var resultDto = _mapper.Map<EstablishmentSelectDto>(updated!);
        resultDto.Images = updated!.Images
                      .Select(i => new ImageSelectDto(
                          i.Id, i.FileName, i.FilePath, i.PublicId, i.EstablishmentId))
                      .ToList();

        return resultDto;
    }



    // ───────────────────────────────────────
    public override async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Intento de eliminar establecimiento inexistente con ID {Id}", id);
            return false;
        }

        var images = await _imagesRepo.GetByEstablishmentIdAsync(id);
        foreach (var image in images)
        {
            await _cloudinary.DeleteAsync(image.PublicId);
            await _imagesRepo.DeleteByPublicIdAsync(image.PublicId);
        }

        var deleted = await _repo.DeleteAsync(id);
        if (deleted)
            _logger.LogInformation("Establecimiento eliminado con ID {Id}", id);
        else
            _logger.LogError("Error al eliminar establecimiento con ID {Id}", id);

        return deleted;
    }

    // ───────────────────────────────────────
    #region Helpers

    // • Subir las imágenes a Cloudinary y crear la entidad de dominio
    private async Task<List<Images>> UploadAndMapImagesAsync(IEnumerable<IFormFile>? files,
                                                             int establishmentId)
    {
        if (files == null || !files.Any())
            return new List<Images>();

        var fileList = files.ToList();
        var uploadTasks = fileList
            .Select(file => _cloudinary.UploadImageAsync(file, establishmentId))
            .ToList();

        var uploadResults = await Task.WhenAll(uploadTasks);

        var images = new List<Images>();
        for (int i = 0; i < fileList.Count; i++)
        {
            images.Add(new Images
            {
                FileName = fileList[i].FileName,
                FilePath = uploadResults[i].SecureUrl.AbsoluteUri,
                PublicId = uploadResults[i].PublicId,
                EstablishmentId = establishmentId
            });
        }

        _logger.LogInformation("{Count} imágenes subidas para establecimiento ID {Id}",
                               images.Count, establishmentId);
        return images;
    }

    #endregion
}
