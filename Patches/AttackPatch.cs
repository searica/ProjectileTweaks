using HarmonyLib;
using UnityEngine;
using ProjectileTweaks.Configs;

namespace ProjectileTweaks.Patches
{
    [HarmonyPatch(typeof(Attack))]
    internal class AttackPatch
    {
        [HarmonyPostfix]
        [HarmonyBefore("blacks7ar.BowPlugin")]
        [HarmonyPriority(Priority.VeryHigh)]
        [HarmonyPatch(nameof(Attack.Start))]
        internal static void AttackStartPrefix(Attack __instance)
        {
            var weapon = GetPlayerWeapon(__instance);
            if (weapon == null) { return; }

            Skills.SkillType skillType = weapon.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Bows)
            {
                // Velocity
                Config.BowVelocityMult.SettingChanged += delegate
                {
                    __instance.m_projectileVel *= Config.BowVelocityMult.Value;
                    __instance.m_projectileVelMin *= Config.BowVelocityMult.Value;
                };
                __instance.m_projectileVel *= Config.BowVelocityMult.Value;
                __instance.m_projectileVelMin *= Config.BowVelocityMult.Value;

                // Spread
                Config.BowSpreadMult.SettingChanged += delegate
                {
                    __instance.m_projectileAccuracy *= Config.BowSpreadMult.Value;
                    __instance.m_projectileAccuracyMin *= Config.BowSpreadMult.Value;
                };
                __instance.m_projectileAccuracy *= Config.BowSpreadMult.Value;
                __instance.m_projectileAccuracyMin *= Config.BowSpreadMult.Value;

                // Launch angle
                Config.BowLaunchAngle.SettingChanged += delegate
                {
                    __instance.m_launchAngle = Config.BowLaunchAngle.Value;
                };
                __instance.m_launchAngle = Config.BowLaunchAngle.Value;
            }
            else if (skillType == Skills.SkillType.Crossbows)
            {
                // Velocity
                Config.XbowVelocityMult.SettingChanged += delegate
                {
                    __instance.m_projectileVel *= Config.XbowVelocityMult.Value;
                    __instance.m_projectileVelMin *= Config.XbowVelocityMult.Value;
                };
                __instance.m_projectileVel *= Config.XbowVelocityMult.Value;
                __instance.m_projectileVelMin *= Config.XbowVelocityMult.Value;

                // Spread
                Config.XbowSpreadMult.SettingChanged += delegate
                {
                    __instance.m_projectileAccuracy *= Config.XbowSpreadMult.Value;
                    __instance.m_projectileAccuracyMin *= Config.XbowSpreadMult.Value;
                };
                __instance.m_projectileAccuracy *= Config.XbowSpreadMult.Value;
                __instance.m_projectileAccuracyMin *= Config.XbowSpreadMult.Value;

                // Launch angle
                Config.XbowLaunchAngle.SettingChanged += delegate
                {
                    __instance.m_launchAngle = Config.XbowLaunchAngle.Value;
                };
                __instance.m_launchAngle = Config.XbowLaunchAngle.Value;
            }
            else if (skillType == Skills.SkillType.Spears)
            {
                // Velocity
                Config.SpearVelocityMult.SettingChanged += delegate
                {
                    __instance.m_projectileVel *= Config.SpearVelocityMult.Value;
                    __instance.m_projectileVelMin *= Config.SpearVelocityMult.Value;
                };
                __instance.m_projectileVel *= Config.SpearVelocityMult.Value;
                __instance.m_projectileVelMin *= Config.SpearVelocityMult.Value;

                // Spread
                Config.SpearSpreadMult.SettingChanged += delegate
                {
                    __instance.m_projectileAccuracy *= Config.SpearSpreadMult.Value;
                    __instance.m_projectileAccuracyMin *= Config.SpearSpreadMult.Value;
                };
                __instance.m_projectileAccuracy *= Config.SpearSpreadMult.Value;
                __instance.m_projectileAccuracyMin *= Config.SpearSpreadMult.Value;

                // Launch angle
                Config.SpearLaunchAngle.SettingChanged += delegate
                {
                    __instance.m_launchAngle = Config.SpearLaunchAngle.Value;
                };
                __instance.m_launchAngle = Config.SpearLaunchAngle.Value;
            }
            else if (skillType == Skills.SkillType.ElementalMagic)
            {
                // Velocity
                Config.StaffVelocityMult.SettingChanged += delegate
                {
                    __instance.m_projectileVel *= Config.StaffVelocityMult.Value;
                    __instance.m_projectileVelMin *= Config.StaffVelocityMult.Value;
                };
                __instance.m_projectileVel *= Config.StaffVelocityMult.Value;
                __instance.m_projectileVelMin *= Config.StaffVelocityMult.Value;

                // Spread
                Config.StaffSpreadMult.SettingChanged += delegate
                {
                    __instance.m_projectileAccuracy *= Config.StaffSpreadMult.Value;
                    __instance.m_projectileAccuracyMin *= Config.StaffSpreadMult.Value;
                };
                __instance.m_projectileAccuracy *= Config.StaffSpreadMult.Value;
                __instance.m_projectileAccuracyMin *= Config.StaffSpreadMult.Value;
            }
        }

