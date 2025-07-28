using Business.Interfaces.Implements.Utilities;
using Business.Repository;
using Data.Interfaz.IDataImplemenent;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Utilities.Helpers.CloudinaryHelper;

namespace Business.Services.Utilities;

public class ImageService : BusinessGeneric<ImageSelectDto, ImageCreateDto, ImageUpdateDto, Images>, IImagesService
{
    private readonly IImagesRepository _imagesRepository;
    private readonly CloudinaryUtility _cloudinaryHelper;

    public ImageService(
        IImagesRepository imagesRepository,
        IMapper mapper,
        CloudinaryUtility cloudinaryHelper
    ) : base(imagesRepository, mapper)
    {
        _imagesRepository = imagesRepository;
        _cloudinaryHelper = cloudinaryHelper;
    }

    /// <summary>
    /// Subir nuevas imágenes a un establecimiento (máx. 5)
    /// </summary>
    public async Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, IFormFileCollection files)
    {
        var existing = await _imagesRepository.GetByEstablishmentIdAsync(establishmentId);
        var remaining = 5 - existing.Count;

        if (remaining <= 0)
            throw new InvalidOperationException("Máximo de 5 imágenes por establecimiento.");

        var imagesToAdd = new List<Images>();

        foreach (var file in files.Take(remaining))
        {
            var uploadResult = await _cloudinaryHelper.UploadImageAsync(file, establishmentId);

            var image = new Images
            {
                FileName = file.FileName,
                FilePath = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                EstablishmentId = establishmentId
            };

            imagesToAdd.Add(image);
        }

        await _imagesRepository.AddAsync(imagesToAdd);
        return _mapper.Map<List<ImageSelectDto>>(imagesToAdd);
    }

    /// <summary>
    /// Reemplaza una imagen existente: sube nueva, elimina anterior y actualiza la BD
    /// </summary>
    public async Task<ImageSelectDto> ReplaceImageAsync(int imageId, IFormFile newFile)
    {
        var image = await _imagesRepository.GetByIdAsync(imageId)
            ?? throw new KeyNotFoundException("Imagen no encontrada");

        var uploadResult = await _cloudinaryHelper.UploadImageAsync(newFile, image.EstablishmentId);

        await _cloudinaryHelper.DeleteAsync(image.PublicId);

        image.PublicId = uploadResult.PublicId;
        image.FilePath = uploadResult.SecureUrl.AbsoluteUri;
        image.FileName = newFile.FileName;

        await _imagesRepository.UpdateAsync(image);

        return _mapper.Map<ImageSelectDto>(image);
    }

    /// <summary>
    /// Eliminar una imagen por ID
    /// </summary>
    public async Task DeleteImageByIdAsync(int imageId)
    {
        var image = await _imagesRepository.GetByIdAsync(imageId)
            ?? throw new KeyNotFoundException("Imagen no encontrada");

        await _cloudinaryHelper.DeleteAsync(image.PublicId);
        await _imagesRepository.DeleteAsync(image.Id);
    }

    /// <summary>
    /// Eliminar múltiples imágenes por PublicId
    /// </summary>
    public async Task DeleteImagesByPublicIdsAsync(List<string> publicIds)
    {
        if (publicIds == null || publicIds.Count == 0)
            return;

        foreach (var publicId in publicIds)
        {
            await _cloudinaryHelper.DeleteAsync(publicId);
            await _imagesRepository.DeleteByPublicIdAsync(publicId);
        }
    }

    /// <summary>
    /// Obtener todas las imágenes asociadas a un establecimiento
    /// </summary>
    public async Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId)
    {
        var images = await _imagesRepository.GetByEstablishmentIdAsync(establishmentId);
        return _mapper.Map<List<ImageSelectDto>>(images);
    }
}
