using UnityEngine;
using UnityEngine.UI;
using static WeaponUpgrade;

public class UpgradeOptionUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descText;
    [SerializeField] private Button selectButton;

    private UpgradeOption _option;
    private System.Action _markSpecific;

    public void Initialize(UpgradeOption option, System.Action markSpecific)
    {
        _option = option;
        _markSpecific = markSpecific;   // 🔴 새로운 필드에 저장

        iconImage.sprite = option.Icon;
        titleText.text = option.Title;
        descText.text = option.Description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _markSpecific?.Invoke();
        _option.OnSelected?.Invoke();
        // 메뉴 닫기
        UpgradeMenu.Instance.CloseMenu();
    }
}
