using HarmonyLib;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Coin_Patch
    {
        static MagnetScript script;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Coin), "Start")]
        private static void patch_Start(Coin __instance)
        {
            if (ConfigManager.coinPatchPanel.value) { script = __instance.GetOrAddComponent<MagnetScript>(); }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Coin), "Update")]
        private static void patch_Update(Coin __instance)
        {
            if (script.isOnMagnet)
            {
                __instance.CancelInvoke("GetDeleted");
            }
            else
            {
                __instance.Invoke("GetDeleted", 5f);
            }
        }


    }
}
