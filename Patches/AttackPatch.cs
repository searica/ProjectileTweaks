using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;

namespace ProjectileTweaks.Patches;

[HarmonyPatch]
internal static class AttackPatch
{
    private static ProjectileTweaks.ProjectileConfig RefProjectileConfig;
    private static float m_projectileVel;
    private static float m_projectileVelMin;
    private static float m_projectileAccuracy;
    private static float m_projectileAccuracyMin;

    [HarmonyPostfix]
    [HarmonyBefore("blacks7ar.BowPlugin")]
    [HarmonyPriority(Priority.VeryHigh)]
    [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
    internal static void AttackStartPrefix(Attack __instance)
    {
        ItemDrop.ItemData weapon = GetPlayerWeapon(__instance);
        if (TryGetProjectileConfig(weapon, out RefProjectileConfig))
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
        m_projectileVel = attack.m_projectileVel;
        m_projectileVelMin = attack.m_projectileVelMin;

        velocityMultiplier.SettingChanged += (sender, e) =>
        {
            attack.m_projectileVel = m_projectileVel * velocityMultiplier.Value;
            attack.m_projectileVelMin = m_projectileVelMin * velocityMultiplier.Value;
        };

        attack.m_projectileVel *= velocityMultiplier.Value;
        attack.m_projectileVelMin *= velocityMultiplier.Value;
    }

    private static void SetSpread(Attack attack, ConfigEntry<float> spreadConfig)
    {
        m_projectileAccuracy = attack.m_projectileAccuracy;
        m_projectileAccuracyMin = attack.m_projectileAccuracyMin;

        spreadConfig.SettingChanged += (sender, e) =>
        {
            attack.m_projectileAccuracy = m_projectileAccuracy * spreadConfig.Value;
            attack.m_projectileAccuracyMin = m_projectileAccuracyMin * spreadConfig.Value;
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
        ItemDrop.ItemData weapon = GetPlayerWeapon(__instance);
        if(TryGetProjectileConfig(weapon, out RefProjectileConfig))
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

    private static ItemDrop.ItemData GetPlayerWeapon(Attack attack)
    {
        if (attack == null ||
            attack.m_character is not Player player ||
            !(player.m_name == "Human" ||
            player == Player.m_localPlayer))
        {
            return null;
        }

        return player.GetCurrentWeapon();
    }

    private static bool IsBomb(ItemDrop.ItemData item)
    {
        return item.m_shared.m_attack.m_attackAnimation == "throw_bomb";
    }
}
