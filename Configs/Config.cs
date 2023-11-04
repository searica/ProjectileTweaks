// Ignore Spelling: Jotunn

using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Managers;
using ProjectileTweaks.Logging;
using System;

using System.IO;
using System.Reflection;

namespace ProjectileTweaks.Configs
{
    internal class Config
    {
        private static BaseUnityPlugin configurationManager;

        private static readonly string ConfigFileName = ProjectileTweaks.PluginGUID + ".cfg";

        private static readonly string ConfigFileFullPath = string.Concat(
            Paths.ConfigPath,
            Path.DirectorySeparatorChar,
            ConfigFileName
        );

        private static ConfigFile configFile;

        private static readonly ConfigurationManagerAttributes AdminConfig = new() { IsAdminOnly = true };
        private static readonly ConfigurationManagerAttributes ClientConfig = new() { IsAdminOnly = false };

        internal enum LoggerLevel
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }

        internal static LoggerLevel VerbosityLevel => Verbosity.Value;
        internal static bool IsVerbosityLow => Verbosity.Value >= LoggerLevel.Low;
        internal static bool IsVerbosityMedium => Verbosity.Value >= LoggerLevel.Medium;
        internal static bool IsVerbosityHigh => Verbosity.Value >= LoggerLevel.High;

        private const string MainSectionName = "\u200BGlobal";
        private const string BowSection = "BowTweaks";
        private const string XbowSection = "CrossbowTweaks";
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

        internal static ConfigEntry<float> StaffSpreadMult { get; set; }
        internal static ConfigEntry<float> StaffVelocityMult { get; set; }
        internal static ConfigEntry<float> StaffLaunchAngle { get; set; }
        internal static ConfigEntry<float> StaffVerticalOffset { get; set; }
        internal static ConfigEntry<float> StaffHorizontalOffset { get; set; }

        internal static ConfigEntry<LoggerLevel> Verbosity { get; private set; }

        private static readonly AcceptableValueList<bool> AcceptableBoolValuesList = new(new bool[] { false, true });

        internal static ConfigEntry<T> BindConfig<T>(
            string section,
            string name,
            T value,
            string description,
            AcceptableValueBase acceptVals = null,
            bool synced = true
        )
        {
            string extendedDescription = GetExtendedDescription(description, synced);
            ConfigEntry<T> configEntry = configFile.Bind(
                section,
                name,
                value,
                new ConfigDescription(
                    extendedDescription,
                    acceptVals,
                    synced ? AdminConfig : ClientConfig
                )
            );
            return configEntry;
        }

        internal static string GetExtendedDescription(
            string description,
            bool synchronizedSetting
        )
        {
            return description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]");
        }

        internal static void Init(ConfigFile config)
        {
            configFile = config;
            configFile.SaveOnConfigSet = false;
        }

        internal static void Save()
        {
            configFile.Save();
        }

        internal static void SaveOnConfigSet(bool value)
        {
            configFile.SaveOnConfigSet = value;
        }

        internal static void SetUpConfig()
        {
            // Bows
            BowSpreadMult = BindConfig<float>(
                BowSection,
                "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            BowVelocityMult = BindConfig<float>(
                BowSection,
                "VelocityMultiplier",
                1,
                "Multiplies velocity of arrows.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            BowLaunchAngle = BindConfig<float>(
                BowSection,
                "LaunchAngle",
                -1,
                "Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5, 5)
            );
            BowHorizontalOffset = BindConfig<float>(
                BowSection,
                "HorizontalOffset",
                0,
                "Offsets the location that arrows are spawned before firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            BowVerticalOffset = BindConfig<float>(
                BowSection,
                "VerticalOffset",
                0,
                "Offsets the location that arrows are spawned before firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            // Crossbows
            XbowSpreadMult = BindConfig<float>(
                XbowSection,
                "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            XbowVelocityMult = BindConfig<float>(
                XbowSection,
                "VelocityMultiplier",
                1,
                "Multiplies velocity of bolts.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            XbowLaunchAngle = BindConfig<float>(
                XbowSection,
                "LaunchAngle",
                -1,
                "Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5, 5)
            );
            XbowHorizontalOffset = BindConfig<float>(
                XbowSection,
                "HorizontalOffset",
                0,
                "Offsets the location that bolts are spawned before firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            XbowVerticalOffset = BindConfig<float>(
                XbowSection,
                "VerticalOffset",
                0,
                "Offsets the location that bolts are spawned before firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            // Staffs
            StaffSpreadMult = BindConfig<float>(
                StaffSection,
                "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero there will be zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            StaffVelocityMult = BindConfig<float>(
                StaffSection,
                "VelocityMultiplier",
                1,
                "Multiplies velocity of projectiles.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            //StaffLaunchAngle = BindConfig<float>(
            //    StaffSection,
            //    "LaunchAngle",
            //    -1,
            //    "Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.",
            //    new AcceptableValueRange<float>(-5, 5)
            //);
            StaffHorizontalOffset = BindConfig<float>(
                StaffSection,
                "HorizontalOffset",
                0,
                "Offsets the location that projectiles are spawned before firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            StaffVerticalOffset = BindConfig<float>(
                StaffSection,
                "VerticalOffset",
                0,
                "Offsets the location that projectiles are spawned before firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            Save();
        }

        internal static void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReloadConfigFile;
            watcher.Created += ReloadConfigFile;
            watcher.Renamed += ReloadConfigFile;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReloadConfigFile(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Log.LogInfo("Reloading config file");

                // turn off saving on config entry set
                var saveOnConfigSet = configFile.SaveOnConfigSet;
                configFile.SaveOnConfigSet = false;

                configFile.Reload();

                // reset config saving state
                configFile.SaveOnConfigSet = saveOnConfigSet;
            }
            catch
            {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
            // Do whatever else I might need to
        }

        /// <summary>
        ///
        /// </summary>
        internal static void CheckForConfigManager()
        {
            if (GUIManager.IsHeadless())
            {
                return;
            }

            if (
                Chainloader.PluginInfos.TryGetValue(
                    "com.bepis.bepinex.configurationmanager",
                    out PluginInfo configManagerInfo
                )
                && configManagerInfo.Instance
            )
            {
                configurationManager = configManagerInfo.Instance;
                Log.LogDebug("Configuration manager found, hooking DisplayingWindowChanged");

                EventInfo eventinfo = configurationManager.GetType()
                    .GetEvent("DisplayingWindowChanged");

                if (eventinfo != null)
                {
                    Action<object, object> local = new(OnConfigManagerDisplayingWindowChanged);
                    Delegate converted = Delegate.CreateDelegate(
                        eventinfo.EventHandlerType,
                        local.Target,
                        local.Method
                    );
                    eventinfo.AddEventHandler(configurationManager, converted);
                }
            }
        }

        private static void OnConfigManagerDisplayingWindowChanged(object sender, object e)
        {
            //Jotunn.Logger.LogDebug("OnConfigManagerDisplayingWindowChanged received.");
            PropertyInfo pi = configurationManager.GetType().GetProperty("DisplayingWindow");
            bool cmActive = (bool)pi.GetValue(configurationManager, null);

            if (!cmActive)
            {
                // do whatever I need to do when the cfg file reloads
            }
        }
    }
}