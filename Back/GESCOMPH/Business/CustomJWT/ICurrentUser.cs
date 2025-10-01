namespace Business.CustomJWT
{
    public interface ICurrentUser
    {
        int? PersonId { get; }
        bool IsInRole(string role);
        bool EsAdministrador { get; }
        bool EsArrendador { get; }
    }
}
