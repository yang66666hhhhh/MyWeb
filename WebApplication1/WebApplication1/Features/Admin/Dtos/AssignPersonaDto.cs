namespace WebApplication1.Features.Admin.Dtos;

public class AssignPersonaRequest
{
    public Guid PersonaTypeId { get; set; }
    public bool IsPrimary { get; set; }
}
