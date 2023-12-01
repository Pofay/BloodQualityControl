using HarmonyLib;
using ProjectM;
using ProjectM.Shared.Systems;
using Unity.Entities;

namespace BloodQualityControl.Services
{
    public class BloodQualityControlService
    {
        private const float MIN_BLOOD_QUALITY = 5f;
        private const float MAX_BLOOD_QUALITY = 100f;

        private float lowestBloodQuality = MIN_BLOOD_QUALITY;
        private float highestBloodQuality = MAX_BLOOD_QUALITY;

        public float MinBloodQuality
        {
            get => lowestBloodQuality;
            set
            {
                lowestBloodQuality = value;
                BloodQualitySpawnSystem_Patch.Enabled = true;
            }
        }

        public float MaxBloodQuality
        {
            get => highestBloodQuality;
            set
            {
                highestBloodQuality = value;
                BloodQualitySpawnSystem_Patch.Enabled = true;
            }
        }

        public void Disable()
        {
            BloodQualitySpawnSystem_Patch.Enabled = false;
        }
    }

    [HarmonyPatch(typeof(BloodQualitySpawnSystem), nameof(BloodQualitySpawnSystem.OnUpdate))]
    public static class BloodQualitySpawnSystem_Patch
    {
        public static bool Enabled { get; internal set; }

        static void Postfix(BloodQualitySpawnSystem __instance)
        {
            if (!Enabled) return;

            var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var entity in entities)
            {
                if (PluginServices.ServerEntityManager.HasComponent<PrefabGUID>(entity))
                {
                    if (PluginServices.ServerEntityManager.TryGetComponentData(entity, out BloodConsumeSource blood))
                    {
                        var characterName = PluginServices.GetCharacterNameFromPrefabGUID(PluginServices.GetPrefabGUID(entity));
                        PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {characterName} has spawned with blood quality {blood.BloodQuality} ");
                        ConfigureBloodQuality(entity, characterName, blood);
                    }
                }
            }
        }

        private static void ConfigureBloodQuality(Entity entity, string prefabName, BloodConsumeSource blood)
        {
            var minBloodQuality = PluginServices.BloodQualityControlService.MinBloodQuality;
            var MaxBloodQuality = PluginServices.BloodQualityControlService.MaxBloodQuality;
            if (blood.BloodQuality <= minBloodQuality)
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{prefabName} has blood quality {blood.BloodQuality} <= {minBloodQuality}");
                // Original uses a curve, implementation might change later.
                var newBloodQuality = UnityEngine.Random.Range(minBloodQuality, MaxBloodQuality);
                blood.BloodQuality = newBloodQuality;
                PluginServices.ServerEntityManager.SetComponentData(entity, blood);
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: Changing blood quality of {entity}_{prefabName} to {newBloodQuality}");
            }
            else
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{prefabName} already has blood quality {blood.BloodQuality} >= {minBloodQuality}");
            }
        }
    }
}