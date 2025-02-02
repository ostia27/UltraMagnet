using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace UltraMagnet
{
    [HarmonyPatch]
    public static class Harpoon_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Harpoon), "Start")]
        private static void patch_Start(Harpoon __instance)
        {
            if (ConfigManager.magnetPatchPanel.value && !__instance.drill) 
            {
                __instance.GetOrAddComponent<DirectMagnetScript>();
                return;
            }
            if (ConfigManager.harpoonPatchPanel.value) { __instance.GetOrAddComponent<DirectMagnetScript>(); }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Harpoon), "Update")]
        private static IEnumerable<CodeInstruction> Transpiler_Update(IEnumerable<CodeInstruction> instructions)
        {
            // Skip if patch is disabled
            if (!ConfigManager.harpoonPatchPanel.value)
            {
                Debug.Log("[UltraMagnet] Harpoon Update transpiler skipped - patch disabled");
                return instructions;
            }

            Debug.Log("[UltraMagnet] Starting Harpoon Update transpiler");
            var codes = new List<CodeInstruction>(instructions);
            
            // Find the start of the velocity check sequence (first stopped field check)
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldarg_0 &&
                    i + 1 < codes.Count &&
                    codes[i + 1].LoadsField(AccessTools.Field(typeof(Harpoon), "stopped")))
                {
                    Debug.Log("[UltraMagnet] Found stopped field check at index " + i);
                    
                    // Find the end of the sequence (the ret instruction after LookAt)
                    int endIndex = i;
                    for (int j = i; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Ret && j < codes.Count - 1)
                        {
                            endIndex = j;
                            Debug.Log("[UltraMagnet] Found end ret instruction at index " + j);
                            break;
                        }
                    }

                    if (endIndex > i)
                    {
                        Debug.Log("[UltraMagnet] Replacing instruction sequence from index " + i + " to " + endIndex);
                        // Replace the first instruction with a jump to after the ret
                        codes[i] = new CodeInstruction(OpCodes.Br, codes[endIndex + 1].labels[0]);
                        
                        // NOP out the rest of the instructions in between
                        for (int k = i + 1; k <= endIndex; k++)
                        {
                            codes[k] = new CodeInstruction(OpCodes.Nop);
                        }
                        break;
                    }
                }
            }

            Debug.Log("[UltraMagnet] Completed Harpoon Update transpiler");
            return codes.AsEnumerable();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Harpoon), "OnTriggerEnter")]
        private static IEnumerable<CodeInstruction> Transpiler_OnTriggerEnter(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);

            /* 
             * We want to insert the following code after the IL sequence that checks:
             *     if (!this.stopped && (other.gameObject.layer == 8 || other.gameObject.layer == 24))
             *
             * The IL equivalent of what we need is:
             *
             * DirectMagnetScript component = this.GetComponent<DirectMagnetScript>();
             * if (component != null)
             *     Object.Destroy(component);
             */

            // Declare a local variable of type DirectMagnetScript for storing the result of GetComponent<T>()
            var directMagnetLocal = generator.DeclareLocal(typeof(DirectMagnetScript));

            // Build the injection instructions.
            var inject = new List<CodeInstruction>();

            // 1. Load 'this'
            inject.Add(new CodeInstruction(OpCodes.Ldarg_0));
            // 2. Call GetComponent<DirectMagnetScript>()
            MethodInfo getComponentMethod = AccessTools.Method(typeof(Component), "GetComponent", new Type[0]);
            getComponentMethod = getComponentMethod.MakeGenericMethod(typeof(DirectMagnetScript));
            inject.Add(new CodeInstruction(OpCodes.Callvirt, getComponentMethod));
            // 3. Store the result in our local
            inject.Add(new CodeInstruction(OpCodes.Stloc, directMagnetLocal));
            // 4. Load the local variable onto the stack to check for null
            inject.Add(new CodeInstruction(OpCodes.Ldloc, directMagnetLocal));
            // 5. Define a label to skip the destroy call if the local is null
            var skipLabel = generator.DefineLabel();
            inject.Add(new CodeInstruction(OpCodes.Brfalse_S, skipLabel));
            // 6. Load the component again from the local
            inject.Add(new CodeInstruction(OpCodes.Ldloc, directMagnetLocal));
            // 7. Call Object.Destroy(component)
            MethodInfo destroyMethod = AccessTools.Method(typeof(UnityEngine.Object), "Destroy", new Type[] { typeof(UnityEngine.Object) });
            inject.Add(new CodeInstruction(OpCodes.Call, destroyMethod));
            // 8. Define the skip point
            CodeInstruction nopInstruction = new CodeInstruction(OpCodes.Nop);
            nopInstruction.labels.Add(skipLabel);
            inject.Add(nopInstruction);

            // Now we have to choose the point of injection. We look for the pattern where the constant 24 is loaded immediately 
            // before a Bne_Un opcode in the code that checks the layer.
            int insertIndex = -1;
            for (int i = 0; i < codes.Count - 1; i++)
            {
                if ((codes[i].opcode == OpCodes.Ldc_I4_S || codes[i].opcode == OpCodes.Ldc_I4) &&
                     codes[i].operand != null &&
                     Convert.ToInt32(codes[i].operand) == 24 &&
                     codes[i + 1].opcode == OpCodes.Bne_Un)
                {
                    // Insert our injection code after the branch instruction.
                    insertIndex = i + 2;
                    break;
                }
            }

            if (insertIndex != -1)
            {
                codes.InsertRange(insertIndex, inject);
                Debug.Log("[UltraMagnet] Injected DirectMagnetScript removal code into OnTriggerEnter transpiler");
            }
            else
            {
                Debug.LogError("[UltraMagnet] Failed to find injection point in Harpoon.OnTriggerEnter");
            }

            return codes.AsEnumerable();
        }
    }
}
