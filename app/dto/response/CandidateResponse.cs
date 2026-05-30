namespace rut_shop.net.dto.response;


public class CandidateResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
  
}