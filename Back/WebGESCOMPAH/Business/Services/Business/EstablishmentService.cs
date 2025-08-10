using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Business;
using Data.Interfaz.IDataImplemenent.Utilities;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.DTOs.Implements.Utilities.Images;
using Entity.Infrastructure.Context;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers.CloudinaryHelper;

public class EstablishmentService : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>, IEstablishmentService
{
    private readonly IEstablishmentsRepository _repo;
    private readonly IImagesRepository _imagesRepo;
    private readonly CloudinaryUtility _cloudinary;
    private readonly IMapper _mapper;
    private readonly ILogger<EstablishmentService> _logger;
    private readonly ApplicationDbContext _context;

    public EstablishmentService(
        IEstablishmentsRepository repo,
        IImagesRepository imagesRepo,
        IMapper mapper,
        CloudinaryUtility cloudinary,
        ILogger<EstablishmentService> logger,
        ApplicationDbContext context) : base(repo, mapper)
    {
        _repo = repo;
        _imagesRepo = imagesRepo;
        _mapper = mapper;
        _cloudinary = cloudinary;
        _logger = logger;
        _context = context;
    }

    public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
    {
        if (dto.Files?.Count > 5)
        {
            _logger.LogWarning("Intento de crear establecimiento con más de 5 imágenes ({Count})", dto.Files.Count);
            throw new BusinessException("Solo se permiten hasta 5 imágenes por establecimiento");
        }

        var entity = _mapper.Map<Establishment>(dto);

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _repo.AddAsync(entity);
            await _context.SaveChangesAsync(); // Guarda para obtener Id

            var images = await UploadAndMapImagesAsync(dto.Files, entity.Id);

            if (images.Any())
            {
                await _imagesRepo.AddAsync(images);
                await _context.SaveChangesAsync(); // Guarda imágenes
            }

            await transaction.CommitAsync();

            var result = _mapper.Map<EstablishmentSelectDto>(entity);
            result.Images = images.Adapt<List<ImageSelectDto>>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando establecimiento");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(dto.Id);
        if (entity == null)
            throw new NotFoundException("Establishment", $"No se encontró el establecimiento {dto.Id}");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Actualizar campos escalares
            entity.Name = dto.Name ?? entity.Name;
            entity.Description = dto.Description ?? entity.Description;
            entity.AreaM2 = dto.AreaM2 != default ? dto.AreaM2 : entity.AreaM2;
            entity.Address = dto.Address ?? entity.Address;
            entity.RentValueBase = dto.RentValueBase != default ? dto.RentValueBase : entity.RentValueBase;
            entity.PlazaId = dto.PlazaId != default ? dto.PlazaId : entity.PlazaId;

            await _repo.UpdateAsync(entity);
            await _context.SaveChangesAsync();

            if (dto.ImagesToDelete != null)
            {
                foreach (var publicId in dto.ImagesToDelete.Where(id => !string.IsNullOrWhiteSpace(id)))
                {
                    await _cloudinary.DeleteAsync(publicId);
                    await _imagesRepo.DeleteLogicalByPublicIdAsync(publicId);
                }
                await _context.SaveChangesAsync();
            }

            if (dto.Images?.Any() == true)
            {
                var validFiles = dto.Images.Where(f => f?.Length > 0).ToList();

                var currentCount = await _imagesRepo.GetByEstablishmentIdAsync(dto.Id);
                if (validFiles.Count + currentCount.Count > 5)
                    throw new BusinessException($"Solo puede subir {5 - currentCount.Count} imágenes adicionales. Máximo 5 por establecimiento.");

                var newImages = await UploadAndMapImagesAsync(validFiles, entity.Id);
                await _imagesRepo.AddAsync(newImages);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            var updated = await _repo.GetByIdAsync(dto.Id);
            var resultDto = _mapper.Map<EstablishmentSelectDto>(updated!);
            resultDto.Images = updated!.Images
                .Select(i => new ImageSelectDto(i.Id, i.FileName, i.FilePath, i.PublicId, i.EstablishmentId))
                .ToList();

            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando establecimiento ID {Id}", dto.Id);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Intento de eliminar establecimiento inexistente con ID {Id}", id);
            return false;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var images = await _imagesRepo.GetByEstablishmentIdAsync(id);

            foreach (var image in images)
            {
                await _cloudinary.DeleteAsync(image.PublicId);
                await _imagesRepo.DeleteLogicalByPublicIdAsync(image.PublicId);
            }
            await _context.SaveChangesAsync();

            var deleted = await _repo.DeleteLogicAsync(id);
            await _context.SaveChangesAsync();

            if (deleted)
                _logger.LogInformation("Establecimiento eliminado con ID {Id}", id);
            else
                _logger.LogError("Error al eliminar establecimiento con ID {Id}", id);

            await transaction.CommitAsync();

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando establecimiento ID {Id}", id);
            await transaction.RollbackAsync();
            throw;
        }
    }

    #region Helpers

    private async Task<List<Images>> UploadAndMapImagesAsync(IEnumerable<IFormFile>? files, int establishmentId)
    {
        if (files == null || !files.Any())
            return new List<Images>();

        var fileList = files.ToList();

        // Limitar concurrencia para no saturar Cloudinary
        var semaphore = new SemaphoreSlim(3);

        var uploadTasks = fileList.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await _cloudinary.UploadImageAsync(file, establishmentId);
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

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
