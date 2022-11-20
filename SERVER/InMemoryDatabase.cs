using System.Collections.Concurrent;

namespace SERVER
{
    public static class InMemoryDatabase
    {
        public static ConcurrentDictionary<string, List<string>> GroupsUsers =
            new ConcurrentDictionary<string, List<string>>();

        public static IEnumerable<string> ClearUserGroupsAndAdd(string group, string user)
        {
            RemoveUserFromGroups(user);
            return AddUserToGroup(group, user);
        }

        public static bool IsGroupExist(string group)
        {
            return GroupsUsers.ContainsKey(group);
        }

        public static IEnumerable<string> AddUserToGroup(string group, string user)
        {
            if (GroupsUsers.ContainsKey(group))
            {
                if (GroupsUsers[group].Contains(user))
                {
                    return GroupsUsers[group];
                }

                GroupsUsers[group].Add(user);
                return GroupsUsers[group];
            }
            var users = new List<string> { user };
            GroupsUsers.TryAdd(group, users);
            return users;
        }

        public static IEnumerable<string> RemoveUserFromGroup(string group, string user)
        {
            if (!GroupsUsers.ContainsKey(group) || !GroupsUsers[group].Contains(user))
            {
                return Enumerable.Empty<string>();
            }

            GroupsUsers[group].Remove(user);

            if (GroupsUsers[group].Count == 0)
            {
                GroupsUsers.Remove(group, out var _);
                return Enumerable.Empty<string>();
            }

            return GroupsUsers[group];
        }

        public static void RemoveUserFromGroups(string user)
        {
            foreach (var group in GroupsUsers.Keys)
            {
                RemoveUserFromGroup(group, user);
            }
        }
    }
}
