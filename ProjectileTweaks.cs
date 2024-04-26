// Ignore Spelling: Plugin Jotunn
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using UnityEngine;
using ProjectileTweaks.Configs;
using System;
using Jotunn.Managers;

namespace ProjectileTweaks
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    internal class ProjectileTweaks : BaseUnityPlugin
    {
        public const string Author = "Searica";
        public const string PluginName = "ProjectileTweaks";
        public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "1.3.0";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        //private static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private const string BombSection = "BombTweaks";
        private const string BowSection = "BowTweaks";
        private const string XbowSection = "CrossbowTweaks";
        private const string SpearSection = "SpearTweaks";
        private const string StaffSection = "StaffTweaks";
        private const string ZoomSection = "Zoom";

        private static bool ShouldSaveConfig = false;

        internal static ConfigEntry<float> BowSpreadMult { get; private set; }
        internal static ConfigEntry<float> BowVelocityMult { get; private set; }
        internal static ConfigEntry<float> BowLaunchAngle { get; private set; }
        internal static ConfigEntry<float> BowVerticalOffset { get; private set; }
        internal static ConfigEntry<float> BowHorizontalOffset { get; private set; }
        internal static ConfigEntry<float> BowDrawSpeed { get; private set; }

        internal static ConfigEntry<float> BombSpreadMult { get; private set; }
        internal static ConfigEntry<float> BombVelocityMult { get; private set; }
        internal static ConfigEntry<float> BombLaunchAngle { get; private set; }
        internal static ConfigEntry<float> BombVerticalOffset { get; private set; }
        internal static ConfigEntry<float> BombHorizontalOffset { get; private set; }

        internal static ConfigEntry<float> XbowSpreadMult { get; private set; }
        internal static ConfigEntry<float> XbowVelocityMult { get; private set; }
        internal static ConfigEntry<float> XbowLaunchAngle { get; private set; }
        internal static ConfigEntry<float> XbowVerticalOffset { get; private set; }
        internal static ConfigEntry<float> XbowHorizontalOffset { get; private set; }
        internal static ConfigEntry<float> XBowReloadSpeed { get; private set; }

        internal static ConfigEntry<float> SpearSpreadMult { get; private set; }
        internal static ConfigEntry<float> SpearVelocityMult { get; private set; }
        internal static ConfigEntry<float> SpearLaunchAngle { get; private set; }
        internal static ConfigEntry<float> SpearVerticalOffset { get; private set; }
        internal static ConfigEntry<float> SpearHorizontalOffset { get; private set; }

        internal static ConfigEntry<float> StaffSpreadMult { get; private set; }
        internal static ConfigEntry<float> StaffVelocityMult { get; private set; }
        internal static ConfigEntry<float> StaffLaunchAngle { get; private set; }
        internal static ConfigEntry<float> StaffVerticalOffset { get; private set; }
        internal static ConfigEntry<float> StaffHorizontalOffset { get; private set; }

        internal static ConfigEntry<bool> EnableBowZoom { get; private set; }

        internal static ConfigEntry<bool> EnableXbowZoom { get; private set; }
        internal static ConfigEntry<KeyCode> ZoomKey { get; private set; }
        internal static ConfigEntry<KeyCode> CancelDrawKey { get; private set; }
        internal static ConfigEntry<float> StayInZoomTime { get; private set; }
        internal static ConfigEntry<float> TimeToZoomIn { get; private set; }
        internal static ConfigEntry<float> ZoomFactor { get; private set; }

        internal static ConfigEntry<bool> AutoBowZoom { get; private set; }

        /// <summary>
        ///     Bool indicating if zooming is enabled for bow || crossbow
        /// </summary>
        internal static bool IsZoomEnabled => EnableBowZoom.Value || EnableXbowZoom.Value;

        /// <summary>
        ///     Called by Unity
        /// </summary>
        public void Awake()
        {
            Log.Init(this.Logger);

            ConfigManager.Init(PluginGUID, Config, false);
            Initialize();
            ConfigManager.Save();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

            Game.isModded = true;

            ConfigManager.SetupWatcher();

            SynchronizationManager.OnConfigurationWindowClosed += () =>
            {
                UpdateConfigFile();
            };

            SynchronizationManager.OnConfigurationSynchronized += (sender, e) =>
            {
                UpdateConfigFile();
            };
        }

        /// <summary>
        ///     Called by Unity
        /// </summary>
        public void OnDestroy()
        {
            ConfigManager.Save();
        }

        private void Initialize()
        {
            // Bows
            BowDrawSpeed = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("Draw Speed Multiplier", 4),
                1f,
                "Multiplier for draw speed of bows. Set to 2 to draw bows twice as fast. Does not affect Vanilla scaling with skill level.",
                new AcceptableValueRange<float>(0.5f, 2f)
            );
            BowDrawSpeed.SettingChanged += UpdateSettings;

            BowSpreadMult = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("Spread Multiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            BowSpreadMult.SettingChanged += UpdateSettings;

            BowVelocityMult = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("Velocity Multiplier", 2),
                1f,
                "Multiplies velocity of arrows.",
                new AcceptableValueRange<float>(0.1f, 2f));
            BowVelocityMult.SettingChanged += UpdateSettings;

            BowLaunchAngle = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("Launch Angle", 1),
                -1f,
                "Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            BowLaunchAngle.SettingChanged += UpdateSettings;

            BowHorizontalOffset = ConfigManager.BindConfig(
                BowSection,
                "Horizontal Offset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            BowHorizontalOffset.SettingChanged += UpdateSettings;

            BowVerticalOffset = ConfigManager.BindConfig(
                BowSection,
                "Vertical Offset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            BowVerticalOffset.SettingChanged += UpdateSettings;

            // Bombs
            BombSpreadMult = ConfigManager.BindConfig(
                BombSection,
                ConfigManager.SetStringPriority("Spread Multiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your bombs will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            BombSpreadMult.SettingChanged += UpdateSettings;

            BombVelocityMult = ConfigManager.BindConfig(
                BombSection,
                ConfigManager.SetStringPriority("Velocity Multiplier", 2),
                1f,
                "Multiplies velocity of bombs.",
                new AcceptableValueRange<float>(0.1f, 2f));
            BombVelocityMult.SettingChanged += UpdateSettings;

            BombLaunchAngle = ConfigManager.BindConfig(
                BombSection,
                ConfigManager.SetStringPriority("Launch Angle", 1),
                -1f,
                "Changes the launch angle for bombs. Vanilla default for bombs is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            BombLaunchAngle.SettingChanged += UpdateSettings;

            BombHorizontalOffset = ConfigManager.BindConfig(
                BombSection,
                "Horizontal Offset",
                0.0f,
                "Offsets the location that bombs are thrown from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            BombHorizontalOffset.SettingChanged += UpdateSettings;

            BombVerticalOffset = ConfigManager.BindConfig(
                BombSection,
                "Vertical Offset",
                0.5f,
                "Offsets the location that bombs are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            BombVerticalOffset.SettingChanged += UpdateSettings;

            // Crossbows
            XBowReloadSpeed = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("Reload Speed Multiplier", 4),
                1f,
                "Multiplier for reload speed of crossbows. Set to 2 to reload twice as fast. Does not affect Vanilla scaling with skill level.",
                new AcceptableValueRange<float>(0.5f, 2f)
            );
            XBowReloadSpeed.SettingChanged += UpdateSettings;

            XbowSpreadMult = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("Spread Multiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            XbowSpreadMult.SettingChanged += UpdateSettings;

            XbowVelocityMult = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("Velocity Multiplier", 2),
                1f,
                "Multiplies velocity of bolts.",
                new AcceptableValueRange<float>(0.1f, 2f));
            XbowVelocityMult.SettingChanged += UpdateSettings;

            XbowLaunchAngle = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("Launch Angle", 1),
                -1f,
                "Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            XbowLaunchAngle.SettingChanged += UpdateSettings;

            XbowHorizontalOffset = ConfigManager.BindConfig(
                XbowSection,
                "Horizontal Offset",
                0f,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            XbowHorizontalOffset.SettingChanged += UpdateSettings;

            XbowVerticalOffset = ConfigManager.BindConfig(
                XbowSection,
                "Vertical Offset",
                0f,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            XbowVerticalOffset.SettingChanged += UpdateSettings;

            // Spears
            SpearSpreadMult = ConfigManager.BindConfig(
                SpearSection,
                ConfigManager.SetStringPriority("Spread Multiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your spear throws will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            SpearSpreadMult.SettingChanged += UpdateSettings;

            SpearVelocityMult = ConfigManager.BindConfig(
                SpearSection,
                ConfigManager.SetStringPriority("Velocity Multiplier", 2),
                1f,
                "Multiplies velocity of thrown spears.",
                new AcceptableValueRange<float>(0.1f, 2f));
            SpearVelocityMult.SettingChanged += UpdateSettings;

            SpearLaunchAngle = ConfigManager.BindConfig(
                SpearSection,
                "Launch Angle",
                -1f,
                "Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            SpearLaunchAngle.SettingChanged += UpdateSettings;

            SpearHorizontalOffset = ConfigManager.BindConfig(
                SpearSection,
                "Horizontal Offset",
                0.1f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            SpearHorizontalOffset.SettingChanged += UpdateSettings;

            SpearVerticalOffset = ConfigManager.BindConfig(
                SpearSection,
                "Vertical Offset",
                0.5f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            SpearVelocityMult.SettingChanged += UpdateSettings;

            // Staffs
            StaffSpreadMult = ConfigManager.BindConfig(
                StaffSection,
                ConfigManager.SetStringPriority("Spread Multiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero there will be zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            StaffSpreadMult.SettingChanged += UpdateSettings;

            StaffVelocityMult = ConfigManager.BindConfig(
                StaffSection,
                ConfigManager.SetStringPriority("Velocity Multiplier", 2),
                1f,
                "Multiplies velocity of projectiles.",
                new AcceptableValueRange<float>(0.1f, 2f));
            StaffVelocityMult.SettingChanged += UpdateSettings;

            StaffHorizontalOffset = ConfigManager.BindConfig(
                StaffSection,
                "Horizontal Offset",
                0f,
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            StaffHorizontalOffset.SettingChanged += UpdateSettings;

            StaffVerticalOffset = ConfigManager.BindConfig(
                StaffSection,
                "Vertical Offset",
                0.3f,
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.75f, 0.75f));
            StaffVerticalOffset.SettingChanged += UpdateSettings;

            // Zoom section
            EnableBowZoom = ConfigManager.BindConfig(
                ZoomSection,
                ConfigManager.SetStringPriority("Bow Zoom", 3),
                true,
                "Set to true/enabled to allow zooming while using a bow.");
            EnableBowZoom.SettingChanged += UpdateSettings;

            EnableXbowZoom = ConfigManager.BindConfig(
                ZoomSection,
                ConfigManager.SetStringPriority("Crossbow Zoom", 3),
                true,
                "Set to true/enabled to allow zooming while using a crossbow.");
            EnableXbowZoom.SettingChanged += UpdateSettings;

            ZoomKey = ConfigManager.BindConfig(
                ZoomSection,
                ConfigManager.SetStringPriority("Zoom Key", 2),
                KeyCode.Mouse1,
                "Set the key used to zoom in while using a bow or crossbow.",
                synced: false);
            ZoomKey.SettingChanged += UpdateSettings;

            CancelDrawKey = ConfigManager.BindConfig(
                ZoomSection,
                ConfigManager.SetStringPriority("Cancel Draw Key", 1),
                KeyCode.E,
                "Set the key used to cancel drawing your bow.",
                synced: false);
            CancelDrawKey.SettingChanged += UpdateSettings;

            TimeToZoomIn = ConfigManager.BindConfig(
                ZoomSection,
                "Time to Zoom in",
                1f,
                "Time that it takes to zoom in all the way. '1' is default and recommended.",
                new AcceptableValueRange<float>(0.2f, 2f),
                synced: false);
            TimeToZoomIn.SettingChanged += UpdateSettings;

            StayInZoomTime = ConfigManager.BindConfig(
                ZoomSection,
                "Stay In-Zoom Time",
                2f,
                "Set the maximum time the camera will stay zoomed in while holding the zoom key after firing a projectile.",
                new AcceptableValueRange<float>(0.5f, 4f),
                synced: false);
            StayInZoomTime.SettingChanged += UpdateSettings;

            ZoomFactor = ConfigManager.BindConfig(
                ZoomSection,
                "Zoom Factor",
                2f,
                "Set how much too zoom in relative to current camera view.",
                new AcceptableValueRange<float>(1f, 4f),
                synced: false);
            ZoomFactor.SettingChanged += UpdateSettings;

            AutoBowZoom = ConfigManager.BindConfig(
                ZoomSection,
                "Auto Bow Zoom",
                false,
                "Set to true/enabled to make bows automatically zoom in as they are drawn.",
                synced: false);
            AutoBowZoom.SettingChanged += UpdateSettings;
        }

        private static void UpdateConfigFile()
        {
            if (ShouldSaveConfig)
            {
                ConfigManager.Save();
                ShouldSaveConfig = false;
            }
        }

        private static void UpdateSettings(object obj, EventArgs e)
        {
            ShouldSaveConfig |= !ShouldSaveConfig;
        }
    }

    /// <summary>
    ///     Log level to control output to BepInEx log
    /// </summary>
    internal enum LogLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    /// <summary>
    ///     Helper class for properly logging from static contexts.
    /// </summary>
    internal static class Log
    {
        private const BindingFlags AllBindings =
               BindingFlags.Public
               | BindingFlags.NonPublic
               | BindingFlags.Instance
               | BindingFlags.Static
               | BindingFlags.GetField
               | BindingFlags.SetField
               | BindingFlags.GetProperty
               | BindingFlags.SetProperty;

        #region Verbosity

        internal static ConfigEntry<LogLevel> Verbosity { get; private set; }
        internal static LogLevel VerbosityLevel => Verbosity.Value;
        internal static bool IsVerbosityLow => Verbosity.Value >= LogLevel.Low;
        internal static bool IsVerbosityMedium => Verbosity.Value >= LogLevel.Medium;
        internal static bool IsVerbosityHigh => Verbosity.Value >= LogLevel.High;

        #endregion Verbosity

        private static ManualLogSource logSource;

        internal static void Init(ManualLogSource logSource)
        {
            Log.logSource = logSource;
        }

        internal static void LogDebug(object data) => logSource.LogDebug(data);

        internal static void LogError(object data) => logSource.LogError(data);

        internal static void LogFatal(object data) => logSource.LogFatal(data);

        internal static void LogMessage(object data) => logSource.LogMessage(data);

        internal static void LogWarning(object data) => logSource.LogWarning(data);

        internal static void LogInfo(object data, LogLevel level = LogLevel.Low)
        {
            if (Verbosity is null || VerbosityLevel >= level)
            {
                logSource.LogInfo(data);
            }
        }

        internal static void LogGameObject(GameObject prefab, bool includeChildren = false)
        {
            LogInfo("***** " + prefab.name + " *****");
            foreach (Component compo in prefab.GetComponents<Component>())
            {
                LogComponent(compo);
            }

            if (!includeChildren) { return; }

            LogInfo("***** " + prefab.name + " (children) *****");
            foreach (Transform child in prefab.transform)
            {
                LogInfo($" - {child.gameObject.name}");
                foreach (Component compo in child.gameObject.GetComponents<Component>())
                {
                    LogComponent(compo);
                }
            }
        }

        internal static void LogComponent(Component compo)
        {
            LogInfo($"--- {compo.GetType().Name}: {compo.name} ---");

            PropertyInfo[] properties = compo.GetType().GetProperties(AllBindings);
            foreach (var property in properties)
            {
                LogInfo($" - {property.Name} = {property.GetValue(compo)}");
            }

            FieldInfo[] fields = compo.GetType().GetFields(AllBindings);
            foreach (var field in fields)
            {
                LogInfo($" - {field.Name} = {field.GetValue(compo)}");
            }
        }
    }
}
