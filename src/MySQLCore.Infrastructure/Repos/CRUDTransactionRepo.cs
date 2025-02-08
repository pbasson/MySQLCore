using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySQLCore.Core.CoreHelpers;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Infrastructure.Entities.Tables;
using MySQLCore.Infrastructure.Models;

namespace MySQLCore.Infrastructure.Repos;

public class CRUDTransactionRepo : BaseRepo, ICRUDTransactionRepo 
{
    public CRUDTransactionRepo(MySQLCoreDBContext dBContext, IMapper mapper) : base(dBContext, mapper) {
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsAsync() {
        try {
            var results = await _dBContext.CRUDTransaction.AsNoTracking().ToListAsync();
            return _mapper.Map<List<CRUDTransactionDTO>>(results);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<List<CRUDTransactionDTO>> GetAllRecordsPaginationAsync(int page) {
        try {
            var settings = new PageSettings();
            var results = await _dBContext.CRUDTransaction.OrderBy(x=>x.Id).Skip( settings.SkipCount(page) ).Take(settings.PageSize).AsNoTracking().ToListAsync();
            return _mapper.Map<List<CRUDTransactionDTO>>(results);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<CRUDTransactionDTO> GetRecordByIdAsync(int id) {
        try {
            var result = await _dBContext.CRUDTransaction.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<CRUDTransactionDTO>(result);
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> CreateRecordAsync(CreateCRUDTransactionDTO dto) {
        try{
            if ( dto.NullChecker() ) {
                var mapped = _mapper.Map<CRUDTransaction>(dto);
                _dBContext.CRUDTransaction.Add(mapped);
                return await SaveChangesAsync();
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto) {
        try {
            CRUDTransaction? existDTO = await FindRecordByIdAsync(dto.Id);

            if (existDTO != null)
            {
                var mapped = _mapper.Map<CRUDTransaction>(dto);
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

    public async Task<bool> DeleteRecordByIdAsync(int id) {
        try {
            CRUDTransaction? existDTO = await FindRecordByIdAsync(id);
            if (existDTO != null) {
                _dBContext.CRUDTransaction.Remove(existDTO);
                return await SaveChangesAsync();
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }
    
    private async Task<CRUDTransaction?> FindRecordByIdAsync(int id) {
        var result = await _dBContext.CRUDTransaction.FindAsync(id);
        return result.NullChecker() ? result : null;
    }
}
