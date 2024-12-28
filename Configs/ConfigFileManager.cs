using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Logging;
using static System.Collections.Specialized.BitVector32;

namespace Configs;

/// <summary>
///     Extends ConfigFile with a convenience method to bind config entries with less boilerplate code 
///     and explicitly expose commonly used configuration manager attributes.
/// </summary>
public static class ConfigFileManager
{
    internal static string ConfigFileName;
    internal static string ConfigFileFullPath;
    internal static DateTime lastRead = DateTime.MinValue;

    internal class ConfigFileOrderingMaps
    {
        public Dictionary<string, int> sectionToSectionNumber = [];
        public Dictionary<string, int> sectionToSettingOrder = [];
    }
    private static readonly Dictionary<string, ConfigFileOrderingMaps> _configFileOrderingMaps = [];

    /// <summary>
    ///     Formats section name as "{sectionNumber} - {section}" based on how
    ///     many sections have been bound to this config.
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    private static string GetOrderedSectionName(this ConfigFile configFile, string section)
    {
        if (!_configFileOrderingMaps.TryGetValue(configFile.ConfigFilePath, out ConfigFileOrderingMaps orderingMaps))
        {
            orderingMaps = new ConfigFileOrderingMaps();
            _configFileOrderingMaps.Add(configFile.ConfigFilePath, orderingMaps);
        }

        if (!orderingMaps.sectionToSectionNumber.TryGetValue(section, out int number))
        {
            number = orderingMaps.sectionToSectionNumber.Count + 1;
            orderingMaps.sectionToSectionNumber[section] = number;
        }

        return $"{number} - {section}";
    }

    /// <summary>
    ///     Orders settings within a section.
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    private static int GetSettingOrder(this ConfigFile configFile, string section)
    {
        if (!_configFileOrderingMaps.TryGetValue(configFile.ConfigFilePath, out ConfigFileOrderingMaps orderingMaps))
        {
            orderingMaps = new ConfigFileOrderingMaps();
            _configFileOrderingMaps.Add(configFile.ConfigFilePath, orderingMaps);
        }

        if (!orderingMaps.sectionToSettingOrder.TryGetValue(section, out int order))
        {
            order = 0;
        }

        orderingMaps.sectionToSettingOrder[section] = order - 1;
        return order;
    }

