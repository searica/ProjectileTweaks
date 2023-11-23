using HarmonyLib;

namespace ProjectileTweaks.Patches
{
    [HarmonyPatch(typeof(Humanoid))]
    internal static class HumanoidPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(nameof(Humanoid.GetAttackDrawPercentage))]
        private static void GetAttackDrawPercentagePrefix(Humanoid __instance, out float __state)
        {
            if (__instance && __instance as Player == Player.m_localPlayer)
            {
                ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
                if (currentWeapon != null && currentWeapon.m_shared.m_attack.m_bowDraw)
                {
                    __state = currentWeapon.m_shared.m_attack.m_drawDurationMin;
                    currentWeapon.m_shared.m_attack.m_drawDurationMin = __state * ProjectileTweaks.BowDrawSpeed.Value;
                    return;
                }
            }

            __state = -1f;
        }


        [HarmonyPostfix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(nameof(Humanoid.GetAttackDrawPercentage))]
        private static void GetAttackDrawPercentagePostfix(Humanoid __instance, ref float __state)
        {
            if (__state == -1f) { return; }

            ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
            if (currentWeapon != null && currentWeapon.m_shared.m_attack.m_bowDraw)
            {
                currentWeapon.m_shared.m_attack.m_drawDurationMin = __state;
            }
        }
    }
}
