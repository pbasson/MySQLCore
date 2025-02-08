using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySQLCore.Core.CoreHelpers;
using MySQLCore.Core.Interfaces.InterfaceRepos;
using MySQLCore.Core.Models.DTOs;
using MySQLCore.Infrastructure.Entities.Tables;
using MySQLCore.Infrastructure.Models;

namespace MySQLCore.Infrastructure.Repos;

public class CRUDTransactionRepo : ICRUDTransactionRepo
{
    private readonly MySQLCoreDBContext _dBContext = default!;
    private readonly IMapper _mapper = default!;

    public CRUDTransactionRepo(MySQLCoreDBContext dBContext, IMapper mapper) {
        _dBContext = dBContext;
        _mapper = mapper;    
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
            var results = await _dBContext.CRUDTransaction.Skip( settings.SkipCount(page) ).Take(settings.PageSize).AsNoTracking().ToListAsync();
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
                var result = await _dBContext.SaveChangesAsync();

                return result > 0;
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> UpdateRecordAsync(UpdateCRUDTransactionDTO dto) {
        try {
            CRUDTransaction? existDTO = await FindRecord(dto.Id);

            if (existDTO != null) {
                var mapped = _mapper.Map<CRUDTransaction>(dto);
                existDTO.SetCreated(mapped);
                _dBContext.Entry(existDTO).State = EntityState.Detached;
                _dBContext.Entry(mapped).State = EntityState.Modified;
                var result = await _dBContext.SaveChangesAsync();
                return result > 0;
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }

    public async Task<bool> DeleteRecordByIdAsync(int id) {
        try {
            CRUDTransaction? existDTO = await FindRecord(id);
            if (existDTO.NullChecker()) {
                _dBContext.CRUDTransaction.Remove(existDTO);
                var result = await _dBContext.SaveChangesAsync();
                return result > 0;                
            }

            return false;
        }
        catch (Exception) {
            throw;
        }
    }
    
    private async Task<CRUDTransaction?> FindRecord(int id) {
        var result = await _dBContext.CRUDTransaction.FindAsync(id);
        return result.NullChecker() ? result : null;
    }
}
