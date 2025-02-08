using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySQLCore.Infrastructure.Models;

namespace MySQLCore.Infrastructure.Repos;

public class BaseRepo(MySQLCoreDBContext dBContext, IMapper mapper)
{
    public readonly MySQLCoreDBContext _dBContext = dBContext;
    public readonly IMapper _mapper = mapper;

    public void UpdateEntity(object existDTO, object mapped)
    {
        _dBContext.Entry(existDTO).State = EntityState.Detached;
        _dBContext.Entry(mapped).State = EntityState.Modified;
    }

}
