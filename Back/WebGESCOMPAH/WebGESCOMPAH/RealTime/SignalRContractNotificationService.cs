using Business.Interfaces.Notifications;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebGESCOMPAH.RealTime
{
    public class SignalRContractNotificationService : IContractNotificationService
    {
        private readonly IHubContext<ContractsHub> _hubContext;

        public SignalRContractNotificationService(IHubContext<ContractsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyContractCreated(int contractId, int personId)
        {
            await _hubContext.Clients.Group("Admin")
                .SendAsync("contracts:created", new { id = contractId });

            await _hubContext.Clients.Group($"tenant-{personId}")
                .SendAsync("contracts:created", new { id = contractId });
        }

        public async Task NotifyContractExpired(int contractId, int personId)
        {
            await _hubContext.Clients.Group("Admin")
                .SendAsync("contracts:expired", new { id = contractId });

            await _hubContext.Clients.Group($"tenant-{personId}")
                .SendAsync("contracts:expired", new { id = contractId });
        }

        public async Task NotifyContractStatusChanged(int contractId, bool active)
        {
            await _hubContext.Clients.All
                .SendAsync("contracts:statusChanged", new { id = contractId, active });
        }

        public async Task NotifyContractDeleted(int contractId)
        {
            await _hubContext.Clients.All
                .SendAsync("contracts:deleted", new { id = contractId });
        }
    }
}
