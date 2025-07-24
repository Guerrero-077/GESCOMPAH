using MediatR;
using Entity.DTOs.Implements.Utilities;
using Business.Interfaces.Implements.Utilities;

namespace Business.CQRS.Utilities.Image.Select
{
    public class GetAllImageQueryHandler : IRequestHandler<GetAllImageQuery, IEnumerable<ImageSelectDto>>
    {
        private readonly IImagesService _imageService;

        public GetAllImageQueryHandler(IImagesService imageService)
        {
            _imageService = imageService;
        }

        public async Task<IEnumerable<ImageSelectDto>> Handle(GetAllImageQuery request, CancellationToken cancellationToken)
        {
            return await _imageService.GetAllAsync();
        }
    }
}
