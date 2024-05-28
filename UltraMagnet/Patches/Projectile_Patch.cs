using HarmonyLib;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Projectile_Patch
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Projectile), "Start")]
        private static void patch_Start(Projectile __instance)
        {
            if (ConfigManager.projectilePatchPanel.value) { __instance.GetOrAddComponent<MagnetScript>(); }
        }
    }
}
