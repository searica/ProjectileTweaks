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
A config file BepInEx/config/Searica.Valheim.ProjectileTweaks.Instance.cfg is created after running the game once with this mod. You can adjust the config values by editing this file using a text editor or in-game using the Configuration Manager. The mod has a built-in file watcher so either method will work and changes will be reflected in-game as you change the settings.

<div class="header">
	<h3>Bow Section</h3>
  These settings control features relating to bows.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
		<tr>
			<td align="center"><b>Draw Speed Multiplier</b></td>
            <td align="center">Yes</td>
			<td align="left">
                Multiplier for draw speed of bows. Set to 2 to draw twice as fast. Does not affect Vanilla scaling with skill level.
				<ul>
					<li>Acceptable values: (0.5, 2)</li>
					<li>Default value: 1</li>
				</ul>
			</td>
		</tr>
        <tr>
            <td align="center"><b>Spread Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.
                <ul>
                    <li>Acceptable values: (0, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Velocity Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies velocity of arrows.
                <ul>
                    <li>Acceptable values: (0.1, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Launch Angle</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Changes the launch angle for arrows. Vanilla default for bows is 0. Negative values angle upwards, and positive values angle downwards.
                <ul>
                    <li>Acceptable values: (-5, 5)</li>
                    <li>Default value: -1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Horizontal Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that arrows are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.2</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Vertical Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that arrows are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.2</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

<div class="header">
	<h3>Bomb Section</h3>
  These settings control features relating to bows.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
        <tr>
            <td align="center"><b>Spread Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies the min and max projectile spread, so if you set it to zero your arrows will have zero spread.
                <ul>
                    <li>Acceptable values: (0, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Velocity Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies velocity of bombs.
                <ul>
                    <li>Acceptable values: (0.1, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Launch Angle</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Changes the launch angle for bombs. Vanilla default for bombs is 0. Negative values angle upwards, and positive values angle downwards.
                <ul>
                    <li>Acceptable values: (-5, 5)</li>
                    <li>Default value: -1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Horizontal Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that bombs are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.0</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Vertical Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that bombs are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.5</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

<div class="header">
	<h3>Crossbow Section</h3>
  These settings control features relating to crossbows.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
		<tr>
			<td align="center"><b>Reload Speed Multiplier</b></td>
            <td align="center">Yes</td>
			<td align="left">
                Multiplier for reload speed of crossbows. Set to 2 to reload twice as fast. Does not affect Vanilla scaling with skill level.
				<ul>
					<li>Acceptable values: (0.5, 2)</li>
					<li>Default value: 1</li>
				</ul>
			</td>
		</tr>
        <tr>
            <td align="center"><b>Spread Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies the min and max projectile spread, so if you set it to zero your bolts will have zero spread.
                <ul>
                    <li>Acceptable values: (0, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Velocity Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies velocity of bolts.
                <ul>
                    <li>Acceptable values: (0.1, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Launch Angle</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Changes the launch angle for bolts. Vanilla default for crossbows is -1. Negative values angle upwards, and positive values angle downwards.
                <ul>
                    <li>Acceptable values: (-5, 5)</li>
                    <li>Default value: -1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Horizontal Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that bolts are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.0</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Vertical Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that bolts are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.0</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

<div class="header">
	<h3>Spears Section</h3>
  These settings control features relating to spears.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
        <tr>
            <td align="center"><b>Spread Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies the min and max projectile spread, so if you set it to zero your spear throws will have zero spread.
                <ul>
                    <li>Acceptable values: (0, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Velocity Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies velocity of thrown spears.
                <ul>
                    <li>Acceptable values: (0.1, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Launch Angle</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.
                <ul>
                    <li>Acceptable values: (-5, 5)</li>
                    <li>Default value: -1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Horizontal Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that thrown spears are launched from when throwing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Vertical Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that thrown spears are launched from when throwing them. Positive shifts it upwards. Negative shifts it downwards.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.5</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

<div class="header">
	<h3>Staffs Section</h3>
  These settings control features relating to staffs.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
        <tr>
            <td align="center"><b>Spread Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies the min and max projectile spread, so if you set it to zero there will be zero spread.
                <ul>
                    <li>Acceptable values: (0, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Velocity Multiplier</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Multiplies velocity of projectiles.
                <ul>
                    <li>Acceptable values: (0.1, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Launch Angle</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Changes the launch angle for thrown spears. Vanilla default for spears is 0. Negative values angle upwards, and positive values angle downwards.
                <ul>
                    <li>Acceptable values: (-5, 5)</li>
                    <li>Default value: -1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Horizontal Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that projectiles are launched from when firing them. Positive shifts it to your characters right. Negative shifts it to your characters left.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.0</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Vertical Offset</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Offsets the location that projectiles are launched from when firing them. Positive shifts it upwards. Negative shifts it downwards.
                <ul>
                    <li>Acceptable values:  (-0.75, 0.75)</li>
                    <li>Default value: 0.3</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

<div class="header">
	<h3>Zoom Section</h3>
  These settings control features relating to zooming in while aiming.
</div>
<table>
	<tbody>
		<tr>
			<th align="center">Setting</th>
            <th align="center">Server Sync</th>
			<th align="center">Description</th>
		</tr>
        <tr>
            <td align="center"><b>Bow Zoom</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Set to true/enabled to allow zooming while using a bow.
                <ul>
                    <li>Acceptable values: false, true</li>
                    <li>Default value: true</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Crossbow Zoom</b></td>
            <td align="center">Yes</td>
            <td align="left">
                Set to true/enabled to allow zooming while using a crossbow.
                <ul>
                    <li>Acceptable values: false, true</li>
                    <li>Default value: true</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Zoom Key</b></td>
            <td align="center">No</td>
            <td align="left">
                Set the key used to zoom in while using a bow or crossbow.
                <ul>
                    <li>Acceptable values: KeyCode</li>
                    <li>Default value: RightClick</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Cancel Draw Key</b></td>
            <td align="center">No</td>
            <td align="left">
                Set the key used to cancel drawing your bow.
                <ul>
                    <li>Acceptable values: KeyCode</li>
                    <li>Default value: E</li>
                </ul>
            </td>
        </tr>
         <tr>
            <td align="center"><b>Auto Bow Zoom</b></td>
            <td align="center">No</td>
            <td align="left">
                Set to true/enabled to make bows automatically zoom in as they are drawn.
                <ul>
                    <li>Acceptable values: false, true</li>
                    <li>Default value: false</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Time to Zoom in</b></td>
            <td align="center">No</td>
            <td align="left">
                Time that it takes to zoom in all the way. '1' is default and recommended.
                <ul>
                    <li>Acceptable values: (0.2, 2)</li>
                    <li>Default value: 1</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Stay In-Zoom Time</b></td>
            <td align="center">No</td>
            <td align="left">
                Set the maximum time the camera will stay zoomed in while holding the zoom key after firing a projectile.
                <ul>
                    <li>Acceptable values: (0.5, 4)</li>
                    <li>Default value: 2</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td align="center"><b>Zoom Factor</b></td>
            <td align="center">No</td>
            <td align="left">
                Set how much to zoom in relative to current camera view.
                <ul>
                    <li>Acceptable values: (1, 4)</li>
                    <li>Default value: 2</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

## Compatibility
These are non-exhaustive lists, feel free to let me know if you want a mod added to any of the lists.

### Incompatible Mods
**BetterArchery (by ishid4)**: BetterArchery touches a lot of the same game code and the features it provides can be replaced using a combination of ProjectileTweaks and BowsBeforeHoes so I won't be supporting any incompatibly or unexpected behavior that occurs if you use ProjectileTweaks alongside BetterArchery.

### Compatible Mods
**BowsBeforeHoes (by Azumatt)**: Fully compatible and BowsBeforeHoes is recommended if you want a quiver for your arrows, and the option to recover arrows after firing them. As both mods offer zoom features it is recommended you only enable the zoom feature from one mod.

**MagicPlugin (by Blacks7ar)**: ProjectileTweaks and MagicPlugin can be used together without issue but both mods allow adjusting how projectiles launched from staffs work. If you use these two mods together then leave `SpreadMultiplier` and `VelocityMultiper` set to 1 in the configuration for ProjectileTweaks.Instance.

**BowPlugin (by Blacks7ar)**: ProjectileTweaks and BowPlugin can be used together without issue but both mods allow adjusting how projectiles fired from bows and crossbows work. If you use these two mods together then leave `SpreadMultiplier` and `VelocityMultipier` set to 1 in the configuration for both Bows and Crossbows. I have also set up ProjectileTweaks so that the settings for `LaunchAngle` should be overridden by the settings in BowPlugin, so `LaunchAngle` settings in ProjectileTweaks will have no effect if BowPlugin is installed.


## Donations/Tips
My mods will always be free to use but if you feel like saying thanks you can tip/donate.

| My Ko-fi: | [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/searica) |
|-----------|---------------|

## Source Code
Source code is available on Github.

| Github Repository: | <img height="18" src="https://github.githubassets.com/favicons/favicon-dark.svg"></img><a href="https://github.com/searica/ProjectileTweaks"> ProjectileTweaks</a> |
|-----------|---------------|

### Contributions
If you would like to provide suggestions, make feature requests, or reports bugs and compatibility issues you can either open an issue on the Github repository or tag me (@searica) with a message on my discord [Searica's Mods](https://discord.gg/sFmGTBYN6n).

 I'm a grad student and have a lot of personal responsibilities on top of that so I can't promise I will always respond quickly, but I do intend to maintain the mod in my free time.

### Credits
This mod was inspired by BetterArchery and BowPlugin. Also a huge shout-out and thanks to the developers of Jotunn for all their work making and maintaining Jotunn.


## Shameless Self Plug (Other Mods By Me)
If you like this mod you might like some of my other ones.

#### Building Mods
- [More Vanilla Build Prefabs](https://thunderstore.io/c/valheim/p/Searica/More_Vanilla_Build_Prefabs/)
- [Extra Snap Points Made Easy](https://thunderstore.io/c/valheim/p/Searica/Extra_Snap_Points_Made_Easy/)
- [AdvancedTerrainModifiers](https://thunderstore.io/c/valheim/p/Searica/AdvancedTerrainModifiers/)
- [BuildRestrictionTweaksSync](https://thunderstore.io/c/valheim/p/Searica/BuildRestrictionTweaksSync/)
- [ToolTweaks](https://thunderstore.io/c/valheim/p/Searica/ToolTweaks/)

#### Gameplay Mods
- [CameraTweaks](https://thunderstore.io/c/valheim/p/Searica/CameraTweaks/)
- [DodgeShortcut](https://thunderstore.io/c/valheim/p/Searica/DodgeShortcut/)
- [FortifySkillsRedux](https://thunderstore.io/c/valheim/p/Searica/FortifySkillsRedux/)
- [SkilledCarryWeight](https://thunderstore.io/c/valheim/p/Searica/SkilledCarryWeight/)
- [SafetyStatus](https://thunderstore.io/c/valheim/p/Searica/SafetyStatus/)
