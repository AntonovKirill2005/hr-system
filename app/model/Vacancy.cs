namespace rut_shop.net.model;


public class Vacancy
{
    
    public Guid Id { get; set; }


    public string Title { get; set; } = string.Empty;

    
    public string Description { get; set; } = string.Empty;

   
    public string Department { get; set; } = string.Empty;

   
    public decimal Salary { get; set; }

    
    public bool IsActive { get; set; } = true;

    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
}