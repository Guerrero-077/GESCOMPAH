
using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Business.Establishment;
using Entity.DTOs.Implements.Utilities.Images;
using Mapster;
using MapsterMapper;
using Utilities.Exceptions;
using Utilities.Helpers.CloudinaryHelper;

namespace Business.Services.Business
{
    //    public class EstablishmentService(IEstablishments data, IMapper mapper) : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>(data, mapper), IEstablishmentService
    //    {


    public class EstablishmentService : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>, IEstablishmentService
    {
        private readonly IEstablishments _repository;
        private readonly IImagesRepository _imagesRepository;
        private readonly CloudinaryUtility _cloudinaryHelper;

        public EstablishmentService(IEstablishments data, IMapper mapper, IImagesRepository imagesRepository, CloudinaryUtility cloudinaryUtility) : base(data, mapper)
        {
            _repository = data;
            _imagesRepository = imagesRepository;
            _cloudinaryHelper = cloudinaryUtility;
        }

        public override async Task<EstablishmentSelectDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var dto = _mapper.Map<EstablishmentSelectDto>(entity);
            dto.Images = (await _imagesRepository.GetByEstablishmentIdAsync(id))
                .Adapt<List<ImageSelectDto>>();

            return dto;
        }

        public override async Task<IEnumerable<EstablishmentSelectDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var result = new List<EstablishmentSelectDto>();

            foreach (var entity in entities)
            {
                var dto = _mapper.Map<EstablishmentSelectDto>(entity);
                dto.Images = (await _imagesRepository.GetByEstablishmentIdAsync(entity.Id))
                    .Adapt<List<ImageSelectDto>>();
                result.Add(dto);
            }

            return result;
        }

        public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
        {
            var entity = _mapper.Map<Establishment>(dto);
            await _repository.AddAsync(entity);

            var images = new List<Images>();
            if (dto.Files?.Any() == true)
            {
                foreach (var file in dto.Files.Take(5))
                {
                    var result = await _cloudinaryHelper.UploadImageAsync(file, entity.Id);
                    images.Add(new Images
                    {
                        FileName = file.FileName,
                        FilePath = result.SecureUrl.AbsoluteUri,
                        PublicId = result.PublicId,
                        EstablishmentId = entity.Id
                    });
                }


                await _imagesRepository.AddAsync(images);
            }

            var dtoResult = _mapper.Map<EstablishmentSelectDto>(entity);
            dtoResult.Images = images.Adapt<List<ImageSelectDto>>();

            return dtoResult;
        }


        public override async Task<EstablishmentSelectDto> UpdateAsync(EstablishmentUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new NotFoundException("Establishment", "Establecimiento no encontrado");


            // Mapea los campos básicos del DTO al modelo
            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            // Obtener imágenes existentes
            var existingImages = await _imagesRepository.GetByEstablishmentIdAsync(dto.Id);
            var totalImages = existingImages.Count;

            // Validar si hay nuevas imágenes para subir
            var newImages = new List<Images>();
            if (dto.Files?.Any() == true)
            {
                var availableSlots = 5 - totalImages;
                if (availableSlots <= 0)
                    throw new BusinessException("Ya se alcanzó el máximo de 5 imágenes para este establecimiento");

                foreach (var file in dto.Files.Take(availableSlots))
                {
                    var result = await _cloudinaryHelper.UploadImageAsync(file, entity.Id);
                    newImages.Add(new Images
                    {
                        FileName = file.FileName,
                        FilePath = result.SecureUrl.AbsoluteUri,
                        PublicId = result.PublicId,
                        EstablishmentId = entity.Id
                    });
                }


                await _imagesRepository.AddAsync(newImages);
            }

            // Retornar DTO actualizado
            var resultDto = _mapper.Map<EstablishmentSelectDto>(entity);
            resultDto.Images = (existingImages.Concat(newImages)).Adapt<List<ImageSelectDto>>();

            return resultDto;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            var images = await _imagesRepository.GetByEstablishmentIdAsync(id);
            foreach (var image in images)
            {
                await _cloudinaryHelper.DeleteAsync(image.PublicId);
                await _imagesRepository.DeleteByPublicIdAsync(image.PublicId);
            }

            return await _repository.DeleteAsync(id);
        }
    }


}
