using Business.Interfaces.Implements.Utilities;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities;
using MapsterMapper;
using Utilities.Helpers.Business;

namespace Business.Services.Utilities
{
    public class ImageService : BusinessGeneric<ImageSelectDto, ImageDto, ImageDto, Images>, IImagesService
    {
        private readonly IDataGeneric<Images> _dataRepository;
        public ImageService(IDataGeneric<Images> data, IMapper mapper) : base(data, mapper)
        {
            _dataRepository = data;
        }

        public override async Task<IEnumerable<ImageSelectDto>> GetAllAsync()
        {
            try
            {
                var entities = await _dataRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ImageSelectDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las imágenes", ex);
            }
        }
        public override async Task<ImageSelectDto> CreateAsync(ImageDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), "El DTO de imagen no puede ser nulo.");
            var entity = _mapper.Map<Images>(dto);
            entity.InitializeLogicalState(); // Inicializa estado lógico (is_deleted = false)
            var created = await _dataRepository.AddAsync(entity);
            return _mapper.Map<ImageSelectDto>(created);
        }
    }
}
