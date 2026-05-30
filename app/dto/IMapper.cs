using rut_shop.net.dto.response;
using rut_shop.net.model;

namespace rut_shop.net.dto;


public interface IMapper
{
   
    VacancyResponse Map(Vacancy vacancy);

   
    CandidateResponse Map(Candidate candidate);
}