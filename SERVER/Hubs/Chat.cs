using Microsoft.AspNetCore.SignalR;
using SERVER.Extensions;
using SignalRSwaggerGen.Attributes;

namespace SERVER.Hubs
{
    [SignalRHub("chat")]
    public class Chat : Hub
    {
        private readonly ILogger<Chat> _logger;

        public Chat(ILogger<Chat> logger)
        {
            _logger = logger;
        }
        public Task Login(string user)
        {
            Context.AddUserName(user);
            return Task.CompletedTask;
        }

        [return: SignalRReturn(typeof(Task<(string, string ,IEnumerable<string>)>), 200, "onJoin(group, userName, usersInGroup)")]
        [return: SignalRReturn(typeof(Task<string>), 201, "onGroupCreated(group)")]
        public async Task JoinGroup(string group)
        {
            Context.AddUserGroup(group);
            var userName = Context.GetUserName();
            if (!InMemoryDatabase.IsGroupExist(group))
            {
                await Clients.All.SendAsync("onGroupCreated", group).ConfigureAwait(false);
            }

            var usersInGroup = InMemoryDatabase.ClearUserGroupsAndAdd(group, userName);

            await Groups.AddToGroupAsync(Context.ConnectionId, group).ConfigureAwait(false);

            await Clients.Group(group).SendAsync("onJoin", group, userName, usersInGroup)
                .ConfigureAwait(false);
        }

        [return: SignalRReturn(typeof(Task<(string, string, IEnumerable<string>)>), 202, "onLeave(group, userName, usersInGroup)")]
        public async Task LeaveGroup(string group)
        {
            Context.RemoveUserGroup(group);
            var userName = Context.GetUserName();

            var usersInGroup = InMemoryDatabase.RemoveUserFromGroup(group, userName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group).ConfigureAwait(false);
            await Clients.Group(group).SendAsync("onLeave", group, userName, usersInGroup).ConfigureAwait(false);
        }

        [return: SignalRReturn(typeof(Task<(string, string, IEnumerable<string>)>), 202, "onLeave(group, userName, usersInGroup)")]
        public async Task Disconnect()
        {
            var group = Context.GetUserGroup();
            if (group is null) return;

            await this.LeaveGroup(group).ConfigureAwait(false);
        }

        [return: SignalRReturn(typeof(Task<(string, string, string)>), 202, "onMessage(group, userName, text)")]
        public async Task SendMessage(string text)
        {
            var group = Context.GetUserGroup();
            if (group is null) return;

            await Clients.Group(
                group).SendAsync("onMessage",
                group,
                Context.GetUserName(),
                text)
                .ConfigureAwait(false);
        }
    }
}
