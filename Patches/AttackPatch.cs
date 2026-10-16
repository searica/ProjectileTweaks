using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;

namespace ProjectileTweaks.Patches;

[HarmonyPatch]
internal static class AttackPatch
{
    private static ProjectileTweaks.ProjectileConfig RefProjectileConfig;
    private static float cached_projectileVel;
    private static float cached_projectileVelMin;
    private static float cached_projectileAccuracy;
    private static float cached_projectileAccuracyMin;
    private static ItemDrop.ItemData WeaponRef;

    [HarmonyPostfix]
    [HarmonyBefore("blacks7ar.BowPlugin")]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
    internal static void AttackStartPrefix(Attack __instance)
    {
        if (!TryGetPlayerWeapon(__instance, out WeaponRef))
        { 
            return; 
        }
        if (TryGetProjectileConfig(WeaponRef, out RefProjectileConfig))
        {
            SetVelocity(__instance, RefProjectileConfig.VelocityMult);
            SetSpread(__instance, RefProjectileConfig.SpreadMult);
            if (RefProjectileConfig.LaunchAngle is not null)
            {
                SetLaunchAngle(__instance, RefProjectileConfig.LaunchAngle);
            }
        }
    }

    private static void SetVelocity(Attack attack, ConfigEntry<float> velocityMultiplier)
    {
        // store values for use with delegate
        cached_projectileVel = attack.m_projectileVel;
        cached_projectileVelMin = attack.m_projectileVelMin;

        velocityMultiplier.SettingChanged += (sender, e) =>
        {
            attack.m_projectileVel = cached_projectileVel * velocityMultiplier.Value;
            attack.m_projectileVelMin = cached_projectileVelMin * velocityMultiplier.Value;
        };

        attack.m_projectileVel *= velocityMultiplier.Value;
        attack.m_projectileVelMin *= velocityMultiplier.Value;
    }

    private static void SetSpread(Attack attack, ConfigEntry<float> spreadConfig)
    {
        cached_projectileAccuracy = attack.m_projectileAccuracy;
        cached_projectileAccuracyMin = attack.m_projectileAccuracyMin;

        spreadConfig.SettingChanged += (sender, e) =>
        {
            attack.m_projectileAccuracy = cached_projectileAccuracy * spreadConfig.Value;
            attack.m_projectileAccuracyMin = cached_projectileAccuracyMin * spreadConfig.Value;
        };

        attack.m_projectileAccuracy *= spreadConfig.Value;
        attack.m_projectileAccuracyMin *= spreadConfig.Value;
    }

    private static void SetLaunchAngle(Attack attack, ConfigEntry<float> angleConfig)
    {
        angleConfig.SettingChanged += (sender, e) =>
        {
            attack.m_launchAngle = angleConfig.Value;
        };

        attack.m_launchAngle = angleConfig.Value;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Attack), nameof(Attack.GetProjectileSpawnPoint))]
    private static void GetProjectileSpawnPointPostfix(Attack __instance, ref Vector3 spawnPoint, ref Vector3 aimDir)
    {
        if (!TryGetPlayerWeapon(__instance, out WeaponRef))
        {
            return;
        }

        if (TryGetProjectileConfig(WeaponRef, out RefProjectileConfig))
        {
            ApplyOffset(
                ref spawnPoint,
                ref aimDir,
                RefProjectileConfig.VerticalOffset.Value,
                RefProjectileConfig.HorizontalOffset.Value,
                RefProjectileConfig.DepthOffset.Value
            );
        }
    }

    private static bool TryGetProjectileConfig(ItemDrop.ItemData weapon, out ProjectileTweaks.ProjectileConfig projectileConfig)
    {
        if (weapon == null) {
            projectileConfig = null;
            return false; 
        }

        Skills.SkillType skillType = weapon.m_shared.m_skillType;
        if (skillType == Skills.SkillType.Bows)
        {
            projectileConfig = ProjectileTweaks.BowTweaks;
            return true;
        }
        else if (skillType == Skills.SkillType.Crossbows)
        {
            projectileConfig = ProjectileTweaks.XBowTweaks;
            return true; ;
        }
        else if (skillType == Skills.SkillType.Spears)
        {
            projectileConfig = ProjectileTweaks.SpearTweaks;
            return true;
        }
        else if (skillType == Skills.SkillType.ElementalMagic || skillType == Skills.SkillType.BloodMagic)
        {
            if (ProjectileTweaks.ShootingStaffNames.Contains(weapon.m_dropPrefab.name))
            {
                projectileConfig = ProjectileTweaks.ShootingStaffTweaks;
                return true;
            }
            else if (ProjectileTweaks.ThrowingStaffNames.Contains(weapon.m_dropPrefab.name))
            {
                projectileConfig = ProjectileTweaks.ThrowingStaffTweaks;
                return true;
            }
        }
        else if (IsBomb(weapon))
        {
            projectileConfig = ProjectileTweaks.BombTweaks;
            return true;
        }

        projectileConfig = null;
        return false;
    }

    private static void ApplyOffset(
        ref Vector3 spawnPoint,
        ref Vector3 aimDir,
        float vertOffset,
        float horzOffset,
        float depthOffset
    )
    {
        spawnPoint += depthOffset * (new Vector3(aimDir.x, 0.0f, aimDir.z).normalized);
        spawnPoint += horzOffset * (new Vector3(aimDir.z, 0.0f, -aimDir.x).normalized);
        spawnPoint += new Vector3(0.0f, vertOffset, 0.0f);
    }

    /// <summary>
    ///     Postfix patch runs on all attacks but we only care about getting the weapon from the local player
    /// </summary>
    /// <param name="attack"></param>
    /// <returns></returns>
    private static bool TryGetPlayerWeapon(Attack attack, out ItemDrop.ItemData weapon)
    {
        if (attack == null || !attack.m_character || !attack.m_character.IsPlayer() || attack.m_character != Player.m_localPlayer)
        {
            weapon = null;
            return false;
        }

        weapon = attack.m_character.GetCurrentWeapon();
        return true;
    }


    private static bool IsBomb(ItemDrop.ItemData item)
    {
        return item.m_shared.m_attack.m_attackAnimation == "throw_bomb";
    }
}
