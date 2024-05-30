using BepInEx;
using HarmonyLib;
using PluginConfig;

namespace UltraMagnet
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(PluginConfiguratorController.PLUGIN_GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "ostia.UltraMagnet";
        public const string PLUGIN_NAME = "UltraMagnet";
        public const string PLUGIN_VERSION = "0.0.11";

        private static Harmony harmony = new Harmony(PLUGIN_GUID);

        void Awake()
        {
            ConfigManager.Init();

            harmony.PatchAll();
        }
    }
}
