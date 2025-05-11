using System;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    protected RangeWeaponHandler targetHandler;
    public static WeaponUpgrade Instance { get; private set; }
    public class UpgradeOption
    {
        public string Id;
        public string Title;
        public string Description;
        public Sprite Icon;
        public Action OnSelected;

        public UpgradeOption(string title, string desc, Sprite icon, Action callback)
        {
            Title = title;
            Description = desc;
            Icon = icon;
            OnSelected = callback;
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if (targetHandler == null)
            targetHandler = GetComponentInChildren<RangeWeaponHandler>();
        if (targetHandler == null)
        {
            Debug.LogError("[RangeWeaponModifier] 대상 Handler를 찾을 수 없습니다.");
            enabled = false;
            return;
        }
    }
    //delay                         공격딜레이
    //weaponSize                    무기 크기
    //power                         데미지
    //speed                         탄속
    //attackRange                   자동공격범위(사용하지않음)
    //isOnKnockback                 넉백유무(bool)
    //knockbackPower                넉백강도
    //knockbackTime                 넉백시간
    //bulletIndex                   총알종류
    //bulletSize                    총알크기
    //duration                      사거리(총알수명)
    //spread                        탄퍼짐
    //numberofProjectiles           한번에 나가는 총알의 수
    //multipleProjectilesAngel      집탄률
    //projectileColor               총알의 색(투명도 체크 필요)

    public void TearUp()
    {
        if (targetHandler != null)
            targetHandler.Delay -= 0.1f;
    }
    public void TripleShot()
    {
        if (targetHandler != null)
            targetHandler.tripleShotEnabled = true;
    }
}
