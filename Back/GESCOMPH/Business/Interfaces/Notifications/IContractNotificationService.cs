namespace Business.Interfaces.Notifications
{
    public interface IContractNotificationService
    {
        Task NotifyContractCreated(int contractId, int personId);
        Task NotifyContractExpired(int contractId, int personId);
        Task NotifyContractStatusChanged(int contractId, bool active);
        Task NotifyContractDeleted(int contractId); 
    }
}
