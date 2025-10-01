using System.ComponentModel.DataAnnotations;

namespace WebGESCOMPH.Contracts.Requests
{
    public sealed record ChangeActiveStatusRequest
    {
        // .NET 8 (preferido): exige presencia en el JSON
        // [JsonRequired] public required bool Active { get; init; }

        // Compatibilidad .NET 6/7:
        [Required] public bool? Active { get; init; }
    }
}
