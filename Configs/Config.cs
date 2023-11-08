// Ignore Spelling: Jotunn

using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace ProjectileTweaks.Configs
{
    internal class Config
    {
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

        internal static ConfigEntry<LoggerLevel> Verbosity { get; private set; }

        private const char ZWS = '\u200B';

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

        /// <summary>
        ///     Sets SaveOnConfigSet to false and returns
        ///     the value prior to calling this method.
        /// </summary>
        /// <returns></returns>
        private static bool DisableSaveOnConfigSet()
        {
            var val = configFile.SaveOnConfigSet;
            configFile.SaveOnConfigSet = false;
            return val;
        }

        internal static void SetUpConfig()
        {
            // Bows
            BowSpreadMult = BindConfig<float>(
                BowSection,
                new string(ZWS, 3) + "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            BowVelocityMult = BindConfig<float>(
                BowSection,
                new string(ZWS, 2) + "VelocityMultiplier",
                1,
                "Multiplies velocity of arrows.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            BowLaunchAngle = BindConfig<float>(
                BowSection,
                ZWS + "LaunchAngle",
                -1,
                "Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5, 5)
            );
            BowHorizontalOffset = BindConfig<float>(
                BowSection,
                "HorizontalOffset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            BowVerticalOffset = BindConfig<float>(
                BowSection,
                "VerticalOffset",
                0.2f,
                "Offsets the location that arrows are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            // Crossbows
            XbowSpreadMult = BindConfig<float>(
                XbowSection,
                new string(ZWS, 3) + "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            XbowVelocityMult = BindConfig<float>(
                XbowSection,
                new string(ZWS, 2) + "VelocityMultiplier",
                1,
                "Multiplies velocity of bolts.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            XbowLaunchAngle = BindConfig<float>(
                XbowSection,
                ZWS + "LaunchAngle",
                -1,
                "Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5, 5)
            );
            XbowHorizontalOffset = BindConfig<float>(
                XbowSection,
                "HorizontalOffset",
                0,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            XbowVerticalOffset = BindConfig<float>(
                XbowSection,
                "VerticalOffset",
                0,
                "Offsets the location that bolts are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            // Spears
            SpearSpreadMult = BindConfig<float>(
                SpearSection,
                new string(ZWS, 3) + "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero your spear throws will have zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            SpearVelocityMult = BindConfig<float>(
                SpearSection,
                new string(ZWS, 2) + "VelocityMultiplier",
                1,
                "Multiplies velocity of thrown spears.",
                new AcceptableValueRange<float>(0.1f, 2)
            );
            SpearLaunchAngle = BindConfig<float>(
                SpearSection,
                "LaunchAngle",
                -1,
                "Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.",
                new AcceptableValueRange<float>(-5, 5)
            );
            SpearHorizontalOffset = BindConfig<float>(
                SpearSection,
                "HorizontalOffset",
                0.1f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            SpearVerticalOffset = BindConfig<float>(
                SpearSection,
                "VerticalOffset",
                0.5f,
                "Offsets the location that thrown spears are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );

            // Staffs
            StaffSpreadMult = BindConfig<float>(
                StaffSection,
                new string(ZWS, 3) + "SpreadMultiplier",
                1,
                "Multiplies the min and max projectile spread, so if you set it to zero there will be zero spread.",
                new AcceptableValueRange<float>(0, 2)
            );
            StaffVelocityMult = BindConfig<float>(
                StaffSection,
                new string(ZWS, 2) + "VelocityMultiplier",
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
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                new AcceptableValueRange<float>(-0.5f, 0.5f)
            );
            StaffVerticalOffset = BindConfig<float>(
                StaffSection,
                "VerticalOffset",
                0.3f,
                "Offsets the location that projectiles are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.",
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
                var saveOnConfigSet = DisableSaveOnConfigSet();

                configFile.Reload();

                // reset config saving state to previous state
                configFile.SaveOnConfigSet = saveOnConfigSet;
            }
            catch
            {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
        }
    }
}