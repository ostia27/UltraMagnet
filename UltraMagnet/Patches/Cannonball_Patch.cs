using HarmonyLib;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Cannonball_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Cannonball), "Start")]
        private static void patch_Start(Cannonball __instance)
        {
            if (ConfigManager.cannonballPatchPanel.value) { __instance.GetOrAddComponent<MagnetScript>(); }
        }
    }
}
