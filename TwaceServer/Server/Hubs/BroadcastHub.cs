using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwaceServer.Server.Hubs
{
    public class BroadcastHub : Hub
    {
        public async Task JoinToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string groupName)
        {
            //await Clients.All.SendAsync("ReceiveMessage");
            await Clients.Group(groupName).SendAsync("ReceiveMessage");
        }

        public async Task NewRequestMessage(string groupname)
        {
            await Clients.Group(groupname).SendAsync("NewRequestMessage", "Пришла новая заявка.");
        }
    }
}
