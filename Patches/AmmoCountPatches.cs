using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemDrop;
using Jotunn.Managers;

namespace ProjectileTweaks.Patches;

[HarmonyPatch]
internal static class AmmoCountPatches
{
    private const string FontName = "Valheim-AveriaSansLibre";
    private const FontStyles FontStyle = FontStyles.Normal;
    private static TMP_FontAsset _CurrentFont = null;
    private const string AmmoCountName = "AmmoCount";
    private const string AmmoTextName = "AmmoText";
    private const string AmmoIconName = "AmmoIcon";

    private static TMP_FontAsset CurrentFont
    {
        get
        {
            if (_CurrentFont is null)
            {
                _CurrentFont = GUIManager.Instance.TMP_AveriaSansLibre;  
            }
            return _CurrentFont;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HotkeyBar), nameof(HotkeyBar.Update))]
    private static void UpdateHotkeyBarAmmoCounter(HotkeyBar __instance)
    {
        // wait until GUIManager is intialized and the correct font has been retrieved
        if (!__instance || !Player.m_localPlayer || CurrentFont is null) 
        {
            return;
        }

        for (int i = 0; i < __instance.m_items.Count(); i++)
        {
            ItemData itemData = __instance.m_items[i];
            int x = itemData.m_gridPos.x;
            if (x < 0 || x >= __instance.m_elements.Count() || __instance.m_elements[x] == null)
            {
                continue;  // out of range
            }
            
            HotkeyBar.ElementData el = __instance.m_elements[x];
            GameObject elGameObject = el.m_go;
            Transform ammoCount = elGameObject.transform.Find(AmmoCountName);

            if (ammoCount && (ProjectileTweaks.ShouldResetIcon || el.m_used))
            {
                // Destory ammo counter and recreate it if needed
                GameObject.DestroyImmediate(ammoCount.gameObject);
                AddAmmoCounter(elGameObject, out ammoCount);
            }
            else if (!ammoCount)  // create ammo count UI element if needed
            {
                AddAmmoCounter(elGameObject, out ammoCount);
            }

            CheckAmmoCount(itemData, ammoCount.gameObject);
        }
        ProjectileTweaks.ShouldResetIcon = false;
    }


    /// <summary>
    ///     Add ammo counter UI element
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="ammoCount"></param>
    private static void AddAmmoCounter(GameObject gameObject, out Transform ammoCount)
    {
        // Create transform
        GameObject ammoCountGameObject = new(AmmoCountName);
        ammoCountGameObject.SetActive(false);
        ammoCountGameObject.transform.SetParent(gameObject.transform);
        ammoCountGameObject.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ammoCount = ammoCountGameObject.transform;

        // Add text object
        GameObject textGameObject = new(AmmoTextName);
        textGameObject.SetActive(false);

        textGameObject.transform.SetParent(ammoCountGameObject.transform);
        TextMeshProUGUI textMeshProUGUI = textGameObject.AddComponent<TextMeshProUGUI>();

        // Set text location
        RectTransform rectTransform = textMeshProUGUI.rectTransform;
        rectTransform.anchoredPosition = ProjectileTweaks.Instance.AmmoTextPosition.Value;
        rectTransform.sizeDelta = new Vector2(90f, 90f);

        // Set text
        textMeshProUGUI.font = CurrentFont;
        textMeshProUGUI.fontSize = ProjectileTweaks.Instance.AmmoTextSize.Value;
        textMeshProUGUI.fontStyle = FontStyle;
        textMeshProUGUI.alignment = ProjectileTweaks.Instance.AmmoTextAlignment.Value;
        textMeshProUGUI.color = ProjectileTweaks.Instance.AmmoTextColor.Value;

        // Enable object after setting up text
        textGameObject.SetActive(true);

        // set up icon
        if (ProjectileTweaks.Instance.ShowAmmoIcon.Value)
        {
            GameObject iconGameObject = new(AmmoIconName);
            iconGameObject.transform.SetParent(ammoCountGameObject.transform);
            iconGameObject.AddComponent<Image>();
            RectTransform iconRectTransform = iconGameObject.GetComponent<RectTransform>();
            iconRectTransform.anchoredPosition = ProjectileTweaks.Instance.AmmoIconPosition.Value;
            iconRectTransform.sizeDelta = ProjectileTweaks.Instance.AmmoIconSize.Value;
        }
    }

    /// <summary>
    ///     Check and set ammo count/icon if it has ammo.
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="go"></param>
    private static void CheckAmmoCount(ItemData itemData, GameObject go)
    {
        if (!TryGetAmmo(itemData, out ItemData ammoItemData))
        {
            go.SetActive(value: false);
            return;
        }

        go.SetActive(true);
        go.transform.Find(AmmoTextName).GetComponent<TextMeshProUGUI>().text = ammoItemData.m_stack.ToString();
        if (ProjectileTweaks.Instance.ShowAmmoIcon.Value)
        {
            go.transform.Find(AmmoIconName).GetComponent<Image>().sprite = ammoItemData.GetIcon();
        }
    }

    /// <summary>
    ///     Try get ammo item data for weapon.
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns></returns>
    private static bool TryGetAmmo(ItemData weapon, out ItemData ammoItemData)
    {
        ammoItemData = null;
        if (string.IsNullOrEmpty(weapon.m_shared.m_ammoType) || weapon.m_shared.m_itemType == ItemData.ItemType.Ammo)
        {
            return false;
        }
        
        ammoItemData = Player.m_localPlayer.GetAmmoItem();
        if (ammoItemData is not null && (!Player.m_localPlayer.GetInventory().ContainsItem(ammoItemData) || ammoItemData.m_shared.m_ammoType != weapon.m_shared.m_ammoType))
        {
            ammoItemData = null;
        }
        ammoItemData ??= Player.m_localPlayer.GetInventory().GetAmmoItem(weapon.m_shared.m_ammoType);

        return ammoItemData is not null;
    }


    
}
