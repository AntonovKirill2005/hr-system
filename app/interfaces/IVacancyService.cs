using rut_shop.net.dto.request;
using rut_shop.net.model;

namespace rut_shop.net.interfaces;


public interface IVacancyService
{
    Task<IReadOnlyList<Vacancy>> GetAllAsync();
    Task<Vacancy?> GetByIdAsync(Guid id);
    Task<Vacancy> AddAsync(CreateVacancyRequest request);
    Task<Vacancy?> UpdateAsync(Guid id, UpdateVacancyRequest request);
    Task<bool> DeleteAsync(Guid id);
    
    Task<Application?> CreateApplicationAsync(CreateApplicationRequest request);
    Task<IReadOnlyList<Application>> GetApplicationsAsync();
    Task<bool> DeleteApplicationAsync(Guid id);
    Task<Application?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request);
}