namespace rut_shop.net.model;

public class Application
{
    public Guid Id { get; set; }
    public Guid VacancyId { get; set; }
    public Guid CandidateId { get; set; }
    
    public string CandidateName { get; set; } = string.Empty;
    public string VacancyTitle { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; }
    public string Status { get; set; } = "New";
}