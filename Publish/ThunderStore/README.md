# ProjectileTweaks
A lightweight gameplay mod that lets you customize how firing projectiles works. Allows customization of bows, crossbows, and staffs. Uses Jotunn to sync configurations if the mod is installed on the server.

**Server-Side Info**: This mod works as a purely client side mod but can optionally be installed on the server to enforce custom configuration settings.

## Description
ProjectileTweaks is intended to be a lightweight and open-source mod that provides an alternative to BetterArchery and BowPlugin. Some of ProjectileTweaks features also overlap with MagicPlugin but the two are fully compatible so you can still enjoy the extra features from MagicPlugin.

Tweaks are applied separately for bows, crossbows, spears, and staffs.

### Bows, Crossbows, & Spears
For bows, crossbows, and Spears you can customize:
- Spread of the projectiles (this is referred to as accuracy in BowPlugin)
- Velocity of projectiles
- Launch angle of arrows and bolts
- Position that projectiles spawn relative to the player when they are fired by changing the horizontal and vertical offsets

The default configuration adjusts bows and spears to launch projectiles at the same angle as crossbows. The spawn locations of arrows and spears are also tweaked to improve the alignment of arrow trajectory with the aiming crosshairs, so that arrows and spears fired with zero spread launch in alignment with the crosshair rather than being slightly offset towards the players left side.

### Staffs
For staffs you can customize:
- Spread of projectiles
- Velocity of projectiles
- Position that projectiles spawn relative to the player when they are fired by changing the horizontal and vertical offsets

The default configuration adjusts the spawn position of projectiles fired from staffs so that if spread is set to zero the projectile is aligned with the aiming crosshair.

## Configuration
A config file BepInEx/config/Searica.Valheim.ProjectileTweaks.cfg is created after running the game once with this mod. You can adjust the config values by editing this file using a text editor or in-game using the Configuration Manager. The mod has a built-in file watcher so either method will work and changes will be reflected in-game as you change the settings.

### Bow Settings
**SpreadMultiplier** [Synced with Server]
- Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.
    - AcceptableValueRange: (0, 2)
    - Default value: 1

**VelocityMultiplier** [Synced with Server]
- Multiplies velocity of arrows.
    - AcceptableValueRange: (0.1, 2)
    - Default value: 1

**LaunchAngle** [Synced with Server]
- Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.
    - AcceptableValueRange: (-5, 5)
    - Default value: -1

**HorizontalOffset** [Synced with Server]
- Offsets the location that arrows are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.2

**VerticalOffset** [Synced with Server]
- Offsets the location that arrows are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.2


### Crossbow Settings
**SpreadMultiplier** [Synced with Server]
- Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.
    - AcceptableValueRange: (0, 2)
    - Default value: 1

**VelocityMultiplier** [Synced with Server]
- Multiplies velocity of bolts.
    - AcceptableValueRange: (0.1, 2)
    - Default value: 1

**LaunchAngle** [Synced with Server]
- Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.
    - AcceptableValueRange: (-5, 5)
    - Default value: -1

**HorizontalOffset** [Synced with Server]
- Offsets the location that bolts are launched from when firing them.Positive shifts it to your characters right. Negative shifts it to your characters left.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.0

**VerticalOffset** [Synced with Server]
- Offsets the location that bolts are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.0

### Spear Settings
**SpreadMultiplier** [Synced with Server]
- Multiplies the min and max projectile spread, so if you set it to zero your spear throws will have zero spread.
    - AcceptableValueRange: (0, 2)
    - Default value: 1

**VelocityMultiplier** [Synced with Server]
- Multiplies velocity of bolts.
    - AcceptableValueRange: (0.1, 2)
    - Default value: 1

**LaunchAngle** [Synced with Server]
- Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.
    - AcceptableValueRange: (-5, 5)
    - Default value: -1

**HorizontalOffset** [Synced with Server]
- Offsets the location that thrown spears are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.1

**VerticalOffset** [Synced with Server]
- Offsets the location that thrown spears are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.5


