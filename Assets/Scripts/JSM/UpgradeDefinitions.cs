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

    public List<UpgradeOption> GetAllOptions()
    {
        Sprite IconOrDefault(Sprite spr) => spr != null ? spr : defaultIcon;

        return new List<UpgradeOption>()
        {
            new UpgradeOption(
                "데미지 증가",
                "무기의 데미지를 올립니다.",
                IconOrDefault(rangeIcon),
                () => WeaponUpgrade.Instance.WU_PowerUp()
            ),
            new UpgradeOption(
                "연사 증가",
                "공격 사이의 간격을 줄입니다.",
                IconOrDefault(speedIcon),
                () => WeaponUpgrade.Instance.WU_TearUp()
            ),
            new UpgradeOption(
                "고속탄",
                "총알이 더 빨라집니다.",
                IconOrDefault(piercingIcon),
                () => WeaponUpgrade.Instance.WU_SpeedUp()
            ),
            new UpgradeOption(
                "난사",
                "연사가 증가하지만, 더 부정확하게 발사됩니다.",
                IconOrDefault(piercingIcon),
                () => WeaponUpgrade.Instance.WU_Rampage()
            ),
            new UpgradeOption(
                "이속 증가",
                "이동속도가 증가합니다.",
                IconOrDefault(piercingIcon),
                () => WeaponUpgrade.Instance.WU_RunningShoes()
            )
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
                        "조정간 점사",
                        "공격방식이 3점사가 됩니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_TripleShot()
                    ),
                    new UpgradeOption(
                        "런앤건",
                        "이동속도와 연사속도가 증가합니다",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_RunAndGun()
                    ),
                };
            case 2:
                return new List<UpgradeOption>()
                {
                    new UpgradeOption(
                        "침착함",
                        "이동속도를 희생하여 데미지를 높입니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_Steady()
                    ),
                    new UpgradeOption(
                        "기다림의 미학",
                        "공격속도를 희생하여 데미지를 높입니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_Waiting()
                    ),
                };
            case 3:
                return new List<UpgradeOption>()
                {
                    new UpgradeOption(
                        "뚱뚱한 총탄",
                        "탄속이 느려지지만 데미지가 올라갑니다.",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_HeavyBullet()
                    ),
                    new UpgradeOption(
                        "유사 슬러그",
                        "집탄율과 데미지가 더 올라갑니다",
                        IconOrDefault(speedIcon),
                        () => WeaponUpgrade.Instance.WU_SlugBullet()
                    ),
                };
            default:
                Debug.LogWarning($"[UpgradeDefinitions] 알 수 없는 category: {category}");
                return new List<UpgradeOption>();
        }
    }
}
