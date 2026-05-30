using rut_shop.net.dto.response;
using rut_shop.net.model;

namespace rut_shop.net.dto;


public class Mapper : IMapper
{
    public VacancyResponse Map(Vacancy vacancy) => new()
    {
        Id = vacancy.Id,
        Title = vacancy.Title,
        Description = vacancy.Description,
        Department = vacancy.Department,
        Salary = vacancy.Salary,
        IsActive = vacancy.IsActive,
        CreatedAt = vacancy.CreatedAt
    };

    public CandidateResponse Map(Candidate candidate) => new()
    {
        Id = candidate.Id,
        FullName = candidate.FullName,
        Email = candidate.Email,
        ResumeUrl = candidate.ResumeUrl,
        Status = candidate.Status,
    };
}