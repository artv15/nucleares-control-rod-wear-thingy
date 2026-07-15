using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using HarmonyLib.Tools;

namespace RodsFixer9000;

#pragma warning disable BepInEx002
[BepInPlugin("RodsFixer9000", "Rods Fixer 9000", "1.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        
        HarmonyFileLog.Enabled = true;
        
        Logger.LogInfo("Patching...");
        var hrm = new Harmony("RodFixer.Harmony.Main");
        hrm.PatchAll(typeof(Patches.RodDegradationPatch));
        Logger.LogInfo("Patches applied!");
    }
}
#pragma warning restore BepInEx002