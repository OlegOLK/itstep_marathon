using Microsoft.AspNetCore.SignalR;

namespace SERVER.Extensions
{
    public static class HubCallerContextEx
    {
        public static string UserNameContextKey = "name";
        public static string UserGroupContextKey = "group";
        public static bool AddUserName(this HubCallerContext ctx, string name)
        {
            return ctx.Items.TryAdd(UserNameContextKey, name);
        }

        public static string GetUserName(this HubCallerContext ctx)
        {
            var isValuePresent = ctx.Items.TryGetValue(UserNameContextKey, out var name);
            return isValuePresent ? name as string : string.Empty;
        }

        public static bool AddUserGroup(this HubCallerContext ctx, string group)
        {
            if (ctx.Items.ContainsKey(UserGroupContextKey))
            {
                return ctx.Items.Remove(UserGroupContextKey);
            }
            return ctx.Items.TryAdd(UserGroupContextKey, group);
        }

        public static bool RemoveUserGroup(this HubCallerContext ctx, string group)
        {
            if (ctx.Items.ContainsKey(UserGroupContextKey))
            {
                ctx.Items.Remove(UserGroupContextKey);
            }
            return true;
        }

        public static string? GetUserGroup(this HubCallerContext ctx)
        {
            var isValuePresent = ctx.Items.TryGetValue(UserGroupContextKey, out var group);
            return isValuePresent ? group as string : null;
        }
    }
}
