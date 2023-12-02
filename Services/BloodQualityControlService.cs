using System;
using BloodQualityControl.Config;
using BloodQualityControl.Constants;
using HarmonyLib;
using ProjectM;
using ProjectM.Shared.Systems;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

namespace BloodQualityControl.Services
{
    public class BloodQualityControlService
    {
        public float MinBloodQuality { get; private set; }

        public float MaxBloodQuality { get; private set; }

        public void Initialize()
        {
            if (!PluginConfig.Enabled.Value)
            {
                PluginServices.Logger.LogInfo($"Mod is currently disabled, to enable please execute any command from this mod with the appropriate values.");
            }
            MinBloodQuality = PluginConfig.MinBloodQuality.Value;
            MaxBloodQuality = PluginConfig.MaxBloodQuality.Value;
            BloodQualitySpawnSystem_Patch.Enabled = PluginConfig.Enabled.Value;
        }

        public void OverrideBloodQualitySettings(Action<string> ReplyCallback, float minBloodQuality, float maxBloodQuality)
        {
            if (!(QualityConstants.MIN_BLOOD_QUALITY <= minBloodQuality && minBloodQuality <= QualityConstants.MAX_BLOOD_QUALITY))
            {
                ReplyCallback($"The given Minimum Blood Quality {minBloodQuality} is not in the range of 5-100");
                return;
            }

            if (!(QualityConstants.MIN_BLOOD_QUALITY <= maxBloodQuality && maxBloodQuality <= QualityConstants.MAX_BLOOD_QUALITY))
            {
                ReplyCallback($"The given Maximum Blood Quality {maxBloodQuality} is not in the range of 5-100");
                return;
            }

            if (minBloodQuality > maxBloodQuality)
            {
                ReplyCallback($"The given Minimum Blood Quality {minBloodQuality} is higher than the given Max Blood Quality {maxBloodQuality}");
                return;
            }
            else
            {
                MinBloodQuality = minBloodQuality;
                MaxBloodQuality = maxBloodQuality;
                BloodQualitySpawnSystem_Patch.Enabled = true;
                PersistConfiguration();
                ReplyCallback(GetFormattedSettings());
                ReplyCallback("Persisted new values in config file.");
            }
        }

        private void PersistConfiguration()
        {
            PluginConfig.MinBloodQuality.Value = MinBloodQuality;
            PluginConfig.MaxBloodQuality.Value = MaxBloodQuality;
            PluginConfig.Enabled.Value = BloodQualitySpawnSystem_Patch.Enabled;
            PluginConfig.Save();
        }

        public void Disable()
        {
            BloodQualitySpawnSystem_Patch.Enabled = false;
        }

        private string GetFormattedSettings()
        {
            return $"\nMIN BLOOD QUALITY: {MinBloodQuality}\nMAX BLOOD QUALITY: {MaxBloodQuality}\nBLOOD QUALITY RANGE: {MinBloodQuality}-{MaxBloodQuality}";
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
                        OverrideBloodQualityCalculation(entity, blood);
                    }
                }
            }
        }

        private static void OverrideBloodQualityCalculation(Entity entity, BloodConsumeSource blood)
        {
            var minBloodQuality = PluginServices.BloodQualityControlService.MinBloodQuality;
            var maxBloodQuality = PluginServices.BloodQualityControlService.MaxBloodQuality;
            var characterName = PluginServices.GetCharacterNameFromPrefabGUID(PluginServices.GetPrefabGUID(entity));
            PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {characterName} has initially spawned with blood quality of {blood.BloodQuality} ");
            if (blood.BloodQuality <= minBloodQuality)
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: Rerolling {entity}_{characterName} with blood quality between {minBloodQuality}-{maxBloodQuality}");
                // Original uses a curve, implementation might change later.
                var newBloodQuality = UnityEngine.Random.Range(minBloodQuality, maxBloodQuality);
                blood.BloodQuality = newBloodQuality;
                PluginServices.ServerEntityManager.SetComponentData(entity, blood);
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{characterName} has rolled with a new blood quality of {newBloodQuality}");
            }
            else
            {
                PluginServices.Logger.LogInfo($"{nameof(BloodQualitySpawnSystem)}_Postfix: {entity}_{characterName} blood quality is unchanged since its equal or above the minimum blood quality of {minBloodQuality}");
            }
        }
    }
}