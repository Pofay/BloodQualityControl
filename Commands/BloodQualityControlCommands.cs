using ProjectM.Network;
using VampireCommandFramework;

namespace BloodQualityControl.Commands
{
    [CommandGroup("bloodquality")]
    public class BloodQualityControlCommand
    {
        [Command(".min", usage: "<BloodQualityValue>", description: "Set's the minimum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 5f)
        {
            if (bloodQuality >= 5f && bloodQuality <= 100f)
            {
                PluginServices.BloodQualityControlService.MinBloodQuality = bloodQuality;
                ctx.Reply(PluginServices.BloodQualityControlService.GetFormattedSettings());
            }
            else
            {
                ctx.Error($"The given blood quality is lower than 5");
            }
        }

        [Command(".max", usage: "<BloodQualityValue>", description: "Set's the maximum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMaxBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 100f)
        {
            if (bloodQuality >= 5f && bloodQuality <= 100f)
            {
                PluginServices.BloodQualityControlService.MaxBloodQuality = bloodQuality;
                ctx.Reply(PluginServices.BloodQualityControlService.GetFormattedSettings());
            }
            else
            {
                ctx.Error($"The given blood quality is higher than 100 or lower than 5");
            }
        }

        [Command(".set", usage: "<MinBloodQuality> <MaxBloodQuality>", description: "Set's the both the minimum and maximum blood quality a mob unit can spawn with. Min and Max must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinAndMaxBloodQualityCommand(ChatCommandContext ctx, float minQuality = 5f, float maxQuality = 100f)
        {
            if (!(minQuality >= 5f && minQuality <= 100f))
            {
                ctx.Error($"Min blood quality should be a value between 5-100.");
            }
            else if (!(maxQuality >= 5f && maxQuality <= 100f))
            {
                ctx.Error($"Max blood quality should be a value between 5-100.");
            }
            else
            {
                PluginServices.BloodQualityControlService.MinBloodQuality = minQuality;
                PluginServices.BloodQualityControlService.MaxBloodQuality = maxQuality;
                ctx.Reply(PluginServices.BloodQualityControlService.GetFormattedSettings());
            }
        }

        [Command(".disable", description: "Returns the blood quality settings to the defaults.")]
        public static void DisableCommand(ChatCommandContext ctx)
        {
            PluginServices.BloodQualityControlService.Disable();
            ctx.Reply($"Blood Quality settings have been restored to defaults");
        }
    }
}