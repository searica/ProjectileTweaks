using HarmonyLib;

namespace ProjectileTweaks.Patches;

[HarmonyPatch(typeof(ItemDrop.ItemData))]
internal static class ItemDataPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(nameof(ItemDrop.ItemData.GetWeaponLoadingTime))]
    public static void GetWeaponLoadingTimePrefix(ItemDrop.ItemData __instance, out float __state)
    {
        if (__instance.m_shared.m_attack.m_requiresReload)
        {
            __state = __instance.m_shared.m_attack.m_reloadTime;
            __instance.m_shared.m_attack.m_reloadTime *= (1 / ProjectileTweaks.XBowReloadSpeed.Value);
            return;
        }

        __state = -1f;
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(ItemDrop.ItemData.GetWeaponLoadingTime))]
    public static void GetWeaponLoadingTimePostfix(ItemDrop.ItemData __instance, ref float __state)
    {
        if (__instance.m_shared.m_attack.m_requiresReload && __state != -1f)
        {
            __instance.m_shared.m_attack.m_reloadTime = __state;
        }
    }
}
