using Business.CustomJWT;

namespace WebGESCOMPAH.Infrastructure
{
    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _ctx;
        public CurrentUser(IHttpContextAccessor ctx) => _ctx = ctx;

        public int? PersonId =>
            int.TryParse(_ctx.HttpContext?.User?.FindFirst("person_id")?.Value, out var id) ? id : null;

        public bool IsInRole(string role) =>
            _ctx.HttpContext?.User?.IsInRole(role) == true;

        public bool EsAdministrador => IsInRole(AppRoles.Administrador);
        public bool EsArrendador => IsInRole(AppRoles.Arrendador);
    }
}
