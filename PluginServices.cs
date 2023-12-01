using BepInEx.Logging;
using BloodQualityControl.Data;
using BloodQualityControl.Services;
using ProjectM;
using Unity.Entities;

namespace BloodQualityControl
{
    public static class PluginServices
    {
        public static EntityManager ServerEntityManager { get; private set; }
        public static ManualLogSource Logger { get; } = Plugin.Logger;
        public static BloodQualityControlService BloodQualityControlService { get; private set; }

        internal static void Initialize(World Server)
        {
            ServerEntityManager = Server.EntityManager;
            BloodQualityControlService = new();
            Logger.LogInfo($"{nameof(Initialize)} completed");
        }

        internal static string GetCharacterNameFromPrefabGUID(PrefabGUID guid)
        {
            if (NpcDictionary.NameFromPrefab.TryGetValue(guid.GuidHash, out string prefabName))
            {
                return prefabName;
            }
            else
            {
                return "Unrecognized";
            }
        }

        internal static PrefabGUID GetPrefabGUID(Entity entity)
        {
            if (ServerEntityManager.TryGetComponentData(entity, out PrefabGUID guid))
            {
                return guid;
            }
            else
            {
                var emptyPrefabGuid = PrefabGUID.Empty;
                emptyPrefabGuid.GuidHash = 0;
                return emptyPrefabGuid;
            }
        }
    }
}