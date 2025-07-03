﻿using BaboonAPI.Hooks.Initializer;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TootTallyCore;
using TootTallyCore.Utils.Assets;
using TootTallyCore.Utils.TootTallyModules;
using UnityEngine;

namespace TootTallySettings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TootTallyCore", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin, ITootTallyModule
    {
        public static Plugin Instance;

        private static Harmony _harmony;

        public ConfigEntry<bool> ModuleConfigEnabled { get; set; }
        public bool IsConfigInitialized { get; set; }
        public string Name { get => "TootTally Settings"; set => Name = value; }

        public static TootTallySettingPage ModulesSettingPage;
        public static TootTallySettingPage MainTootTallySettingPage;

        public static void LogInfo(string msg) => Instance.Logger.LogInfo(msg);
        public static void LogError(string msg) => Instance.Logger.LogError(msg);

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _harmony = new Harmony(Info.Metadata.GUID);

            GameInitializationEvent.Register(Info, TryInitialize);
        }

        private void TryInitialize()
        {
            // Bind to the TTModules Config for TootTally
            ModuleConfigEnabled = TootTallyCore.Plugin.Instance.Config.Bind("Modules", "TootTallySettings", true, "TootTally Setting Panel for mods");
            MainTootTallySettingPage = TootTallySettingsManager.AddNewPage("TootTally", "TootTally", 40f, new Color(.1f, .1f, .1f, .3f));

            var path = Path.Combine(Paths.BepInExRootPath, "Themes");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var filePaths = Directory.GetFiles(path);
            List<string> fileNames = new List<string>();
            fileNames.AddRange(new string[] { "Day", "Night", "Random", "Default" });
            filePaths.ToList().ForEach(path => fileNames.Add(Path.GetFileNameWithoutExtension(path)));
            MainTootTallySettingPage.AddLabel("GameThemesLabel", "Game Theme", 24f, TMPro.FontStyles.Normal, TMPro.TextAlignmentOptions.BottomLeft);
            MainTootTallySettingPage.AddDropdown("Themes", TootTallyCore.Plugin.Instance.ThemeName, fileNames.ToArray());
            MainTootTallySettingPage.AddButton("ResetThemeButton", new Vector2(350, 50), "Refresh Theme", ThemeManager.RefreshTheme);
            MainTootTallySettingPage.AddToggle("Change pitch speed", TootTallyCore.Plugin.Instance.ChangePitch);
            MainTootTallySettingPage.AddToggle("Run GC While Playing", TootTallyCore.Plugin.Instance.RunGCWhilePlaying);
            MainTootTallySettingPage.AddSlider("Offset At Default Speed", -100, 100, 500, "Offset At Default Speed (ms)", TootTallyCore.Plugin.Instance.OffsetAtDefaultSpeed, true);
            ThemeManager.OnThemeRefreshEvents += TootTallySettingsManager.OnRefreshTheme;
            TootTallyModuleManager.AddModule(this);
        }

        public void LoadModule()
        {
            AssetManager.LoadAssets(Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "Assets"));
            MainTootTallySettingPage.AddImageToPageButton("icon.png");
            TryAddThunderstoreIconToPageButton(Instance.Info.Location, Name, ModulesSettingPage);
            _harmony.PatchAll(typeof(TootTallySettingsManager));
        }

        public void AddModuleToSettingPage(ITootTallyModule module)
        {
            ModulesSettingPage ??= TootTallySettingsManager.AddNewPage("Enable / Disable Modules", "TTModules", 40f, new Color(0, 0, 0, 0));
            ModulesSettingPage.AddToggle(module.Name, module.ModuleConfigEnabled);
        }

        public static void TryAddThunderstoreIconToPageButton(string pluginLocation, string moduleName, TootTallySettingPage page)
        {
            var iconPath = Path.Combine(Path.GetDirectoryName(pluginLocation), "icon.png");
            if (!File.Exists(iconPath))
                iconPath = Path.Combine(Path.GetDirectoryName(pluginLocation), "../icon.png");
            if (File.Exists(iconPath))
                AssetManager.LoadSingleAsset(iconPath, $"{moduleName}.png", delegate { page.AddImageToPageButton($"{moduleName}.png"); });
            else
                Plugin.LogError($"Couldn't find {moduleName} icon image.");
        }

        public void UnloadModule()
        {
            _harmony.UnpatchSelf();
            LogInfo($"Module unloaded!");
        }
    }
}