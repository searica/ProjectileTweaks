// Ignore Spelling: Plugin Jotunn
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Utils;
using UnityEngine;
using System;
using Jotunn.Managers;
using Configs;
using Logging;


namespace ProjectileTweaks;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
[NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
[SynchronizationMode(AdminOnlyStrictness.IfOnServer)]
internal class ProjectileTweaks : BaseUnityPlugin
{
    public const string Author = "Searica";
    public const string PluginName = "ProjectileTweaks";
    public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
    public const string PluginVersion = "1.5.2";

    // Use this class to add your own localization to the game
    // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
    //private static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

    public static ProjectileTweaks Instance;

    private const string BombSection = "Bomb Tweaks";
    private const string BowSection = "Bow Tweaks";
    private const string XbowSection = "Crossbow Tweaks";
    private const string SpearSection = "Spear Tweaks";
    private const string StaffSection = "Staff Tweaks";
    private const string ZoomSection = "Zoom";

    private static bool ShouldSaveConfig = false;
    
    internal class ProjectileConfig
    {
        public string SectionName { get; private set; }
        public string ProjectileType { get; private set; }
        public float DefaultSpread { get; private set; }
        public float DefaultVelocity { get; private set; }
        public float DefaultLaunchAngle { get; private set; } 
        public float DefaultLoadSpeed { get; private set; }
        public float DefaultVerticalOffset { get; private set; } 
        public float DefaultHorizontalOffset { get; private set; }
        
        public ConfigEntry<float> SpreadMult { get; private set; }
        public ConfigEntry<float> VelocityMult { get; private set; }
        public ConfigEntry<float> LaunchAngle { get; private set; }
        public ConfigEntry<float> LoadSpeed { get; private set; }
        public ConfigEntry<float> VerticalOffset { get; private set; }
        public ConfigEntry<float> HorizontalOffset { get; private set; }

        /// <summary>
        ///     Creates and binds config settings based on provided default values. If a default is NaN
        ///     then skip binding that config.
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="sectionName"></param>
        /// <param name="projectileType"></param>
        /// <param name="defaultSpread"></param>
        /// <param name="defaultVelocity"></param>
        /// <param name="defaultLaunchAngle"></param>
        /// <param name="defaultVanillaLaunchAngle"></param>
        /// <param name="defaultLoadSpeed"></param>
        /// <param name="defaultVerticalOffset"></param>
        /// <param name="defaultHorizontalOffset"></param>
        public ProjectileConfig(
            ConfigFile configFile,
            string sectionName,
            float defaultSpread = 1f, 
            float defaultVelocity = 1f, 
            float defaultLaunchAngle = -1f,
            float defaultVanillaLaunchAngle = float.NaN,
            float defaultLoadSpeed = 1f,
            float defaultVerticalOffset = float.NaN,
            float defaultHorizontalOffset = float.NaN
        )
        {
            SectionName = sectionName;
            if (defaultLoadSpeed != float.NaN)
            {
                LoadSpeed = configFile.BindConfigInOrder(
                    sectionName,
                    "Load/Draw Speed Multiplier",
                    defaultLoadSpeed,
                    "Multiplier for speed of loading projectiles or drawing bows. Set to 2 to load/draw twice as fast. Does not affect Vanilla scaling with skill level.",
                    acceptableValues: new AcceptableValueRange<float>(0.5f, 2f)
                );
                LoadSpeed.SettingChanged += UpdateSettings;
            }

            if (defaultSpread != float.NaN)
            {
                SpreadMult = configFile.BindConfigInOrder(
                    sectionName,
                    "Spread Multiplier",
                    defaultSpread,
                    "Multiplies the min and max projectile spread, so if you set it to zero your projectiles will have zero spread.",
                    acceptableValues: new AcceptableValueRange<float>(0f, 2f)
                );
                SpreadMult.SettingChanged += UpdateSettings;
            }

            if (defaultVelocity != float.NaN)
            {
                VelocityMult = configFile.BindConfigInOrder(
                    sectionName,
                    "Velocity Multiplier",
                    defaultVelocity,
                    "Multiplies velocity of projectiles.",
                    acceptableValues: new AcceptableValueRange<float>(0.1f, 2f)
                );
                VelocityMult.SettingChanged += UpdateSettings;
            }

            if (defaultLaunchAngle != float.NaN)
            {
                string vanillaAngle = defaultVanillaLaunchAngle != float.NaN ? $"Vanilla default is {defaultVanillaLaunchAngle}. " : string.Empty;
                LaunchAngle = configFile.BindConfigInOrder(
                    sectionName,
                    "Launch Angle",
                    defaultLaunchAngle,
                    $"Changes the launch angle of projectiles. {vanillaAngle}" +
                    "Negative values angle upwards, and positive values angle downwards.",
                    acceptableValues: new AcceptableValueRange<float>(-5f, 5f)
                );
                LaunchAngle.SettingChanged += UpdateSettings;
            }

            if (defaultHorizontalOffset != float.NaN)
            {
                HorizontalOffset = configFile.BindConfigInOrder(
                    sectionName,
                    "Horizontal Offset",
                    defaultHorizontalOffset,
                    "Offsets the location that projectiles are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.",
                    acceptableValues: new AcceptableValueRange<float>(-0.75f, 0.75f)
                );
                HorizontalOffset.SettingChanged += UpdateSettings;
            }

            if (defaultVerticalOffset != float.NaN)
            {
                VerticalOffset = configFile.BindConfigInOrder(
                    sectionName,
                    "Vertical Offset",
                    defaultVerticalOffset,
                    "Offsets the location that projectiles are launched from when firing them.Positive shifts it upwards.Negative shifts it downwards.",
                    acceptableValues: new AcceptableValueRange<float>(-0.75f, 0.75f)
                );
                VerticalOffset.SettingChanged += UpdateSettings;
            }
        }
    }

    internal ProjectileConfig BowTweaks;
    internal ProjectileConfig BombTweaks;
    internal ProjectileConfig XBowTweaks;
    internal ProjectileConfig SpearTweaks;
    internal ProjectileConfig StaffTweaks;

    // Zoom settings
    internal ConfigEntry<bool> EnableBowZoom { get; private set; }

    internal ConfigEntry<bool> EnableXbowZoom { get; private set; }
    internal ConfigEntry<KeyCode> ZoomKey { get; private set; }
    internal ConfigEntry<KeyCode> CancelDrawKey { get; private set; }
    internal ConfigEntry<float> StayInZoomTime { get; private set; }
    internal ConfigEntry<float> TimeToZoomIn { get; private set; }
    internal ConfigEntry<float> ZoomFactor { get; private set; }
    internal  ConfigEntry<bool> AutoBowZoom { get; private set; }

    /// <summary>
    ///     Bool indicating if zooming is enabled for bow || crossbow
    /// </summary>
    internal bool IsZoomEnabled => EnableBowZoom.Value || EnableXbowZoom.Value;

    /// <summary>
    ///     Called by Unity
    /// </summary>
    public void Awake()
    {
        Instance = this;
        Log.Init(this.Logger);

        Config.Init(PluginGUID, false);
        SetUpConfigEntries();
        Config.Save();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

        Game.isModded = true;

        Config.SetupWatcher();
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
        Config.Save();
    }

    private void SetUpConfigEntries()
    {
        BowTweaks = new ProjectileConfig(
            Config,
            BowSection,
            defaultSpread: 1f,
            defaultVelocity: 1f,
            defaultLaunchAngle: -1f,
            defaultVanillaLaunchAngle: 0f,
            defaultLoadSpeed: 1f,
            defaultHorizontalOffset: 0.2f,
            defaultVerticalOffset: 0.2f
        );

        BombTweaks = new ProjectileConfig(
            Config,
            BombSection,
            defaultSpread: 1f,
            defaultVelocity: 1f,
            defaultLaunchAngle: -1f,
            defaultVanillaLaunchAngle: 0f,
            defaultLoadSpeed: float.NaN,
            defaultHorizontalOffset: 0.0f,
            defaultVerticalOffset: 0.5f
        );

        XBowTweaks = new ProjectileConfig(
            Config,
            XbowSection,
            defaultSpread: 1f,
            defaultVelocity: 1f,
            defaultLaunchAngle: -1f,
            defaultVanillaLaunchAngle: -1f,
            defaultLoadSpeed: 1f,
            defaultHorizontalOffset: 0.0f,
            defaultVerticalOffset: 0.0f
        );

        SpearTweaks = new ProjectileConfig(
            Config,
            SpearSection,
            defaultSpread: 1f,
            defaultVelocity: 1f,
            defaultLaunchAngle: -1f,
            defaultVanillaLaunchAngle: 0f,
            defaultLoadSpeed: float.NaN,
            defaultHorizontalOffset: 0.1f,
            defaultVerticalOffset: 0.5f
        );

        StaffTweaks = new ProjectileConfig(
            Config,
            StaffSection,
            defaultSpread: 1f,
            defaultVelocity: 1f,
            defaultLaunchAngle: float.NaN,
            defaultVanillaLaunchAngle: 0f,
            defaultLoadSpeed: float.NaN,
            defaultHorizontalOffset: 0.0f,
            defaultVerticalOffset: 0.3f
        );
       
        // Zoom Configs
        EnableBowZoom = Config.BindConfigInOrder(
            ZoomSection,
            "Bow Zoom",
            true,
            "Set to true/enabled to allow zooming while using a bow.");
        EnableBowZoom.SettingChanged += UpdateSettings;

        EnableXbowZoom = Config.BindConfigInOrder(
            ZoomSection,
            "Crossbow Zoom",
            true,
            "Set to true/enabled to allow zooming while using a crossbow.");
        EnableXbowZoom.SettingChanged += UpdateSettings;

        ZoomKey = Config.BindConfigInOrder(
            ZoomSection,
            "Zoom Key",
            KeyCode.Mouse1,
            "Set the key used to zoom in while using a bow or crossbow.",
            synced: false);
        ZoomKey.SettingChanged += UpdateSettings;

        CancelDrawKey = Config.BindConfigInOrder(
            ZoomSection,
            "Cancel Draw Key",
            KeyCode.E,
            "Set the key used to cancel drawing your bow.",
            synced: false);
        CancelDrawKey.SettingChanged += UpdateSettings;

        TimeToZoomIn = Config.BindConfigInOrder(
            ZoomSection,
            "Time to Zoom in",
            1f,
            "Time that it takes to zoom in all the way. '1' is default and recommended.",
            acceptableValues: new AcceptableValueRange<float>(0.2f, 2f),
            synced: false
        );
        TimeToZoomIn.SettingChanged += UpdateSettings;

        StayInZoomTime = Config.BindConfigInOrder(
            ZoomSection,
            "Stay In-Zoom Time",
            2f,
            "Set the maximum time the camera will stay zoomed in while holding the zoom key after firing a projectile.",
            acceptableValues: new AcceptableValueRange<float>(0.5f, 4f),
            synced: false
        );
        StayInZoomTime.SettingChanged += UpdateSettings;

        ZoomFactor = Config.BindConfigInOrder(
            ZoomSection,
            "Zoom Factor",
            2f,
            "Set how much too zoom in relative to current camera view.",
            acceptableValues: new AcceptableValueRange<float>(1f, 4f),
            synced: false
        );
        ZoomFactor.SettingChanged += UpdateSettings;

        AutoBowZoom = Config.BindConfigInOrder(
            ZoomSection,
            "Auto Bow Zoom",
            false,
            "Set to true/enabled to make bows automatically zoom in as they are drawn.",
            synced: false
        );
        AutoBowZoom.SettingChanged += UpdateSettings;
    }

    private void UpdateConfigFile()
    {
        if (ShouldSaveConfig)
        {
            Config.Save();
            ShouldSaveConfig = false;
        }
    }

    private static void UpdateSettings(object obj, EventArgs e)
    {
        ShouldSaveConfig |= !ShouldSaveConfig;
    }
}
