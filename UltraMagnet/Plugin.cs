using BepInEx;
using HarmonyLib;
using PluginConfig;

namespace UltraMagnet
{
    /// <summary>
    /// Main plugin class for UltraMagnet.
    /// </summary>
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(PluginConfiguratorController.PLUGIN_GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "ostia.UltraMagnet";
        public const string PLUGIN_NAME = "UltraMagnet";
        public const string PLUGIN_VERSION = "0.0.2";

        private static Harmony harmony;

        private void Awake()
        {
            ConfigManager.Init();

            harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}
