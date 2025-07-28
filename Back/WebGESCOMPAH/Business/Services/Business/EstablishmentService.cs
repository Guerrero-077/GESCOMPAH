using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplemenent;
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

public class EstablishmentService : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>,
    IEstablishmentService
{
    private readonly IEstablishments _repository;
    private readonly CloudinaryUtility _cloudinaryHelper;
    private readonly IImagesRepository _imagesRepository;
    private readonly ILogger<EstablishmentService> _logger;

    public EstablishmentService(IEstablishments data, IImagesRepository imagesRepository, IMapper mapper, CloudinaryUtility cloudinaryHelper, ILogger<EstablishmentService> logger) : base(data, mapper)
    {
        _repository = data;
        _imagesRepository = imagesRepository;
        _cloudinaryHelper = cloudinaryHelper;
        _logger = logger;
    }


    public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
    {
        if (dto.Files?.Count > 5)
        {
            _logger.LogWarning("Intento de crear establecimiento con más de 5 imágenes ({Count})", dto.Files.Count);
            throw new BusinessException("Solo se permiten hasta 5 imágenes por establecimiento");
        }

        var entity = _mapper.Map<Establishment>(dto);
        await _repository.AddAsync(entity);
        _logger.LogInformation("Establecimiento creado con ID {Id}", entity.Id);

        var images = await UploadAndMapImagesAsync(dto.Files, entity.Id);
        if (images.Any())
        {
            await _imagesRepository.AddAsync(images);
            _logger.LogInformation("{Count} imágenes asociadas al establecimiento ID {Id}", images.Count, entity.Id);
        }

        var dtoResult = _mapper.Map<EstablishmentSelectDto>(entity);
        dtoResult.Images = images.Adapt<List<ImageSelectDto>>();
        return dtoResult;
    }

    public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null)
        {
            _logger.LogWarning("Intento de actualizar establecimiento inexistente con ID {Id}", dto.Id);
            throw new NotFoundException("Establishment", "Establecimiento no encontrado");
        }

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Establecimiento actualizado con ID {Id}", entity.Id);

        var existingImages = await _imagesRepository.GetByEstablishmentIdAsync(dto.Id);
        var totalImages = existingImages.Count;

        var newImages = new List<Images>();
        if (dto.Files?.Any() == true)
        {
            var availableSlots = 5 - totalImages;
            if (dto.Files.Count > availableSlots)
            {
                _logger.LogWarning("Actualización excede el límite de imágenes: {Count} nuevas, {Available} permitidas",
                    dto.Files.Count, availableSlots);
                throw new BusinessException($"Solo puede subir {availableSlots} imágenes adicionales (máximo 5 por establecimiento)");
            }

            var filesToUpload = dto.Files.Take(availableSlots).ToList();
            newImages = await UploadAndMapImagesAsync(filesToUpload, entity.Id);

            if (newImages.Any())
            {
                await _imagesRepository.AddAsync(newImages);
                _logger.LogInformation("{Count} imágenes nuevas agregadas al establecimiento ID {Id}", newImages.Count, entity.Id);
            }
        }

        var resultDto = _mapper.Map<EstablishmentSelectDto>(entity);
        resultDto.Images = existingImages
            .Concat(newImages)
            .Adapt<List<ImageSelectDto>>();

        return resultDto;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Intento de eliminar establecimiento inexistente con ID {Id}", id);
            return false;
        }

        var images = await _imagesRepository.GetByEstablishmentIdAsync(id);
        foreach (var image in images)
        {
            await _cloudinaryHelper.DeleteAsync(image.PublicId);
            await _imagesRepository.DeleteByPublicIdAsync(image.PublicId);
            _logger.LogInformation("Imagen eliminada de Cloudinary y DB: PublicId {PublicId}", image.PublicId);
        }

        var deleted = await _repository.DeleteAsync(id);
        if (deleted)
            _logger.LogInformation("Establecimiento eliminado con ID {Id}", id);
        else
            _logger.LogError("Error al eliminar establecimiento con ID {Id}", id);

        return deleted;
    }

    private async Task<List<Images>> UploadAndMapImagesAsync(IEnumerable<IFormFile>? files, int establishmentId)
    {
        if (files == null || !files.Any())
        {
            _logger.LogInformation("No se encontraron archivos para subir para el establecimiento ID {Id}", establishmentId);
            return new List<Images>();
        }

        var fileList = files.ToList();
        var uploadTasks = fileList
            .Select(file => _cloudinaryHelper.UploadImageAsync(file, establishmentId))
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

        _logger.LogInformation("{Count} imágenes subidas correctamente para establecimiento ID {Id}", images.Count, establishmentId);
        return images;
    }
}