        // for unity
        // private static readonly Vector3 forwardVector = new Vector3(0f, 0f, 1f);

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Attack.GetProjectileSpawnPoint))]
        internal static void GetProjectileSpawnPointPostfix(Attack __instance, ref Vector3 spawnPoint, ref Vector3 aimDir)
        {
            var weapon = GetPlayerWeapon(__instance);
            if (weapon == null) { return; }

            // I think that I just want to use the aimDir vector to rotate the offsets
            // and then change the spawn point based on the config

            Skills.SkillType skillType = weapon.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Bows)
            {
                Log.LogInfo($"SpawnPoint {spawnPoint}");
                var horizontalOffset = new Vector3(aimDir.z, 0, -aimDir.x).normalized;
                horizontalOffset *= Config.BowHorizontalOffset.Value;
                spawnPoint += horizontalOffset;
                spawnPoint += new Vector3(0.0f, Config.BowVerticalOffset.Value, 0.0f);
                Log.LogInfo($"SpawnPoint {spawnPoint}");
            }
            else if (skillType == Skills.SkillType.Crossbows)
            {
                var horizontalOffset = new Vector3(aimDir.z, 0, -aimDir.x).normalized;
                horizontalOffset *= Config.XbowHorizontalOffset.Value;
                spawnPoint += horizontalOffset;
                spawnPoint += new Vector3(0.0f, Config.XbowVerticalOffset.Value, 0.0f);
            }
            else if (skillType == Skills.SkillType.Spears)
            {
                var horizontalOffset = new Vector3(aimDir.z, 0, -aimDir.x).normalized;
                horizontalOffset *= Config.SpearHorizontalOffset.Value;
                spawnPoint += horizontalOffset;
                spawnPoint += new Vector3(0.0f, Config.SpearVerticalOffset.Value, 0.0f);
            }
            else if (skillType == Skills.SkillType.ElementalMagic)
            {
                var horizontalOffset = new Vector3(aimDir.z, 0, -aimDir.x).normalized;
                horizontalOffset *= Config.StaffHorizontalOffset.Value;
                spawnPoint += horizontalOffset;
                spawnPoint += new Vector3(0.0f, Config.StaffVerticalOffset.Value, 0.0f);
            }
        }

        private static ItemDrop.ItemData GetPlayerWeapon(Attack attack)
        {
            if (
                attack == null ||
                attack.m_character is not Player player ||
                !(player.m_name == "Human" || player == Player.m_localPlayer)
            )
            {
                return null;
            }

            return player.GetCurrentWeapon();
        }
    }
}