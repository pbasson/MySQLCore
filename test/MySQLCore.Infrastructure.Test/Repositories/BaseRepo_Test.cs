using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySQLCore.API.Configurations;
using MySQLCore.Core.Test.Helpers;
using MySQLCore.Infrastructure.Models;

namespace MySQLCore.Infrastructure.Test.Repositories {
    public class BaseRepo_Test : Base_Test, IDisposable {
        public readonly MySQLCoreDBContext _dBContext = default!;
        public readonly IMapper _mapper; 

        public BaseRepo_Test() {
            var option = new DbContextOptionsBuilder<MySQLCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "MySQLCore" ).Options;

            _dBContext = new MySQLCoreDBContext(option);
            _mapper = new MapperConfiguration( x => { x.AddProfile(new MappingProfile() ); } ).CreateMapper();
        }

        public void Dispose() { 
            _dBContext.Database.EnsureDeleted();
            _dBContext.Dispose();
        }

        public void Add<T>(T response) where T : class
        {
            _dBContext.Add(response);
            _dBContext.SaveChanges();
        }

        public void AddRange<T>(List<T> fixture) where T : class 
        {
            _dBContext.AddRange(fixture);
            _dBContext.SaveChanges();
        }
    }
}