namespace rut_shop.net.dto.request;

public record CreateApplicationRequest(
    Guid VacancyId,
    Guid CandidateId,
    string CandidateName
);