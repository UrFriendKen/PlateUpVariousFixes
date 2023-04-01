using HarmonyLib;
using Kitchen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace KitchenVariousFixes.Patches
{
    [HarmonyPatch(typeof(ApplyItemProcesses), "Run")]
    static class ApplyItemProcesses_Patch
    {
        static readonly List<OpCode> OPCODES_TO_MATCH = new List<OpCode>()
        {
            OpCodes.Ldloc_S,
            OpCodes.Ldfld,
            OpCodes.Ldc_I4_0,
            OpCodes.Ceq,
            OpCodes.Br
        };

        static readonly List<object> OPERANDS_TO_MATCH = new List<object>()
        {
            null,
            typeof(CSplittableItem).GetField("AllowMergeSplit", BindingFlags.Public | BindingFlags.Instance),
            null,
            null,
            null
        };

        static readonly List<OpCode> MODIFIED_OPCODES = new List<OpCode>()
        {
            //OpCodes.Ldloc_S,
            //OpCodes.Ldfld,
            //OpCodes.Ldc_I4,
            //OpCodes.Ceq,
            //OpCodes.Br_S
        };

        static readonly List<object> MODIFIED_OPERANDS = new List<object>()
        {
            null,
            typeof(CSplittableItem).GetField("PreventExplicitSplit", BindingFlags.Public | BindingFlags.Instance)
        };

        const int EXPECTED_MATCH_COUNT = 1;

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Main.LogInfo("ApplyItemProcesses Transpiler");
            Main.LogInfo("Attempt change AllowMergeSplit to PreventExplicitSplit");
            List<CodeInstruction> list = instructions.ToList();

            int matches = 0;
            int windowSize = OPCODES_TO_MATCH.Count;
            for (int i = 0; i < list.Count - windowSize; i++)
            {
                for (int j = 0; j < windowSize; j++)
                {
                    if (OPCODES_TO_MATCH[j] == null)
                    {
                        Main.LogError("OPCODES_TO_MATCH cannot contain null!");
                        return instructions;
                    }

                    string logLine = $"{j}:\t{OPCODES_TO_MATCH[j]}";

                    int index = i + j;
                    OpCode opCode = list[index].opcode;
                    if (j < OPCODES_TO_MATCH.Count && opCode != OPCODES_TO_MATCH[j])
                    {
                        if (j > 0)
                        {
                            logLine += $" != {opCode}";
                            Main.LogInfo($"{logLine}\tFAIL");
                        }
                        break;
                    }
                    logLine += $" == {opCode}";

                    if (j == 0)
                        Debug.Log("-------------------------");

                    if (j < OPERANDS_TO_MATCH.Count && OPERANDS_TO_MATCH[j] != null)
                    {
                        logLine += $"\t{OPERANDS_TO_MATCH[j]}";
                        object operand = list[index].operand;
                        if (OPERANDS_TO_MATCH[j] != operand)
                        {
                            logLine += $" != {operand}";
                            Main.LogInfo($"{logLine}\tFAIL");
                            break;
                        }
                        logLine += $" == {operand}";
                    }
                    Main.LogInfo($"{logLine}\tPASS");

                    if (j == OPCODES_TO_MATCH.Count - 1)
                    {
                        Main.LogInfo($"Found match {++matches}");
                        if (matches > EXPECTED_MATCH_COUNT)
                        {
                            Main.LogError("Number of matches found exceeded EXPECTED_MATCH_COUNT! Returning original IL.");
                            return instructions;
                        }

                        // Perform replacements
                        for (int k = 0; k < MODIFIED_OPCODES.Count; k++)
                        {
                            if (MODIFIED_OPCODES[k] != null)
                            {
                                int replacementIndex = i + k;
                                OpCode beforeChange = list[replacementIndex].opcode;
                                list[replacementIndex].opcode = MODIFIED_OPCODES[k];
                                Main.LogInfo($"Line {replacementIndex}: Replaced Opcode ({beforeChange} ==> {MODIFIED_OPCODES[k]})");
                            }
                        }

                        for (int k = 0; k < MODIFIED_OPERANDS.Count; k++)
                        {
                            if (MODIFIED_OPERANDS[k] != null)
                            {
                                int replacementIndex = i + k;
                                object beforeChange = list[replacementIndex].operand;
                                list[replacementIndex].operand = MODIFIED_OPERANDS[k];
                                Main.LogInfo($"Line {replacementIndex}: Replaced operand ({beforeChange ?? "null"} ==> {MODIFIED_OPERANDS[k] ?? "null"})");
                            }
                        }

                        //for (int k = 0; k < 5; k++)
                        //{
                        //    object operand = list[i + k].operand;
                        //    if (operand == null)
                        //    {
                        //        Main.LogInfo("null");
                        //        continue;
                        //    }
                        //    if (operand is Label label)
                        //    {
                        //        Main.LogInfo(label.GetType().GetField("label", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(label));
                        //        continue;
                        //    }
                        //    Main.LogInfo(operand ?? "null");
                        //}
                    }
                }
            }

            Main.LogWarning($"{(matches > 0 ? (matches == EXPECTED_MATCH_COUNT ? "Transpiler Patch succeeded with no errors" : $"Completed with {matches}/{EXPECTED_MATCH_COUNT} found.") : "Failed to find match")}");
            return list.AsEnumerable();
        }
    }






    [HarmonyPatch(typeof(AcceptIntoHolder), "<OnUpdate>b__0_0")]
    static class AcceptIntoHolder_Patch
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Main.LogInfo("AcceptIntoHolder OnUpdate");
            List<CodeInstruction> list = instructions.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                Main.LogInfo($"{list[i].opcode}\t{list[i].operand ?? ""}");

            }

            return list.AsEnumerable();
        }
    }
}
