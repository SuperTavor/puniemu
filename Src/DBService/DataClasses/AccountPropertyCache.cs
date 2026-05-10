using System.Collections.Concurrent;
using System.Reflection;

namespace Puniemu.Src.DBService.DataClasses
{
    public static class AccountPropertyCache
    {
        public static ConcurrentDictionary<string, PropertyInfo> Cache = new();
    }
}