    internal static string GetExtendedDescription(string description, bool synchronizedSetting)
    {
        // these two hardcoded strings should probably be localized
        return description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]");
    }

    private static readonly List<string> ConfigManagerGUIDs = new List<string>()
    {
        "_shudnal.ConfigurationManager",
        "com.bepis.bepinex.configurationmanager"
    };
    private static BaseUnityPlugin _configManager = null;

    /// <summary>
    ///     Gets and caches a reference to the in-game config manager if one is installed.
    /// </summary>
    private static BaseUnityPlugin ConfigManager
    {
        get
        {
            if (_configManager == null)
            {
                foreach (string GUID in ConfigManagerGUIDs)
                {
                    if (Chainloader.PluginInfos.TryGetValue(GUID, out PluginInfo configManagerInfo) && configManagerInfo.Instance)
                    {
                        _configManager = configManagerInfo.Instance;
                        break;
                    }
                }
            }
            return _configManager;
        }
    }

    private const string WindowChangedEventName = "DisplayingWindowChanged";
    private const string DisplayingWindowName = "DisplayingWindow";
    private static PropertyInfo _dispWindowInfo = null;

    /// <summary>
    ///     Caches the PropertyInfo for ConfigManager.DisplayingWindow
    /// </summary>
    private static PropertyInfo DisplayWindowInfo
    {
        get
        {
            _dispWindowInfo ??= ConfigManager.GetType().GetProperty(DisplayingWindowName);
            return _dispWindowInfo;
        }
    }

    /// <summary>
    ///     Event triggered after the file watcher reloads the configuration file.
    /// </summary>
    internal static event Action OnConfigFileReloaded;

    /// <summary>
    ///     Safely invoke the <see cref="OnConfigFileReloaded"/> event
    /// </summary>
    private static void InvokeOnConfigFileReloaded()
    {
        OnConfigFileReloaded?.SafeInvoke();
    }

    /// <summary>
    ///     Event triggered after a the in-game configuration manager is closed.
    /// </summary>
    internal static event Action OnConfigWindowClosed;

    /// <summary>
    ///     Safely invoke the <see cref="OnConfigWindowClosed"/> event.
    /// </summary>
    private static void InvokeOnConfigWindowClosed()
    {
        OnConfigWindowClosed?.SafeInvoke();
    }

    public static void Init(this ConfigFile configFile, string GUID, bool saveOnConfigSet = false)
    {
        configFile.SaveOnConfigSet = saveOnConfigSet;
        ConfigFileName = GUID + ".cfg";
        ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);
    }

    /// <summary>
    ///     Sets SaveOnConfigSet to false and returns
    ///     the Value prior to calling this method.
    /// </summary>
    /// <returns></returns>
    public static bool DisableSaveOnConfigSet(this ConfigFile configFile)
    {
        bool val = configFile.SaveOnConfigSet;
        configFile.SaveOnConfigSet = false;
        return val;
    }

    /// <summary>
    ///     Bind a new config entry to the config file and modify description to state whether the config entry is synced or not.
    /// </summary>
    /// <typeparam name="T">Type of the value the config entry holds.</typeparam>
    /// <param name="configFile">Configuration file to bind the config entry to.</param>
    /// <param name="section">Configuration file section to list the config entry in. Settings are grouped by this.</param>
    /// <param name="key">Name of the setting.</param>
    /// <param name="defaultValue">Default value of the config entry.</param>
    /// <param name="description">Plain text description of the config entry to display as hover text in configuration manager.</param>
    /// <param name="synced">Whether the config entry IsAdminOnly and should be synced with server.</param>
    /// <param name="sectionOrder">Whether to number the section names using a prefix based on the order they are bound to the config file.</param>
    /// <param name="settingOrder">Whether to order the settings in each section based on the order they are bound to the config file.</param>
    /// <param name="acceptableValues">Acceptable values for config entry as an AcceptableValueRange, AcceptableValueList, or custom subclass.</param>
    /// <param name="customDrawer">Custom setting editor (OnGUI code that replaces the default editor provided by ConfigurationManager).</param>
    /// <param name="configAttributes">Config manager attributes for additional user specified functionality. Any fields of BindConfig will overwrite properties in configAttributes.</param>
    /// <returns>ConfigEntry bound to the config file.</returns>
    public static ConfigEntry<T> BindConfigInOrder<T>(
        this ConfigFile configFile,
        string section,
        string key,
        T defaultValue,
        string description,
        bool synced = true,
        bool sectionOrder = true,
        bool settingOrder = true,
        AcceptableValueBase acceptableValues = null,
        Action<ConfigEntryBase> customDrawer = null,
        ConfigurationManagerAttributes configAttributes = null
    )
    {
        section = sectionOrder ? configFile.GetOrderedSectionName(section) : section;
        int order = settingOrder ? configFile.GetSettingOrder(section) : 0;
        return configFile.BindConfig(section, key, defaultValue, description, synced, order, acceptableValues, customDrawer, configAttributes);
    }

    /// <summary>
    ///     Bind a new config entry to the config file and modify description to state whether the config entry is synced or not.
    /// </summary>
    /// <typeparam name="T">Type of the value the config entry holds.</typeparam>
    /// <param name="configFile">Configuration file to bind the config entry to.</param>
    /// <param name="section">Configuration file section to list the config entry in. Settings are grouped by this.</param>
    /// <param name="key">Name of the setting.</param>
    /// <param name="defaultValue">Default value of the config entry.</param>
    /// <param name="description">Plain text description of the config entry to display as hover text in configuration manager.</param>
    /// <param name="synced">Whether the config entry IsAdminOnly and should be synced with server.</param>
    /// <param name="order">Order of the setting on the settings list relative to other settings in a category. 0 by default, higher number is higher on the list.</param>
    /// <param name="acceptableValues">Acceptable values for config entry as an AcceptableValueRange, AcceptableValueList, or custom subclass.</param>
    /// <param name="customDrawer">Custom setting editor (OnGUI code that replaces the default editor provided by ConfigurationManager).</param>
    /// <param name="configAttributes">Config manager attributes for additional user specified functionality. Any fields of BindConfig will overwrite properties in configAttributes.</param>
    /// <returns>ConfigEntry bound to the config file.</returns>
    public static ConfigEntry<T> BindConfig<T>(
        this ConfigFile configFile,
        string section,
        string key,
        T defaultValue,
        string description,
        bool synced = true,
        int? order = null,
        AcceptableValueBase acceptableValues = null,
        Action<ConfigEntryBase> customDrawer = null,
        ConfigurationManagerAttributes configAttributes = null
    )
    {
        string extendedDescription = GetExtendedDescription(description, synced);
        configAttributes ??= new ConfigurationManagerAttributes();
        configAttributes.IsAdminOnly = synced;
        configAttributes.Order = order;
        configAttributes.CustomDrawer = customDrawer;
        ConfigEntry<T> configEntry = configFile.Bind(
            section,
            key,
            defaultValue,
            new ConfigDescription(extendedDescription, acceptableValues, configAttributes)
        );
        return configEntry;
    }

    

    /// <summary>
    ///     Create a file watcher to triger reloads of the config file when 
    ///     it is chaned, created, or renamed.
    /// </summary>
    /// <param name="configFile"></param>
    internal static void SetupWatcher(this ConfigFile configFile)
    {
        var watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
        watcher.Changed += configFile.ReloadConfigFile;
        watcher.Created += configFile.ReloadConfigFile;
        watcher.Renamed += configFile.ReloadConfigFile;
        watcher.IncludeSubdirectories = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    ///     Reloads config file if and only if the last write time difers from the last read time.
    /// </summary>
    /// <param name="configFile"></param>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    internal static void ReloadConfigFile(this ConfigFile configFile, object sender, FileSystemEventArgs eventArgs)
    {

        if (!File.Exists(ConfigFileFullPath))
        {
            return;
        }

        try
        {
            DateTime lastWriteTime = File.GetLastWriteTime(eventArgs.FullPath);
            if (lastRead != lastWriteTime)
            {
                Log.LogInfo("Reloading config file");
                bool saveOnConfigSet = configFile.DisableSaveOnConfigSet(); // turn off saving on config entry set
                configFile.Reload();
                configFile.SaveOnConfigSet = saveOnConfigSet; // reset config saving state

                InvokeOnConfigFileReloaded(); // fire event
                lastRead = lastWriteTime;
            }
        }
        catch
        {
            Log.LogError($"There was an issue loading your {ConfigFileName}");
            Log.LogError("Please check your config entries for spelling and format!");
        }
    }

    /// <summary>
    ///     Checks for in-game configuration manager and
    ///     sets Up OnConfigWindowClosed event if it is present
    /// </summary>
    internal static void CheckForConfigManager(this ConfigFile config
        )
    {
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
        {
            return;
        }

        if (ConfigManager == null)
        {
            return;
        }
        Log.LogDebug($"Configuration manager found, hooking {WindowChangedEventName}");

        EventInfo eventinfo = ConfigManager.GetType().GetEvent(WindowChangedEventName);
        if (eventinfo == null)
        {
            return;
        }

        Action<object, object> local = new(OnConfigManagerDisplayingWindowChanged);
        Delegate converted = Delegate.CreateDelegate(
            eventinfo.EventHandlerType,
            local.Target,
            local.Method
        );
        eventinfo.AddEventHandler(ConfigManager, converted);
    }

    /// <summary>
    ///     Invokes OnConfigWindowClosed if window has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnConfigManagerDisplayingWindowChanged(object sender, object e)
    {
        if (DisplayWindowInfo == null)
        {
            return;
        }

        bool ConfigurationManagerWindowShown = (bool)DisplayWindowInfo.GetValue(ConfigManager, null);
        if (!ConfigurationManagerWindowShown)
        {
            InvokeOnConfigWindowClosed();
        }
    }

    /// <summary>
    ///     try/catch the delegate chain so that it doesn't break on the first failing Delegate.
    /// </summary>
    /// <param name="events"></param>
    private static void SafeInvoke(this Action events)
    {
        if (events == null)
        {
            return;
        }

        foreach (Action @event in events.GetInvocationList())
        {
            try
            {
                @event();
            }
            catch (Exception e)
            {
                Log.LogWarning(
                    $"Exception thrown at event {new StackFrame(1).GetMethod().Name}"
                    + $" in {@event.Method.DeclaringType.Name}.{@event.Method.Name}:\n{e}"
                );
            }
        }
    }
}
