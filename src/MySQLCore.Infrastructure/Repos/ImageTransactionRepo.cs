using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySQLCore.Core.CoreHelpers;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs.ImageDTOs;
using MySQLCore.Infrastructure.Entities.Tables.ImageTables;
using MySQLCore.Infrastructure.Models;

namespace MySQLCore.Infrastructure.Repos;

public class ImageTransactionRepo : BaseRepo, IImageTransactionRepo
{
    public ImageTransactionRepo(MySQLCoreDBContext dBContext, IMapper mapper) : base(dBContext, mapper) {
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsAsync() {
        try {
            var results = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries).AsNoTracking().ToListAsync();
            return _mapper.Map<List<ImageTransactionDTO>>(results);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<List<ImageTransactionDTO>> GetAllRecordsPaginationAsync(int page) {
        try {
            var settings = new PageSettings();
            var results = await _dBContext.ImageTransaction.Skip(settings.SkipCount(page)).Take(settings.PageSize)
                        .Include(x => x.ImageGalleries).AsNoTracking().ToListAsync();
            return _mapper.Map<List<ImageTransactionDTO>>(results);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<ImageTransactionDTO> GetRecordByIdAsync(int id)  {
        try {
            var result = await _dBContext.ImageTransaction.Include(x => x.ImageGalleries).FirstOrDefaultAsync(x => x.ImageTransactionID == id);
            return _mapper.Map<ImageTransactionDTO>(result);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> CreateRecordAsync(ImageTransactionDTO dto) {
        try {
            if ( dto.NullChecker() ) {
                var mapped = _mapper.Map<ImageTransaction>(dto);
                _dBContext.ImageTransaction.Add(mapped);
                return await SaveChangesAsync();
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> UpdateRecordAsync(ImageTransactionDTO dto) {
        try {
            ImageTransaction? existDTO = await FindRecord(dto.ImageTransactionID);

            if (existDTO != null) {
                var mapped = _mapper.Map<ImageTransaction>(dto);
                existDTO.SetCreated(mapped);
                UpdateEntity(existDTO, mapped);
                return await SaveChangesAsync();
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> DeleteRecordByIdAsysc(int id)  {
        try {
            ImageTransaction? existDTO = await FindRecord(id);
            if ( existDTO != null ) {
                _dBContext.ImageTransaction.Remove(existDTO);
                return await SaveChangesAsync();
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }
    
    private async Task<ImageTransaction?> FindRecord(int id) {
        var result = await _dBContext.ImageTransaction.FindAsync(id);
        return result.NullChecker() ? result : null;
    }
}

