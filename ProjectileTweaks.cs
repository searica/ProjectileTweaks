// Ignore Spelling: Plugin Jotunn
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using UnityEngine;
using ProjectileTweaks.Configs;

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
        public const string PluginVersion = "1.0.3";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        //private static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();


        private const string BowSection = "BowTweaks";
        private const string XbowSection = "CrossbowTweaks";
        private const string SpearSection = "SpearTweaks";
        private const string StaffSection = "StaffTweaks";

        internal static ConfigEntry<float> BowSpreadMult { get; set; }
        internal static ConfigEntry<float> BowVelocityMult { get; set; }
        internal static ConfigEntry<float> BowLaunchAngle { get; set; }
        internal static ConfigEntry<float> BowVerticalOffset { get; set; }
        internal static ConfigEntry<float> BowHorizontalOffset { get; set; }

        internal static ConfigEntry<float> XbowSpreadMult { get; set; }
        internal static ConfigEntry<float> XbowVelocityMult { get; set; }
        internal static ConfigEntry<float> XbowLaunchAngle { get; set; }
        internal static ConfigEntry<float> XbowVerticalOffset { get; set; }
        internal static ConfigEntry<float> XbowHorizontalOffset { get; set; }

        internal static ConfigEntry<float> SpearSpreadMult { get; set; }
        internal static ConfigEntry<float> SpearVelocityMult { get; set; }
        internal static ConfigEntry<float> SpearLaunchAngle { get; set; }
        internal static ConfigEntry<float> SpearVerticalOffset { get; set; }
        internal static ConfigEntry<float> SpearHorizontalOffset { get; set; }

        internal static ConfigEntry<float> StaffSpreadMult { get; set; }
        internal static ConfigEntry<float> StaffVelocityMult { get; set; }
        internal static ConfigEntry<float> StaffLaunchAngle { get; set; }
        internal static ConfigEntry<float> StaffVerticalOffset { get; set; }
        internal static ConfigEntry<float> StaffHorizontalOffset { get; set; }


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
            BowSpreadMult = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("SpreadMultiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            BowVelocityMult = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("VelocityMultiplier", 2),
                1f,
                "Multiplies velocity of arrows.",
                new AcceptableValueRange<float>(0.1f, 2f));
            BowLaunchAngle = ConfigManager.BindConfig(
                BowSection,
                ConfigManager.SetStringPriority("LaunchAngle", 1),
                -1f,
                "Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            BowHorizontalOffset = ConfigManager.BindConfig<float>(
                BowSection,
                "HorizontalOffset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));
            BowVerticalOffset = ConfigManager.BindConfig<float>(
                BowSection,
                "VerticalOffset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));

            // Crossbows
            XbowSpreadMult = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("SpreadMultiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            XbowVelocityMult = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("VelocityMultiplier", 2),
                1f,
                "Multiplies velocity of bolts.",
                new AcceptableValueRange<float>(0.1f, 2f));
            XbowLaunchAngle = ConfigManager.BindConfig(
                XbowSection,
                ConfigManager.SetStringPriority("LaunchAngle", 1),
                -1f,
                "Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            XbowHorizontalOffset = ConfigManager.BindConfig(
                XbowSection,
                "HorizontalOffset",
                0f,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));
            XbowVerticalOffset = ConfigManager.BindConfig(
                XbowSection,
                "VerticalOffset",
                0f,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));

            // Spears
            SpearSpreadMult = ConfigManager.BindConfig(
                SpearSection,
                ConfigManager.SetStringPriority("SpreadMultiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero your spear throws will have zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            SpearVelocityMult = ConfigManager.BindConfig(
                SpearSection,
                ConfigManager.SetStringPriority("VelocityMultiplier", 2),
                1f,
                "Multiplies velocity of thrown spears.",
                new AcceptableValueRange<float>(0.1f, 2f));
            SpearLaunchAngle = ConfigManager.BindConfig(
                SpearSection,
                "LaunchAngle",
                -1f,
                "Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5f, 5f));
            SpearHorizontalOffset = ConfigManager.BindConfig(
                SpearSection,
                "HorizontalOffset",
                0.1f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));
            SpearVerticalOffset = ConfigManager.BindConfig(
                SpearSection,
                "VerticalOffset",
                0.5f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));

            // Staffs
            StaffSpreadMult = ConfigManager.BindConfig(
                StaffSection,
                ConfigManager.SetStringPriority("SpreadMultiplier", 3),
                1f,
                "Multiplies the min and max projectile spread, so if you set it to zero there will be zero spread.",
                new AcceptableValueRange<float>(0f, 2f));
            StaffVelocityMult = ConfigManager.BindConfig(
                StaffSection,
                ConfigManager.SetStringPriority("VelocityMultiplier", 2),
                1f,
                "Multiplies velocity of projectiles.",
                new AcceptableValueRange<float>(0.1f, 2f));
            StaffHorizontalOffset = ConfigManager.BindConfig(
                StaffSection,
                "HorizontalOffset",
                0f,
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));
            StaffVerticalOffset = ConfigManager.BindConfig(
                StaffSection,
                "VerticalOffset",
                0.3f,
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f));
        }
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

    internal static ConfigEntry<LogLevel> Verbosity { get; set; }
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
