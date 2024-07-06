using UnityEngine;
using Comfort.Common;
using EFT;
using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System.IO;
using System.Reflection;

namespace FIKA_HEADLESS_PLAYER
{
    [BepInPlugin("ekky.fika_headless_player", "Fika Headless Player", "1.0.0")]
    public class FIKA_HEADLESS_PLAYER : BaseUnityPlugin
    {
        // Mod
        private bool camerasEnabled = false;
        private bool autoCameraDisable = true;

        // EFT
        public static GameWorld gameWorld;
        public static Player myPlayer;

        // BepInEx
        public static string PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static ConfigEntry<KeyboardShortcut> ToggleCamera;
        public static ConfigEntry<bool> AutoKill;
        public static GameObject Hook;


        void Awake()
        {
            Logger.LogInfo("FIKA_HEADLESS_PLAYER :::: INFO :::: Mod Loaded");
            ToggleCamera = Config.Bind("Main", "Toggle Camera", new KeyboardShortcut(KeyCode.F10), "Keybind to toggle main player camera.");
            AutoKill = Config.Bind<bool>("Main", "Auto Kill Player", false, "Automatically kills player once another player joins.");
            Hook = new GameObject();
        }


        void Update() 
        {

            // IF MAP NOT LOADED, RETURN
            if (!MapLoaded())
                return;

            gameWorld = Singleton<GameWorld>.Instance;
            myPlayer = gameWorld?.MainPlayer;

            // IF IN MENU or HIDEOUT, RETURN
            if (gameWorld == null || myPlayer == null || gameWorld.LocationId == "hideout")
            {
                return;
            }

            if (AutoKill.Value && myPlayer.HealthController.IsAlive) 
            {
                myPlayer.KillMe(EBodyPartColliderType.HeadCommon, 1000);
            }

            if (autoCameraDisable) {
                ToggleGameWorldRendering(false);
                autoCameraDisable = false;
            }

            if (Input.GetKey(ToggleCamera.Value.MainKey))
            {
                camerasEnabled = !camerasEnabled;
                ToggleGameWorldRendering(camerasEnabled);
            }

        }
        public static bool MapLoaded() => Singleton<GameWorld>.Instantiated;

        private void ToggleGameWorldRendering(bool enable)
        {
            if (myPlayer != null)
            {

                GameObject cameraContainer = myPlayer.CameraContainer;
                if (cameraContainer != null)
                {
                    Camera[] cameras = cameraContainer.GetComponentsInChildren<Camera>();
                    foreach (var camera in cameras)
                    {
                        camera.enabled = enable;
                        Logger.LogInfo($"FIKA_HEADLESS_PLAYER :::: INFO :::: Camera {camera.name} {(enable ? "enabled" : "disabled")}.");
                    }
                }
                else
                {
                    Logger.LogWarning("FIKA_HEADLESS_PLAYER :::: WARNING :::: CameraContainer not found within myPlayer.");
                }
            }
            else
            {
                Logger.LogWarning("FIKA_HEADLESS_PLAYER :::: WARNING :::: myPlayer object not found.");
            }
        }

    }
}

