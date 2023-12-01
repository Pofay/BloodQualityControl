using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using ProjectM;
using ProjectM.Shared.Systems;
using Unity.Entities;

namespace BloodQualityControl.Services
{
    public class BloodQualityControlService
    {
    }



    [HarmonyPatch(typeof(BloodQualitySpawnSystem), nameof(BloodQualitySpawnSystem.OnUpdate))]
    public static class BloodQualitySpawnSystem_Patch
    {
        private const float MIN_BLOOD_THRESHOLD = 40;

        static void Postfix(BloodQualitySpawnSystem __instance)
        {
            var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

            foreach (var entity in entities)
            {
                if (PluginServices.ServerEntityManager.HasComponent<PrefabGUID>(entity))
                {
                    if (PluginServices.ServerEntityManager.TryGetComponentData(entity, out BloodConsumeSource blood))
                    {
                        var characterName = PluginServices.GetCharacterNameFromPrefabGUID(PluginServices.GetPrefabGUID(entity));
                        PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {characterName} has spawned with blood quality {blood.BloodQuality} ");
                        ChangeBloodQualityIfBelowThreshold(entity, characterName, blood, MIN_BLOOD_THRESHOLD);
                    }
                }
            }
        }

        private static void ChangeBloodQualityIfBelowThreshold(Entity entity, string prefabName, BloodConsumeSource blood, float threshold)
        {
            if (blood.BloodQuality <= threshold)
            {
                var newBloodQuality = UnityEngine.Random.Range(threshold, 100f);
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{prefabName} has blood quality {blood.BloodQuality} <= {threshold}");
                blood.BloodQuality = newBloodQuality;
                PluginServices.ServerEntityManager.SetComponentData(entity, blood);
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: Changing blood quality of {entity}_{prefabName} to {newBloodQuality}");
            }
            else
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{prefabName} already has blood quality {blood.BloodQuality} >= {threshold}");
            }
        }
    }
}