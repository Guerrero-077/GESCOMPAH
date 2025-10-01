using System.Collections.Generic;
using System.Linq;

using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;

using Entity.DTOs.Implements.Business.Clause;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.PremisesLeased;

using Mapster;

namespace Business.Mapping.Registers
{
    public class BusinessContractMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // ContractCreateDto -> Person
            TypeAdapterConfig<ContractCreateDto, Person>.NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.City)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Document, src => src.Document)
                .Map(dest => dest.Phone, src => src.Phone)
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.CityId, src => src.CityId);

            // ContractCreateDto -> Contract
            TypeAdapterConfig<ContractCreateDto, Contract>.NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Person)
                .Ignore(dest => dest.PersonId)
                .Ignore(dest => dest.PremisesLeased)
                .Ignore(dest => dest.ContractClauses)
                .Ignore(dest => dest.TotalBaseRentAgreed)
                .Ignore(dest => dest.TotalUvtQtyAgreed)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.CreatedAt)
                .Map(dest => dest.StartDate, src => src.StartDate)
                .Map(dest => dest.EndDate, src => src.EndDate);

            // Contract -> ContractSelectDto
            TypeAdapterConfig<Contract, ContractSelectDto>.NewConfig()
                .Map(dest => dest.FullName, src => src.Person.FirstName + " " + src.Person.LastName)
                .Map(dest => dest.Document, src => src.Person.Document)
                .Map(dest => dest.Phone, src => src.Person.Phone)
                .Map(dest => dest.Email, src => src.Person.User != null ? src.Person.User.Email : string.Empty)
                .Map(dest => dest.PremisesLeased, src => src.PremisesLeased.Adapt<List<PremisesLeasedSelectDto>>())
                .Map(dest => dest.TotalBaseRentAgreed, src => src.TotalBaseRentAgreed)
                .Map(dest => dest.TotalUvtQtyAgreed, src => src.TotalUvtQtyAgreed)
                .Map(dest => dest.Active, src => src.Active)
                .Map(dest => dest.Clauses, src => src.ContractClauses
                    .Select(cc => cc.Clause)
                    .Adapt<List<ClauseSelectDto>>());

            // PremisesLeased -> PremisesLeasedSelectDto
            config.NewConfig<PremisesLeased, PremisesLeasedSelectDto>()
                .Map(dest => dest.EstablishmentId, src => src.Establishment.Id)
                .Map(dest => dest.EstablishmentName, src => src.Establishment.Name)
                .Map(dest => dest.Description, src => src.Establishment.Description)
                .Map(dest => dest.AreaM2, src => src.Establishment.AreaM2)
                .Map(dest => dest.RentValueBase, src => src.Establishment.RentValueBase)
                .Map(dest => dest.Address, src => src.Establishment.Address)
                .Map(dest => dest.PlazaName, src => src.Establishment.Plaza.Name)
                .Map(dest => dest.Images, src => src.Establishment.Images.Adapt<List<Entity.DTOs.Implements.Utilities.Images.ImageSelectDto>>());
        }
    }
}

