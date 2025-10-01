namespace Business.Interfaces.Notifications
{
    public interface IPermissionsNotificationService
    {
        /// <summary>
        /// Notifica a los clientes que los permisos/roles han cambiado para ciertos usuarios.
        /// </summary>
        Task NotifyPermissionsUpdated(IEnumerable<int> userIds);
    }
}
