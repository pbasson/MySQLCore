using AutoMapper;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Infrastructure.Entities.Tables;

namespace MySQLCore.API.Configurations;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        SetMapping();  
    }

    private void SetMapping() {

        #region CRUDTransaction
        CreateMap<CRUDTransactionDTO, CRUDTransaction>().ReverseMap();
        CreateMap<CreateCRUDTransactionDTO, CRUDTransaction>();
        CreateMap<UpdateCRUDTransactionDTO, CRUDTransaction>();
        #endregion 
            
        #region ImageTransaction
        CreateMap<ImageGalleryDTO, ImageGallery>().ReverseMap();
        CreateMap<ImageTransactionDTO, ImageTransaction>().ReverseMap();
        #endregion
    }


}
