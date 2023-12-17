using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;

namespace ProjectileTweaks.Patches {
    [HarmonyPatch(typeof(Attack))]
    internal static class AttackPatch {
        private static float m_projectileVel;
        private static float m_projectileVelMin;
        private static float m_projectileAccuracy;
        private static float m_projectileAccuracyMin;

        [HarmonyPostfix]
        [HarmonyBefore("blacks7ar.BowPlugin")]
        [HarmonyPriority(Priority.VeryHigh)]
        [HarmonyPatch(nameof(Attack.Start))]
        internal static void AttackStartPrefix(Attack __instance) {
            var weapon = GetPlayerWeapon(__instance);
            if (weapon == null) { return; }

            Skills.SkillType skillType = weapon.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Bows) {
                SetVelocity(__instance, ProjectileTweaks.BowVelocityMult);
                SetSpread(__instance, ProjectileTweaks.BowSpreadMult);
                SetLaunchAngle(__instance, ProjectileTweaks.BowLaunchAngle);
            }
            else if (skillType == Skills.SkillType.Crossbows) {
                SetVelocity(__instance, ProjectileTweaks.XbowVelocityMult);
                SetSpread(__instance, ProjectileTweaks.XbowSpreadMult);
                SetLaunchAngle(__instance, ProjectileTweaks.XbowLaunchAngle);
            }
            else if (skillType == Skills.SkillType.Spears) {
                SetVelocity(__instance, ProjectileTweaks.SpearVelocityMult);
                SetSpread(__instance, ProjectileTweaks.SpearSpreadMult);
                SetLaunchAngle(__instance, ProjectileTweaks.SpearLaunchAngle);
            }
            else if (skillType == Skills.SkillType.ElementalMagic) {
                SetVelocity(__instance, ProjectileTweaks.StaffVelocityMult);
                SetSpread(__instance, ProjectileTweaks.StaffSpreadMult);
            }
            else if (IsBomb(weapon)) {
                SetVelocity(__instance, ProjectileTweaks.BombVelocityMult);
                SetSpread(__instance, ProjectileTweaks.BombSpreadMult);
                SetLaunchAngle(__instance, ProjectileTweaks.BombLaunchAngle);
            }
        }

        private static void SetVelocity(Attack attack, ConfigEntry<float> velocityMultiplier) {
            // store values for use with delegate
            m_projectileVel = attack.m_projectileVel;
            m_projectileVelMin = attack.m_projectileVelMin;

            velocityMultiplier.SettingChanged += (sender, e) => {
                attack.m_projectileVel = m_projectileVel * velocityMultiplier.Value;
                attack.m_projectileVelMin = m_projectileVelMin * velocityMultiplier.Value;
            };

            attack.m_projectileVel *= velocityMultiplier.Value;
            attack.m_projectileVelMin *= velocityMultiplier.Value;
        }

        private static void SetSpread(Attack attack, ConfigEntry<float> spreadConfig) {
            m_projectileAccuracy = attack.m_projectileAccuracy;
            m_projectileAccuracyMin = attack.m_projectileAccuracyMin;

            spreadConfig.SettingChanged += (sender, e) => {
                attack.m_projectileAccuracy = m_projectileAccuracy * spreadConfig.Value;
                attack.m_projectileAccuracyMin = m_projectileAccuracyMin * spreadConfig.Value;
            };

            attack.m_projectileAccuracy *= spreadConfig.Value;
            attack.m_projectileAccuracyMin *= spreadConfig.Value;
        }

        private static void SetLaunchAngle(Attack attack, ConfigEntry<float> angleConfig) {
            angleConfig.SettingChanged += (sender, e) => {
                attack.m_launchAngle = angleConfig.Value;
            };

            attack.m_launchAngle = angleConfig.Value;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Attack.GetProjectileSpawnPoint))]
        private static void GetProjectileSpawnPointPostfix(Attack __instance, ref Vector3 spawnPoint, ref Vector3 aimDir) {
            var weapon = GetPlayerWeapon(__instance);
            if (weapon == null) { return; }

            // I think that I just want to use the aimDir vector to rotate the offsets
            // and then change the spawn point based on the config
            Skills.SkillType skillType = weapon.m_shared.m_skillType;
            if (skillType == Skills.SkillType.Bows) {
                ApplyOffset(
                    ref spawnPoint,
                    ref aimDir,
                    ProjectileTweaks.BowVerticalOffset.Value,
                    ProjectileTweaks.BowHorizontalOffset.Value);
            }
            else if (skillType == Skills.SkillType.Crossbows) {
                ApplyOffset(
                    ref spawnPoint,
                    ref aimDir,
                    ProjectileTweaks.XbowVerticalOffset.Value,
                    ProjectileTweaks.XbowHorizontalOffset.Value);
            }
            else if (skillType == Skills.SkillType.Spears) {
                ApplyOffset(
                    ref spawnPoint,
                    ref aimDir,
                    ProjectileTweaks.SpearVerticalOffset.Value,
                    ProjectileTweaks.SpearHorizontalOffset.Value);
            }
            else if (skillType == Skills.SkillType.ElementalMagic) {
                ApplyOffset(
                    ref spawnPoint,
                    ref aimDir,
                    ProjectileTweaks.StaffVerticalOffset.Value,
                    ProjectileTweaks.StaffHorizontalOffset.Value);
            }
            else if (IsBomb(weapon)) {
                ApplyOffset(
                    ref spawnPoint,
                    ref aimDir,
                    ProjectileTweaks.BombVerticalOffset.Value,
                    ProjectileTweaks.BombHorizontalOffset.Value);
            }
        }

        private static void ApplyOffset(
            ref Vector3 spawnPoint,
            ref Vector3 aimDir,
            float vertOffset,
            float horzOffset
        ) {
            var horizontalOffset = new Vector3(aimDir.z, 0, -aimDir.x).normalized;
            horizontalOffset *= horzOffset;
            spawnPoint += horizontalOffset;
            spawnPoint += new Vector3(0.0f, vertOffset, 0.0f);
        }

        private static ItemDrop.ItemData GetPlayerWeapon(Attack attack) {
            if (attack == null ||
                attack.m_character is not Player player ||
                !(player.m_name == "Human" ||
                player == Player.m_localPlayer)) {
                return null;
            }

            return player.GetCurrentWeapon();
        }

        private static bool IsBomb(ItemDrop.ItemData item) {
            return item.m_shared.m_attack.m_attackAnimation == "throw_bomb";
        }
    }
}
