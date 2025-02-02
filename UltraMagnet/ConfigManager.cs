using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System.IO;
using System.Reflection;

namespace UltraMagnet
{
    public static class ConfigManager
    {
        public enum MagnetType
        {
            Rocket,
            Chainsaw
        }

        public static PluginConfigurator config;

        // Magnet settings
        public static FloatField radius;
        public static FloatField spinning;
        public static BoolField unbreakable;
        //public static EnumField<MagnetType> magnetType;
        public static BoolField magnetPatchPanel;

        // Patch toggles
        public static BoolField cannonballPatchPanel;
        public static BoolField coinPatchPanel;
        public static BoolField projectilePatchPanel;
        public static BoolField harpoonPatchPanel;
        public static BoolField rocketPatchPanel;
        public static BoolField grenadePatchPanel;

        /// <summary>
        /// Initializes the configuration manager and sets up all configuration fields
        /// </summary>
        public static void Init()
        {
            config = PluginConfigurator.Create(Plugin.PLUGIN_NAME, Plugin.PLUGIN_GUID);
            SetupIcon();
            SetupPatchToggles();
            SetupMagnetSettings();
        }

        private static void SetupIcon()
        {
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconPath = Path.Combine(pluginPath, "icon.png");
            config.SetIconWithURL("file://" + iconPath);
        }

        private static void SetupPatchToggles()
        {
            new ConfigHeader(config.rootPanel, "Patches");
            cannonballPatchPanel = new BoolField(config.rootPanel, "Cannonball", "cannonballPatchPanel", true);
            coinPatchPanel = new BoolField(config.rootPanel, "Coin", "coinPatchPanel", true);
            projectilePatchPanel = new BoolField(config.rootPanel, "Projectile", "projectilePatchPanel", true);
            harpoonPatchPanel = new BoolField(config.rootPanel, "Harpoon (Require restart)", "harpoonPatchPanel", true);
            grenadePatchPanel = new BoolField(config.rootPanel, "Grenade", "grenadePatchPanel", true);
            rocketPatchPanel = new BoolField(config.rootPanel, "Rocket (Require restart)", "rocketPatchPanel", true);
        }

        private static void SetupMagnetSettings()
        {
            new ConfigHeader(config.rootPanel, "Magnet");
            //magnetType = new EnumField<MagnetType>(config.rootPanel, "Magnet type", "field_guid", MagnetType.Rocket);
            magnetPatchPanel = new BoolField(config.rootPanel, "Magnet", "magnetPatchPanel", true);
            unbreakable = new BoolField(config.rootPanel, "Unbreakable", "unbreakable", true);
            radius = new FloatField(config.rootPanel, "Radius", "radius", 1000f, 0f, float.MaxValue);
            spinning = new FloatField(config.rootPanel, "Spinning", "spinning", 80f, 0f, 85f);
        }
    }
}
