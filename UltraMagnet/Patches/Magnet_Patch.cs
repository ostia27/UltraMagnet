using HarmonyLib;
using UnityEngine;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Magnet_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Magnet), "Start")]
        private static void patch_Start(Magnet __instance)
        {
            __instance.GetComponent<SphereCollider>().radius = ConfigManager.radius.value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Magnet), "OnTriggerEnter")]
        private static void patch_OnTriggerEnter(Magnet __instance, ref Collider other)
        {
            MagnetScript magnetScript;
            if (other.TryGetComponent<MagnetScript>(out magnetScript))
            {
                if (!magnetScript.magnets.Contains(__instance))
                {
                    magnetScript.magnets.Add(__instance);
                }
            }
        }

    }
}
