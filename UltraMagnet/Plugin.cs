using UnityEngine;
using BepInEx;
using HarmonyLib;
using PluginConfig;
using System.Linq;
using PluginConfig.API;
using PluginConfig.API.Fields;

namespace UltraMagnet
{
    public static class ConfigManager
    {
        private static PluginConfigurator config;
        public static FloatField radius;

        public static void Setup()
        {
            config = PluginConfigurator.Create("UltraMagnet", "ostia.UltraMagnet");
            radius = new FloatField(config.rootPanel, "Radius", "field.radius", 1000f);
            
        }

    }

    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(PluginConfiguratorController.PLUGIN_GUID)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "ostia.UltraMagnet";
        private const string modName = "UltraMagnet";
        private const string modVersion = "0.0.1";
        private readonly Harmony harmony = new Harmony(modGUID);

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch]
        public static class Magnet_Patch 
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Magnet), "Start")]
            private static void patch_Start(Magnet __instance)
            {
                __instance.GetComponent<SphereCollider>().radius = ConfigManager.radius.value;
            }


        }

    }
}
