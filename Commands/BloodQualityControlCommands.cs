using VampireCommandFramework;

namespace BloodQualityControl.Commands
{
    [CommandGroup("bloodquality")]
    public class BloodQualityControlCommand
    {
        [Command(".min", usage: "<BloodQualityValue>", description: "Set's the minimum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 5f)
        {
            var maxBloodQuality = PluginServices.BloodQualityControlService.MaxBloodQuality;
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(ctx.Reply, minBloodQuality: bloodQuality, maxBloodQuality: maxBloodQuality);
        }

        [Command(".max", usage: "<BloodQualityValue>", description: "Set's the maximum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMaxBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 100f)
        {
            var minBloodQuality = PluginServices.BloodQualityControlService.MinBloodQuality;
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(ctx.Reply, minBloodQuality: minBloodQuality, maxBloodQuality: bloodQuality);
        }

        [Command(".range", usage: "<MinBloodQuality> <MaxBloodQuality>", description: "Set's both the minimum and maximum blood quality a mob unit can spawn with. Min and Max must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinAndMaxBloodQualityCommand(ChatCommandContext ctx, float minBloodQuality = 5f, float maxBloodQuality = 100f)
        {
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(ctx.Reply, minBloodQuality: minBloodQuality, maxBloodQuality: maxBloodQuality);
        }

        [Command(".disable", description: "Returns the blood quality settings to the defaults.", adminOnly: true)]
        public static void DisableCommand(ChatCommandContext ctx)
        {
            PluginServices.BloodQualityControlService.Disable();
            ctx.Reply($"Blood Quality settings have been restored to defaults");
        }
    }
}