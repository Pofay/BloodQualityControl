using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BloodQualityControl.Constants;
using BloodQualityControl.utils;

namespace BloodQualityControl.Config
{
    public class PluginConfig
    {
        private static ConfigFile mainConfig;

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<float> MinBloodQuality { get; private set; }
        public static ConfigEntry<float> MaxBloodQuality { get; private set; }

        public static void Initialize()
        {
            var bepInExConfigFolder = BepInEx.Paths.ConfigPath ?? Path.Combine("BepInEx", "config");
            var configFolder = Path.Combine(bepInExConfigFolder, "BloodQualityControl");
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var mainConfigFilePath = Path.Combine(configFolder, "BloodQualityControl.cfg");
            mainConfig = File.Exists(mainConfigFilePath) ? new ConfigFile(mainConfigFilePath, false) : new ConfigFile(mainConfigFilePath, true);
            Enabled = mainConfig.Bind("Main", "Enabled", false, "Determines whether the mod is enabled or not.");
            MinBloodQuality = mainConfig.Bind("Main", "MinimumBloodQuality", QualityConstants.MIN_BLOOD_QUALITY, "The minimum blood quality that units will spawn with. Should be a value between 5-100 and must not be higher than the MaxBloodQuality.");
            MaxBloodQuality = mainConfig.Bind("Main", "MaximumBloodQuality", QualityConstants.MAX_BLOOD_QUALITY, "The maximum blood quality that units will spawn with. Should be a value between 5-100 and must not be lower than the MinBloodQuality.");
            ValidateBloodQualityValues();
        }

        private static void ValidateBloodQualityValues()
        {
            var validationMessage = Validations.ValidateBloodQuality(MinBloodQuality.Value, MaxBloodQuality.Value);
            if (validationMessage != string.Empty)
            {
                PluginServices.Logger.LogInfo(validationMessage);
                MinBloodQuality.Value = 5f;
                MaxBloodQuality.Value = 100f;
                Save();
            }
        }

        public static void Save()
        {
            mainConfig.Save();
        }

        public static void Destroy()
        {
            mainConfig.Clear();
        }

    }
}