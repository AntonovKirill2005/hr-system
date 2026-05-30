using rut_shop.net.dto.request;
using rut_shop.net.model;

namespace rut_shop.net.interfaces;


public interface ICandidateService
{
    Task<IReadOnlyList<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(Guid id);
    Task<Candidate> AddAsync(CreateCandidateRequest request);
    Task<Candidate?> UpdateAsync(Guid id, UpdateCandidateRequest request);
    Task<bool> DeleteAsync(Guid id);
}