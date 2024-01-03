using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;
using ILLGames.Unity.Component;

namespace HC_LogoRemover
{
    [BepInProcess("HoneyCome")]
    [BepInProcess("HoneyComeccp")]
    //[BepInProcess("DigitalCraft")]
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
            //Config entry and update logo if settings changed
            disableWaterMark = Config.Bind("General", "Disable watermark", true, "");
            disableWaterMark.SettingChanged += (sender, args) => LogoRemoverComponent.SetLogo();
            //Patch methods
            Harmony.CreateAndPatchAll(typeof(LogoRemoverComponent.Hooks), GUID);
        }

        public class LogoRemoverComponent
        {
            //Instance and original logo
            private static ScreenshotHandlerURP screenshotHandler;
            private static UnityEngine.Texture2D originalLogo;

            public static void SetLogo()
            {
                //If disableWaterMark, set texture to null, else restore original
                if (disableWaterMark.Value && screenshotHandler != null)
                    screenshotHandler.texCapMark = null;
                else if (!disableWaterMark.Value && screenshotHandler != null)
                    screenshotHandler.texCapMark = originalLogo;
            }

            public static class Hooks
            {
                [HarmonyPrefix]
                [HarmonyPatch(typeof(ScreenshotHandlerURP), "OnEnable")]
                public static void ScreenShotOnEnableHook(ScreenshotHandlerURP __instance)
                {
                    //Get instance and save original, then set logo
                    screenshotHandler = __instance;
                    originalLogo = screenshotHandler.texCapMark;
                    SetLogo();
                }
            }
        }
    }
}
