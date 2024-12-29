
using HarmonyLib;
using UnityEngine;

namespace ProjectileTweaks;

[HarmonyPatch]
internal static class ZoomManager
{
    internal enum ZoomState
    {
        Fixed,
        ZoomingIn,
        ZoomingOut
    }

    private static ZoomState CurrentZoomState;

    private static float ZoomInTimer = 0f;

    private static float ZoomOutTimer;

    private static float ZoomOutDelayTimer = 0f;

    private static float BaseFov;

    private static float LastZoomFov;

    private static float NewZoomFov = 0f;

    private const float DiffTol = 0.1f;
    private const float PercTol = 0.01f;
    private const float ZoomOutDuration = 1f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.UpdateCamera))]
    [HarmonyPriority(0)]
    private static void UpdateCameraPrefix(GameCamera __instance)
    {
        if (ProjectileTweaks.Instance.IsZoomEnabled && (CurrentZoomState == ZoomState.ZoomingOut || CurrentZoomState == ZoomState.ZoomingIn))
        {
            __instance.m_fov = NewZoomFov;
        }
    }

    /// <summary>
    ///     Check for un-zoomed FoV so it can be reset correctly. Also check if
    ///     the base un-zoomed GameCamera FoV has changed since it was last stored.
    ///     (Mods like CameraTweaks can change default FoV)
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.UpdateCamera))]
    [HarmonyPriority(Priority.Low)]
    private static void UpdateCameraPostfix(GameCamera __instance)
    {
        if (__instance != null &&
            ProjectileTweaks.Instance.IsZoomEnabled &&
            CurrentZoomState == ZoomState.Fixed &&
            Mathf.Abs(__instance.m_fov - BaseFov) > DiffTol)
        {
            BaseFov = __instance.m_fov;
        }

    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.SetControls))]
    private static void SetControlsPrefix(Player __instance, ref bool attackHold, ref bool blockHold)
    {
        if (!ProjectileTweaks.Instance.IsZoomEnabled)
        {
            return;
        }

        if (attackHold) // use to cancel bow draw
        {
            blockHold = Input.GetKey(ProjectileTweaks.Instance.CancelDrawKey.Value);
            return;
        }

        // prevent blocking while zooming in with zoomable item
        if (CurrentZoomState == ZoomState.ZoomingIn && HasZoomableItem(__instance))
        {
            blockHold = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateCrosshair))]
    [HarmonyPriority(0)]
    private static void UpdateCrosshairPostfix(Player player, float bowDrawPercentage)
    {
        if (!ProjectileTweaks.Instance.IsZoomEnabled || BaseFov == 0.0f)
        {
            return;
        }

        if (!HasZoomableItem(player))
        {
            ZoomOut();
            return;
        }

        bool isKeyPressed = Input.GetKey(ProjectileTweaks.Instance.ZoomKey.Value);
        bool shouldZoomBow = (isKeyPressed || ProjectileTweaks.Instance.AutoBowZoom.Value) && bowDrawPercentage > PercTol;
        bool shouldZoomXbow = isKeyPressed && player.IsWeaponLoaded();

        if (shouldZoomBow || shouldZoomXbow)
        {
            ZoomIn();
        }
        else if (isKeyPressed && bowDrawPercentage <= PercTol)
        {
            ProcessZoomOutDelay();
        }
        else
        {
            ZoomOut();
        }
    }

    internal static void ZoomIn()
    {
        ZoomInTimer += Time.deltaTime;
        CurrentZoomState = ZoomState.ZoomingIn;

        float t = Mathf.InverseLerp(0.05f, ProjectileTweaks.Instance.TimeToZoomIn.Value, ZoomInTimer);
        LastZoomFov = Mathf.Lerp(BaseFov, BaseFov / ProjectileTweaks.Instance.ZoomFactor.Value, t);
        GameCamera.instance.m_fov = LastZoomFov;
        NewZoomFov = LastZoomFov;
    }

    public static void ZoomOut()
    {
        if (CurrentZoomState != ZoomState.Fixed)
        {
            if (CurrentZoomState == ZoomState.ZoomingIn)
            {
                CurrentZoomState = ZoomState.ZoomingOut;
                ZoomOutTimer = 0f;
                ZoomInTimer = 0f;
            }
            else
            {
                ZoomOutTimer += Time.deltaTime;
                if (ZoomOutTimer > ZoomOutDuration)
                {
                    GameCamera.instance.m_fov = BaseFov;
                    CurrentZoomState = ZoomState.Fixed;
                    ZoomOutDelayTimer = 0f;
                    return;
                }
            }
            float t = Mathf.InverseLerp(0f, 0.3f, ZoomOutTimer);
            NewZoomFov = Mathf.Lerp(LastZoomFov, BaseFov, t);
            GameCamera.instance.m_fov = NewZoomFov;
        }
        else if (Mathf.Abs(GameCamera.instance.m_fov - BaseFov) >= DiffTol)
        {
            GameCamera.instance.m_fov = BaseFov;
        }
    }

    internal static void ProcessZoomOutDelay()
    {
        ZoomOutDelayTimer += Time.deltaTime;
        if (ZoomOutDelayTimer > ProjectileTweaks.Instance.StayInZoomTime.Value)
        {
            ZoomOut();
        }
    }

    /// <summary>
    ///     Check if current when is zoomable and zoom for that weapon is enabled.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    internal static bool HasZoomableItem(Player player)
    {
        ItemDrop.ItemData leftItem = player.GetCurrentWeapon();
        if (leftItem != null)
        {
            Skills.SkillType itemSkill = leftItem.m_shared.m_skillType;
            bool isZoomableBow = ProjectileTweaks.Instance.EnableBowZoom.Value && itemSkill == Skills.SkillType.Bows;
            bool isZoomableXbow = ProjectileTweaks.Instance.EnableXBowZoom.Value && itemSkill == Skills.SkillType.Crossbows;
            return isZoomableBow || isZoomableXbow;
        }

        return false;
    }
}
