using HarmonyLib;
using UnityEngine;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Breakable_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Breakable), "Start")]
        private static void patch_Start(Breakable __instance)
        {
            Harpoon harpoon = __instance.GetComponentInParent<Harpoon>() as Harpoon;
            if (harpoon && !harpoon.drill)
            {
                __instance.gameObject.SetActive(!ConfigManager.unbreakable.value);
            }
        }
    }
}
