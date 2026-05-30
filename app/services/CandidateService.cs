using Microsoft.EntityFrameworkCore;
using rut_shop.net.database;
using rut_shop.net.dto.request;
using rut_shop.net.interfaces;
using rut_shop.net.model;

namespace rut_shop.net.services;


public class CandidateService(HrDbContext db) : ICandidateService
{
    public async Task<IReadOnlyList<Candidate>> GetAllAsync()
        => await db.Candidates
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync();

    public async Task<Candidate?> GetByIdAsync(Guid id)
        => await db.Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Candidate> AddAsync(CreateCandidateRequest request)
    {
        ValidateCandidateFields(request.FullName, request.Email);

       
      

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Candidates.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Кандидат с идентификатором {id} уже существует.");
        }

        var entity = new Candidate
        {
            Id = id,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            ResumeUrl = request.ResumeUrl.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "В поиске работы" : request.Status.Trim(),
         
        };

        db.Candidates.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Candidate?> UpdateAsync(Guid id, UpdateCandidateRequest request)
    {
        ValidateCandidateFields(request.FullName, request.Email);
        
       

        var entity = await db.Candidates.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.FullName = request.FullName.Trim();
        entity.Email = request.Email.Trim();
        entity.ResumeUrl = request.ResumeUrl.Trim();
        entity.Status = request.Status.Trim();
       

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Candidates.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        db.Candidates.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateCandidateFields(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("ФИО кандидата не должно быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email кандидата не должен быть пустым.");
        }
    }
}