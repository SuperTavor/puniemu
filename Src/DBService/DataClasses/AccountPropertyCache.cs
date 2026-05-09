using System.Reflection;

namespace Puniemu.Src.DBService.DataClasses
{
    public static class AccountPropertyCache
    {
        public static Dictionary<string, PropertyInfo> Cache = new();
    }
}
