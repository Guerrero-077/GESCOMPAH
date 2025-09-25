using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace WebGESCOMPAH.RealTime
{
    public class SignalRPermissionsNotificationService : IPermissionsNotificationService
    {
        private readonly IHubContext<SecurityHub> _hub;

        public SignalRPermissionsNotificationService(IHubContext<SecurityHub> hub)
        {
            _hub = hub;
        }

        public async Task NotifyPermissionsUpdated(IEnumerable<int> userIds)
        {
            var arr = (userIds ?? Enumerable.Empty<int>()).Distinct().ToArray();
            await _hub.Clients.All.SendAsync("permissions:updated", new { userIds = arr });
        }
    }
}

