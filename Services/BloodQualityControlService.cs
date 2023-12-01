using System;
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

        private Action<string> OnFailure;
        private Action<string> OnSuccess;

        public BloodQualityControlService()
        {
            this.MinBloodQuality = MIN_BLOOD_QUALITY;
            this.MaxBloodQuality = MAX_BLOOD_QUALITY;
        }

        public float MinBloodQuality { get; private set; }
        public float MaxBloodQuality { get; private set; }

        public void HookReplyCallbacks(Action<string> OnSuccess, Action<string> OnFailure)
        {
            this.OnSuccess = OnSuccess;
            this.OnFailure = OnFailure;
        }

        public void OverrideBloodQualitySettings(float minBloodQuality, float maxBloodQuality)
        {
            if (!(5f <= minBloodQuality && minBloodQuality <= 100f))
            {
                OnFailure($"Minimum Blood Quality given {minBloodQuality} is not in the range of 5-100");
                return;
            }

            if (!(5f <= maxBloodQuality && maxBloodQuality <= 100f))
            {
                OnFailure($"Maximum Blood Quality given {maxBloodQuality} is not in the range of 5-100");
                return;
            }

            if (minBloodQuality > maxBloodQuality)
            {
                OnFailure($"The given Minimum Blood Quality {minBloodQuality} is higher than the current Max Blood Quality {maxBloodQuality}");
                return;
            }
            else
            {
                MinBloodQuality = minBloodQuality;
                MaxBloodQuality = maxBloodQuality;
                BloodQualitySpawnSystem_Patch.Enabled = true;
                OnSuccess(GetFormattedSettings());
            }
        }

        public void Disable()
        {
            BloodQualitySpawnSystem_Patch.Enabled = false;
        }

        private string GetFormattedSettings()
        {
            return $"\nMIN BLOOD QUALITY: {MinBloodQuality}\nMAX BLOOD QUALITY: {MaxBloodQuality}\nBLOOD QUALITY RANGE: {MinBloodQuality}-{MaxBloodQuality}";
        }

        public void UnhookReplyCallbacks()
        {
            this.OnSuccess = null;
            this.OnFailure = null;
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
                        OverrideBloodQualityCalculation(entity, characterName, blood);
                    }
                }
            }
        }

        private static void OverrideBloodQualityCalculation(Entity entity, string prefabName, BloodConsumeSource blood)
        {
            var minBloodQuality = PluginServices.BloodQualityControlService.MinBloodQuality;
            var maxBloodQuality = PluginServices.BloodQualityControlService.MaxBloodQuality;
            if (blood.BloodQuality <= minBloodQuality)
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{prefabName} has blood quality {blood.BloodQuality} <= {minBloodQuality}");
                // Original uses a curve, implementation might change later.
                var newBloodQuality = UnityEngine.Random.Range(minBloodQuality, maxBloodQuality);
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