using HarmonyLib;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Breakable_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Breakable), "Start")]
        private static void patch_Start(Breakable __instance)
        {
            if (__instance.GetComponentInParent<Harpoon>())
            {
                __instance.gameObject.SetActive(!ConfigManager.unbreakable.value);
            }
        }
    }
}
