using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;
using System.Linq.Expressions;
using Utilities.Exceptions;

namespace Business.Services.Business
{
    public class PlazasService : BusinessGeneric<PlazaSelectDto, PlazaCreateDto, PlazaUpdateDto, Plaza>, IPlazaService
    {
        private readonly IDataGeneric<Plaza> _data;
        private readonly IEstablishmentsRepository _establishmentsRepository;
        private readonly IContractRepository _contractRepository;

        public PlazasService(
            IDataGeneric<Plaza> data,
            IMapper mapper,
            IEstablishmentsRepository establishmentsRepository,
            IContractRepository contractRepository
        ) : base(data, mapper)
        {
            _data = data;
            _establishmentsRepository = establishmentsRepository;
            _contractRepository = contractRepository;
        }

        public override async Task UpdateActiveStatusAsync(int id, bool active)
        {
            // Cargar entidad y validar estado actual
            var entity = await _data.GetByIdAsync(id) ?? throw new KeyNotFoundException($"No se encontró el registro con ID {id}.");
            if (entity.Active == active) return;

            // Si vamos a desactivar, verificar contratos activos asociados a la plaza
            if (!active)
            {
                var hasActiveContracts = await _contractRepository.AnyActiveByPlazaAsync(id);
                if (hasActiveContracts)
                {
                    throw new BusinessException("No se puede desactivar la plaza porque tiene establecimientos con contratos activos.");
                }
            }

            // Actualiza el estado de la plaza
            entity.Active = active;
            await _data.UpdateAsync(entity);

            // Cascada: activar/desactivar establecimientos de la plaza
            await _establishmentsRepository.SetActiveByPlazaIdAsync(id, active);
        }


        protected override Expression<Func<Plaza, string>>[] SearchableFields() =>
        [
            p => p.Name,
            p => p.Description,
            p => p.Location
        ];

        protected override string[] SortableFields() =>
        [
            nameof(Plaza.Name),
            nameof(Plaza.Description),
            nameof(Plaza.Location),
            nameof(Plaza.Active),
            nameof(Plaza.CreatedAt),
            nameof(Plaza.Id)
        ];

        protected override IDictionary<string, Func<string, Expression<Func<Plaza, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<Plaza, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Plaza.Name)] = value => p => p.Name == value,
                [nameof(Plaza.Active)] = value => p => p.Active == bool.Parse(value)
            };

    }
}

