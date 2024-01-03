using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;
using DigitalCraft;

namespace HC_LogoRemover
{
    //[BepInProcess("HoneyCome")]
    //[BepInProcess("HoneyComeccp")]
    [BepInProcess("DigitalCraft")]
    [BepInPlugin(GUID, PluginName, PluginVersion)]
    public class LogoRemover : BasePlugin
    {
        //Plugin info
        public const string GUID = "HC_LogoRemover";
        public const string PluginName = "HC_LogoRemover";
        public const string PluginVersion = "1.0.0";
        //Variables
        public static ConfigEntry<bool> disableWaterMark;

        public override void Load()
        {
            //Config and patch methods
            disableWaterMark = Config.Bind("General", "Disable watermark", true, "");
            Harmony.CreateAndPatchAll(typeof(LogoRemoverComponent.Hooks), GUID);
        }

        public class LogoRemoverComponent
        {
            //Instances
            private static GameScreenShotURP screenshotHandlerDC;
            //Variables
            private static bool takeScreenShot;

            public static class Hooks
            {
                [HarmonyPostfix]
                [HarmonyPatch(typeof(GameScreenShotURP), "OnEnable")]
                public static void ScreenShotOnEnableHook(GameScreenShotURP __instance)
                {
                    //Get instance
                    screenshotHandlerDC = __instance;
                }

                [HarmonyPrefix]
                [HarmonyPatch(typeof(GameScreenShotURP), "Capture")]
                public static bool CaptureDCHook(bool __runOriginal)
                {
                    //If logo disabled and first call, skip original method and call method again with logo arg disabled
                    if (disableWaterMark.Value && takeScreenShot == false)
                    {
                        takeScreenShot = true;
                        screenshotHandlerDC.Capture(logo: false);
                        return __runOriginal = false;
                    }
                    //If logo disabled and second call, do called method with new args
                    else if (disableWaterMark.Value && takeScreenShot == true)
                    {
                        takeScreenShot = false;
                        return __runOriginal = true;
                    }
                    //If logo enabled run original method
                    return __runOriginal = true;
                }
            }
        }
    }
}
