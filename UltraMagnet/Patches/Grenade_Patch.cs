using HarmonyLib;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Grenade_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Grenade), "Start")]
        private static void patch_Start(Grenade __instance)
        {
            if (ConfigManager.grenadePatchPanel.value && !__instance.rocket) { __instance.GetOrAddComponent<MagnetScript>(); }
        }
    }
}
