using System.Text.Json.Serialization;

namespace WebApplication1.Features.Admin.Dtos;

public class AssignPersonaRequest
{
    [JsonPropertyName("personaTypeId")]
    public Guid PersonaTypeId { get; set; }

    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }
}
