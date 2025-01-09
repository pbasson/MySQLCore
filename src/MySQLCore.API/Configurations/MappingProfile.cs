using System;
using AutoMapper;
using MySQLCore.Core.DTOs;
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
    }


}
