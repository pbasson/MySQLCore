using AutoMapper;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Infrastructure.Entities.Tables;
using MySQLCore.Infrastructure.Entities.Tables.ImageTables;

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
        CreateMap<CreateCRUDTransactionDTO, CRUDTransaction>().ReverseMap();
        CreateMap<UpdateCRUDTransactionDTO, CRUDTransaction>().ReverseMap();
        #endregion 
            
        #region ImageTransaction
        CreateMap<ImageTransactionDTO, ImageTransaction>().ReverseMap();
        CreateMap<CreateImageTransactionDTO, ImageTransaction>().ReverseMap();
        CreateMap<UpdateImageTransactionDTO, ImageTransaction>().ReverseMap();
        
        CreateMap<ImageGalleryDTO, ImageGallery>().ReverseMap();
        CreateMap<CreateImageGalleryDTO, ImageGallery>().ReverseMap();
        #endregion
    }


}
