using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace RodsFixer9000.Patches;

public class RodDegradationPatch
{
    [HarmonyPatch(typeof(BarraDeControl), "RegistrarAbsorcion")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> WearAndTearNegate(IEnumerable<CodeInstruction> instructions)
    {
        var originalInstructions = instructions.ToList(); // multiple enumeration prevent
        var matcher = new CodeMatcher(originalInstructions);

        Type controlRodType = typeof(BarraDeControl); // nucleares naming all symbols in spanish moment 🥀💔

        FieldInfo cyclesAbsorptionField = AccessTools.Field(controlRodType, "_ciclosAbsorvidos");

        CodeMatch[] matcherTarget = new[]
        {
            new CodeMatch(OpCodes.Ldarg_0),                       // [this]
            new CodeMatch(OpCodes.Ldarg_0),                       // [this, this]
            new CodeMatch(OpCodes.Ldfld, cyclesAbsorptionField),  // [this, <currentAbsorptions>]
            new CodeMatch(OpCodes.Ldc_R4, 1f)             // [this, <currentAbsorptions>, 1f] -- then there's add and a store to the field, where the stack is finally cleared blah blah blah I'm changing only ldc anyway
        };
        
        matcher.MatchForward(true, matcherTarget);
        if (!matcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching for rod wear failed!");
            return originalInstructions;
        }
        
        matcher.SetInstruction(new CodeInstruction(OpCodes.Ldc_R4, 0f)); // [this, <currentAbsorptions>, 0f], suppress cycle increase

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(typeof(BarraDeControl), "GetCapacidadAbsorcion")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> WearEffectNegate(IEnumerable<CodeInstruction> instructions)
    {
        var originalInstructions = instructions.ToList(); // multiple enumeration prevent
        var matcher = new CodeMatcher(originalInstructions);

        Type controlRodType = typeof(BarraDeControl); // do I need to say it again?

        FieldInfo cyclesAbsorptionField = AccessTools.Field(controlRodType, "_ciclosAbsorvidos");
        FieldInfo cyclesAbsorptionMaxField = AccessTools.Field(controlRodType, "CiclosDeAbsorcion");

        CodeMatch[] matcherTarget = new[]
        {
            new CodeMatch(OpCodes.Ldc_R4, 1f),               // [1f]
            new CodeMatch(OpCodes.Ldarg_0),                          // [1f, this]
            new CodeMatch(OpCodes.Ldfld, cyclesAbsorptionField),     // [1f, <currentAbsorptions>]
            new CodeMatch(OpCodes.Ldarg_0),                          // [1f, <currentAbsorptions>, this]
            new CodeMatch(OpCodes.Ldfld, cyclesAbsorptionMaxField),  // [1f, <currentAbsorptions>, <maxAbsorptions>]
            new CodeMatch(OpCodes.Div),                              // [1f, (<currentAbsorptions>/<maxAbsorptions>)]
            new CodeMatch(OpCodes.Sub),                              // [(whatever the result of the substraction is)]
            new CodeMatch(OpCodes.Stloc_0)                           // [], pop the stack to local var 0
        };

        matcher.MatchForward(false, matcherTarget);
        if (!matcher.IsValid)
        {
            Plugin.Logger.LogError("IL matching for rod efficiency failed!");
            return originalInstructions;
        }
        
        matcher.Advance(1); // keep the LoadConstant_Float 1f
        matcher.RemoveInstructions(6); // delete all but pop to local 0

        return matcher.InstructionEnumeration();
    }
}