### Staff Settings
**SpreadMultiplier** [Synced with Server]
- Multiplies the min and max projectile spread, so if you set it to zero your projectiles will have zero spread.
    - AcceptableValueRange: (0, 2)
    - Default value: 1

**VelocityMultiplier** [Synced with Server]
- Multiplies velocity of projectiles.
    - AcceptableValueRange: (0.1, 2)
    - Default value: 1

**HorizontalOffset** [Synced with Server]
- Offsets the location that projectiles are launched from when firing them.Positive shifts it to your characters right. Negative shifts it to your characters left.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.0

**VerticalOffset** [Synced with Server]
- Offsets the location that projectiles are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
    - AcceptableValueRange: (-0.5, 0.5)
    - Default value: 0.3

## Compatibility
These are non-exhaustive lists, feel free to let me know if you want a mod added to any of the lists.

### Incompatible Mods
**BetterArchery (by ishid4)**: BetterArchery touches a lot of the same game code and the features it provides can be replaced using a combination of ProjectileTweaks and BowsBeforeHoes so I won't be supporting any incompatibly or unexpected behavior that occurs if you use ProjectileTweaks alongside BetterArchery.

### Partial Incompatibility
**MagicPlugin (by Blacks7ar)**: ProjectileTweaks and MagicPlugin can be used together without issue but both mods allow adjusting how projectiles launched from staffs work. If you use these two mods together then leave `SpreadMultiplier` and `VelocityMultiper` set to 1 in the configuration for ProjectileTweaks.

**BowPlugin (by Blacks7ar)**: ProjectileTweaks and MagicPlugin can be used together without issue but both mods allow adjusting how projectiles fired from bows and crossbows work. If you use these two mods together then leave `SpreadMultiplier` and `VelocityMultipier` set to 1 in the configuration for both Bows and Crossbows. I have also set up ProjectileTweaks so that the settings for `LaunchAngle` should be overridden by the settings in BowPlugin, so `LaunchAngle` settings in ProjectileTweaks will have no effect if BowPlugin is installed.

### Compatible Mods
**BowsBeforeHoes (by Azumatt)**: Fully compatible and BowsBeforeHoes is recommended if you want a quiver for your arrows, a zoom feature while aiming, and the option to recover arrows after firing them.

## Donations/Tips
 My mods will always be free to use but if you feel like saying thanks you can tip/donate.

  [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/searica)

## Source Code
Github: https://github.com/searica/ProjectileTweaks

### Contributions
If you would like to provide suggestions, make feature requests, or reports bugs and compatibility issues you can either open an issue on the Github repository or tag me (@searica) with a message on my discord [Searica's Mods](https://discord.gg/sFmGTBYN6n).
<!--the [Jotunn discord](https://discord.gg/DdUt6g7gyA), or the [Odin Plus discord](https://discord.gg/mbkPcvu9ax)-->


 I'm a grad student and have a lot of personal responsibilities on top of that so I can't promise I will always respond quickly, but I do intend to maintain the mod in my free time.

### Credits
This mod was inspired by BetterArchery and BowPlugin. Also a huge shout-out and thanks to the developers of Jotunn for all their work making and maintaining Jotunn.


## Shameless Self Plug (Other Mods By Me)
If you like this mod you might like some of my other ones.

#### Building Mods
- [MoreVanillaBuildPrefabs](https://valheim.thunderstore.io/package/Searica/More_Vanilla_Build_Prefabs/)
- [Extra Snap Points Made Easy](https://valheim.thunderstore.io/package/Searica/Extra_Snap_Points_Made_Easy/)

#### Gameplay Mods
- [FortifySkillsRedux](https://valheim.thunderstore.io/package/Searica/FortifySkillsRedux/)
- [DodgeShortcut](https://valheim.thunderstore.io/package/Searica/DodgeShortcut/)
<!--- [ProjectileTweaks](https://github.com/searica/ProjectileTweaks)-->
