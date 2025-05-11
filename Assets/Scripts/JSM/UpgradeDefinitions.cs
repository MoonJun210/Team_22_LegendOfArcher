using System.Collections.Generic;
using UnityEngine;
using static WeaponUpgrade;

public class UpgradeDefinitions : MonoBehaviour
{
    [Header("Icons (null 시 Default 사용)")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite rangeIcon;
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite piercingIcon;

    /// <summary>
    /// 정의된 모든 옵션을 리턴합니다.
    /// </summary>
    public List<UpgradeOption> GetAllOptions()
    {
        // 헬퍼 함수: 아이콘이 null이면 defaultIcon으로 대체
        Sprite IconOrDefault(Sprite spr) => spr != null ? spr : defaultIcon;

        return new List<UpgradeOption>()
        {
            new UpgradeOption(
                "사거리 증가",
                "원거리 무기의 사거리를 +2만큼 늘립니다.",
                IconOrDefault(rangeIcon),
                () => WeaponUpgrade.Instance.TearUp()
            ),
            new UpgradeOption(
                "연발 속도",
                "공격 딜레이를 -0.2초만큼 줄입니다.",
                IconOrDefault(speedIcon),
                () => WeaponUpgrade.Instance.TearUp()
            ),
            new UpgradeOption(
                "관통탄",
                "총알이 적을 관통하도록 만듭니다.",
                IconOrDefault(piercingIcon),
                () => WeaponUpgrade.Instance.TearUp()
            ),
            // 필요하면 더 추가…
        };
    }
    public List<UpgradeOption> GetOptionsByCategory(int category)
    {
        Sprite IconOrDefault(Sprite spr) => spr != null ? spr : defaultIcon;
        switch (category)
        {
            case 1:
                return new List<UpgradeOption>()
                {
                    new UpgradeOption(
                        "라이플",
                        "공격 딜레이를 -0.2초만큼 줄입니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.TearUp()
                    ),
                };
            case 2:
                return new List<UpgradeOption>()
                {
                    new UpgradeOption(
                        "샷건",
                        "공격 딜레이를 -0.2초만큼 줄입니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.TearUp()
                    ),
                };
            case 3:
                return new List<UpgradeOption>()
                {
                    new UpgradeOption(
                        "저격소총",
                        "공격 딜레이를 -0.2초만큼 줄입니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.TearUp()
                    ),
                };
            default:
                Debug.LogWarning($"[UpgradeDefinitions] 알 수 없는 category: {category}");
                return new List<UpgradeOption>();
        }
    }
}
