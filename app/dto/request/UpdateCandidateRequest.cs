

namespace rut_shop.net.dto.request;


public class UpdateCandidateRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}