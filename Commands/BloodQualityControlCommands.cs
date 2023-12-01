using VampireCommandFramework;

namespace BloodQualityControl.Commands
{
    [CommandGroup("bloodquality")]
    public class BloodQualityControlCommand
    {
        [Command(".min", usage: "<BloodQualityValue>", description: "Set's the minimum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 5f)
        {
            PluginServices.BloodQualityControlService.HookReplyCallbacks(
                ctx.Reply,
                ctx.Reply
            );
            var maxBloodQuality = PluginServices.BloodQualityControlService.MaxBloodQuality;
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(minBloodQuality: bloodQuality, maxBloodQuality: maxBloodQuality);
        }

        [Command(".max", usage: "<BloodQualityValue>", description: "Set's the maximum blood quality a mob unit can spawn with. Must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMaxBloodQualityCommand(ChatCommandContext ctx, float bloodQuality = 100f)
        {
            PluginServices.BloodQualityControlService.HookReplyCallbacks(
                ctx.Reply,
                ctx.Reply
            );
            var minBloodQuality = PluginServices.BloodQualityControlService.MinBloodQuality;
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(minBloodQuality: minBloodQuality, maxBloodQuality: bloodQuality);
        }

        [Command(".range", usage: "<MinBloodQuality> <MaxBloodQuality>", description: "Set's both the minimum and maximum blood quality a mob unit can spawn with. Min and Max must be a value between 5-100", adminOnly: true)]
        public static void ConfigureMinAndMaxBloodQualityCommand(ChatCommandContext ctx, float minBloodQuality = 5f, float maxBloodQuality = 100f)
        {
            PluginServices.BloodQualityControlService.HookReplyCallbacks(
                ctx.Reply,
                ctx.Reply
            );
            PluginServices.BloodQualityControlService.OverrideBloodQualitySettings(minBloodQuality: minBloodQuality, maxBloodQuality: maxBloodQuality);
        }

        [Command(".disable", description: "Returns the blood quality settings to the defaults.")]
        public static void DisableCommand(ChatCommandContext ctx)
        {
            PluginServices.BloodQualityControlService.Disable();
            PluginServices.BloodQualityControlService.UnhookReplyCallbacks();
            ctx.Reply($"Blood Quality settings have been restored to defaults");
        }
    }
}