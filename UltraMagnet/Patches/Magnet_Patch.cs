using GameConsole.pcon;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            DirectMagnetScript directMagnetScript;
            if (other.TryGetComponent<MagnetScript>(out magnetScript))
            {
                if (!magnetScript.magnets.Contains(__instance))
                {
                    magnetScript.magnets.Add(__instance);
                }
            }
            if (other.TryGetComponent<DirectMagnetScript>(out directMagnetScript))
            {
                if (!directMagnetScript.magnets.Contains(__instance))
                {
                    directMagnetScript.magnets.Add(__instance);
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Magnet), "OnTriggerEnter")]
        private static IEnumerable<CodeInstruction> Transpiler_OnTriggerEnter(IEnumerable<CodeInstruction> instructions)
        {
            if (!ConfigManager.rocketPatchPanel.value)
            {
                return instructions; // Return original instructions if disabled
            }

            Debug.Log("[UltraMagnet] Starting OnTriggerEnter transpiler");
            var codes = new List<CodeInstruction>(instructions);
            
            // Find the start of the onEnemy check sequence
            for (int i = 0; i < codes.Count; i++)
            {
                // Look for the first ldarg.0 followed by the onEnemy field load
                if (codes[i].opcode == OpCodes.Ldarg_0 &&
                    i + 1 < codes.Count &&
                    codes[i + 1].LoadsField(AccessTools.Field(typeof(Magnet), "onEnemy")))
                {
                    Debug.Log("[UltraMagnet] Found onEnemy check sequence at index " + i);
                    
                    // Find the end of the onEnemy check sequence (the grenade.enemy check)
                    int endIndex = i;
                    for (int j = i; j < codes.Count; j++)
                    {
                        if (codes[j].LoadsField(AccessTools.Field(typeof(Grenade), "enemy")))
                        {
                            endIndex = j;
                            Debug.Log("[UltraMagnet] Found grenade.enemy check at index " + j);
                            break;
                        }
                    }

                    if (endIndex > i)
                    {
                        Debug.Log("[UltraMagnet] Replacing instruction sequence from index " + i + " to " + endIndex);
                        // Replace the entire sequence with just loading true
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4_1); // Load constant 1 (true)
                        
                        // Remove all instructions between
                        for (int k = i + 1; k <= endIndex; k++)
                        {
                            codes[k] = new CodeInstruction(OpCodes.Nop);
                        }
                        break;
                    }
                }
            }

            Debug.Log("[UltraMagnet] Completed OnTriggerEnter transpiler");
            return codes.AsEnumerable();
        }
    }
}
