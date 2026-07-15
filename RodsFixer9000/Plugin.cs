using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;

namespace RodsFixer9000;

#pragma warning disable BepInEx002
[BepInPlugin("RodsFixer9000", "Rods Fixer 9000", "0.0.0-Dev")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        
        Logger.LogInfo("Patching...");
        var hrm = new Harmony("RodFixer.Harmony.Main");
        hrm.PatchAll();
        Logger.LogInfo("Patches applied!");
    }
}
#pragma warning restore BepInEx002