using Microsoft.EntityFrameworkCore;
using rut_shop.net.database;
using rut_shop.net.dto.request;
using rut_shop.net.interfaces;
using rut_shop.net.model;
using rut_shop.net.exceptions;
namespace rut_shop.net.services;

public class VacancyService(HrDbContext db) : IVacancyService
{
    public async Task<IReadOnlyList<Vacancy>> GetAllAsync()
        => await db.Vacancies
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync();

    public async Task<Vacancy?> GetByIdAsync(Guid id)
        => await db.Vacancies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Vacancy> AddAsync(CreateVacancyRequest request)
    {
        ValidateVacancyFields(request.Title, request.Description, request.Department);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Vacancies.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Вакансия с идентификатором {id} уже существует.");
        }

        var entity = new Vacancy
        {
            Id = id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Department = request.Department.Trim(),
            Salary = request.Salary,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        db.Vacancies.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Vacancy?> UpdateAsync(Guid id, UpdateVacancyRequest request)
    {
        ValidateVacancyFields(request.Title, request.Description, request.Department);

        var entity = await db.Vacancies.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        entity.Title = request.Title.Trim();
        entity.Description = request.Description.Trim();
        entity.Department = request.Department.Trim();
        entity.Salary = request.Salary;
        entity.IsActive = request.IsActive;

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Vacancies.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

      
        

        db.Vacancies.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
    
    
    public async Task<Application?> CreateApplicationAsync(CreateApplicationRequest request)
    {
      
        var vacancy = await db.Vacancies.FindAsync(request.VacancyId);
        if (vacancy is null) return null; 
 
        
        if (!vacancy.IsActive)
        {
            throw new BusinessException("Невозможно подать отклик: данная вакансия закрыта или заархивирована.");
        }

        
        var candidate = await db.Candidates.FindAsync(request.CandidateId);
        if (candidate is null) return null; // Если кандидата нет —  404

       
        var application = new Application
        {
            Id = Guid.NewGuid(),
            VacancyId = request.VacancyId,
            CandidateId = request.CandidateId,
            CandidateName = candidate.FullName, 
            VacancyTitle = vacancy.Title,
            CreatedAtUtc = DateTime.UtcNow,
            Status = "New"
        };

      
        vacancy.Description = vacancy.Description + " (Получен отклик)";

        db.Applications.Add(application);
        await db.SaveChangesAsync();

        return application;
    }

    public async Task<IReadOnlyList<Application>> GetApplicationsAsync()
    {
        return await db.Applications.AsNoTracking().ToListAsync();
    }
    
    
    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
       
        var application = await db.Applications.FindAsync(id);
        if (application is null)
        {
            return false; // Вернет 404 Not Found
        }

      
        db.Applications.Remove(application);
        await db.SaveChangesAsync();
        return true;
    }
    
    public async Task<Application?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request)
    {
        
        var application = await db.Applications.FindAsync(id);
        if (application is null)
        {
            return null; 
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw new ArgumentException("Статус отклика не может быть пустым.");
        }

        application.Status = request.Status.Trim();

        db.Applications.Update(application);
        await db.SaveChangesAsync();

        return application;
    }
    
    private static void ValidateVacancyFields(string title, string description, string department)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Название вакансии не должно быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Описание вакансии не должно быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(department))
        {
            throw new ArgumentException("Указание отдела/департамента обязательно.");
        }
    }
}