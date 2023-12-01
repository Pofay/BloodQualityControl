using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Logging;
using BloodQualityControl.Data;
using ProjectM;
using Unity.Entities;

namespace BloodQualityControl
{
    public static class PluginServices
    {
        public static EntityManager ServerEntityManager { get; private set; }
        public static ManualLogSource Logger { get; } = Plugin.Logger;

        internal static void Initialize(World Server)
        {
            ServerEntityManager = Server.EntityManager;
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
            PrefabGUID guid;
            try
            {
                guid = ServerEntityManager.GetComponentData<PrefabGUID>(entity);
            }
            catch
            {
                guid.GuidHash = 0;
            }
            return guid;
        }
    }
}