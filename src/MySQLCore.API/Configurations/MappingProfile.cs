using AutoMapper;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Infrastructure.Entities.Tables;

namespace MySQLCore.API.Configurations;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        SetMapping();  
    }

    private void SetMapping() {
        CreateMap<CRUDTransactionDTO, CRUDTransaction>().ReverseMap();
        CreateMap<CreateCRUDTransactionDTO, CRUDTransaction>();
        CreateMap<UpdateCRUDTransactionDTO, CRUDTransaction>();
    }


}
