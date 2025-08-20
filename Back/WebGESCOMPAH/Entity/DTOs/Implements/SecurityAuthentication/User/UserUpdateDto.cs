using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public sealed class UserUpdateDto : BaseDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Document { get; init; }
        public required string Phone { get; init; }
        public required string Address { get; init; }
        public required int CityId { get; init; }

        // Usuario
        public required string Email { get; init; }

        public IReadOnlyList<int> RoleIds { get; init; } = Array.Empty<int>();

    }
}
