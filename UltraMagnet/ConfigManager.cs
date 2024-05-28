using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System.IO;
using System.Reflection;

namespace UltraMagnet
{
    public static class ConfigManager
    {
        public static PluginConfigurator config;
        public static FloatField radius;
        public static FloatField hz;

        public static BoolField cannonballPatchPanel;
        public static BoolField coinPatchPanel;
        public static BoolField projectilePatchPanel;
        public static BoolField grenadePatchPanel;

        public static BoolField unbreakable;

        public static void Init()
        {
            config = PluginConfigurator.Create(Plugin.PLUGIN_NAME, Plugin.PLUGIN_GUID);
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconPath = Path.Combine(pluginPath, "icon.png");
            config.SetIconWithURL("file://" + iconPath);

            new ConfigHeader(config.rootPanel, "Patches");
            cannonballPatchPanel = new BoolField(config.rootPanel, "Cannonball", "cannonballPatchPanel", true);
            coinPatchPanel = new BoolField(config.rootPanel, "Coin", "coinPatchPanel", true);
            projectilePatchPanel = new BoolField(config.rootPanel, "Projectile", "projectilePatchPanel", true);
            grenadePatchPanel = new BoolField(config.rootPanel, "Grenade", "grenadePatchPanel", true);

            new ConfigHeader(config.rootPanel, "Magnet");
            unbreakable = new BoolField(config.rootPanel, "Unbreakable", "unbreakable", true);
            radius = new FloatField(config.rootPanel, "Radius", "radius", 1000f);
            hz = new FloatField(config.rootPanel, "hz", "field.hz", 80, 0, 85);

        }
    }
}
