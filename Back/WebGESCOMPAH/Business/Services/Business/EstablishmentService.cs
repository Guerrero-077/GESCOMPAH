using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Utilities;
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
    private readonly ILogger<EstablishmentService> _logger;
    private readonly ApplicationDbContext _context;

    private const int MaxImages = 5;

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
        _cloudinary = cloudinary;
        _logger = logger;
        _context = context;
    }

    public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
    {
        ValidateMaxImages(dto.Files?.Count ?? 0);

        var entity = dto.Adapt<Establishment>();

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _repo.AddAsync(entity);
            await _context.SaveChangesAsync();

            var images = await UploadAndMapImagesAsync(dto.Files, entity.Id);
            if (images.Any())
            {
                await _imagesRepo.AddAsync(images);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            var result = entity.Adapt<EstablishmentSelectDto>();
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
        var entity = await _repo.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException("Establishment", $"No se encontró el establecimiento {dto.Id}");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            dto.Adapt(entity);

            await _repo.UpdateAsync(entity);

            // Conteo actual antes de modificaciones
            var currentImages = await _imagesRepo.GetByEstablishmentIdAsync(dto.Id);
            var currentCount = currentImages.Count;

            // Calcular eliminadas
            var toDelete = dto.ImagesToDelete?.Count ?? 0;

            // Calcular nuevas válidas
            var validFiles = dto.Images?.Where(f => f?.Length > 0).ToList() ?? new List<IFormFile>();
            var toAdd = validFiles.Count;

            // Conteo final esperado
            var finalCount = currentCount - toDelete + toAdd;

            ValidateMaxImages(finalCount);

            // Procesar eliminaciones
            if (toDelete > 0)
                await DeleteImagesAsync(dto.ImagesToDelete!);

            // Procesar nuevas
            if (toAdd > 0)
            {
                var newImages = await UploadAndMapImagesAsync(validFiles, entity.Id);
                await _imagesRepo.AddAsync(newImages);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (await _repo.GetByIdAsync(dto.Id))!.Adapt<EstablishmentSelectDto>();
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
            await DeleteImagesAsync((await _imagesRepo.GetByEstablishmentIdAsync(id)).Select(i => i.PublicId).ToList());
            var deleted = await _repo.DeleteLogicAsync(id);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(deleted
                ? "Establecimiento eliminado con ID {Id}"
                : "Error al eliminar establecimiento con ID {Id}", id);

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

    private void ValidateMaxImages(int finalCount)
    {
        if (finalCount > MaxImages)
            throw new BusinessException($"Solo se permiten hasta {MaxImages} imágenes por establecimiento. Actualmente: {finalCount}.");
    }


    private async Task DeleteImagesAsync(IEnumerable<string> publicIds)
    {
        foreach (var publicId in publicIds.Where(id => !string.IsNullOrWhiteSpace(id)))
        {
            await _cloudinary.DeleteAsync(publicId);
            await _imagesRepo.DeleteLogicalByPublicIdAsync(publicId);
        }
    }

    private async Task<List<Images>> UploadAndMapImagesAsync(IEnumerable<IFormFile>? files, int establishmentId)
    {
        if (files == null || !files.Any())
            return new List<Images>();

        var semaphore = new SemaphoreSlim(3);
        var uploadTasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                var uploadResult = await _cloudinary.UploadImageAsync(file, establishmentId);
                return new Images
                {
                    FileName = file.FileName,
                    FilePath = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId,
                    EstablishmentId = establishmentId
                };
            }
            finally
            {
                semaphore.Release();
            }
        });

        var images = (await Task.WhenAll(uploadTasks)).ToList();
        _logger.LogInformation("{Count} imágenes subidas para establecimiento ID {Id}", images.Count, establishmentId);
        return images;
    }

    #endregion
}
