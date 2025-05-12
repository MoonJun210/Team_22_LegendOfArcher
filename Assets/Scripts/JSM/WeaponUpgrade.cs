using System;
using UnityEditor;
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


    // 수치들은 재조정 필요
    // 공용 업그레이드
    public void WU_TearUp()
    {
        if (targetHandler != null)
        {
            targetHandler.Delay *= 0.9f;
        }
    }
    public void WU_PowerUp()
    {
        if (targetHandler != null)
        {
            targetHandler.Power *= 1.2f;
        }
    }
    public void WU_SpeedUp()
    {
        if (targetHandler != null)
        {
            targetHandler.Speed *= 1.2f;
        }
    }
    public void WU_Rampage()
    {
        if (targetHandler != null)
        {
            targetHandler.Delay *= 0.7f;
            targetHandler.Spread *= 5f;
        }
    }

    //돌격소총 전용 - 데미지업 없이 연사만 증가하는 방향으로
    public void WU_TripleShot()
    {
        if (targetHandler != null)
        {
            targetHandler.TripleShotEnabled = true;
        }
    }
    public void WU_RunAndGun()
    {
        if (targetHandler != null)
        {
            targetHandler.Delay *= 0.9f;
            //이동속도 증가 추가필요
        }
    }

    //샷건 전용
    public void WU_HeavyBullet()
    {
        if (targetHandler != null)
        {
            targetHandler.Speed *= 0.8f;
            targetHandler.Power *= 1.2f;
            targetHandler.IsOnKnockback = true;
        }
    }
    public void WU_SlugBullet()
    {
        if (targetHandler != null)
        {
            targetHandler.MultipleProjectilesAngel *= 0f;
            targetHandler.Power *= 1.2f;
        }
    }

    //저격 전용 - 상승폭을 크게 주고 패널티
    public void WU_Steady()
    {
        if (targetHandler != null)
        {
            targetHandler.Power *= 1.5f;
            //이동속도 감소 추가필요
        }
    }
    public void WU_Waiting()
    {
        if (targetHandler != null)
        {
            targetHandler.Power *= 2.0f;
            targetHandler.Delay *= 1.5f;
        }
    }
    //특수능력
    public void ActiveDoubleWeapon()
    {
        if (targetHandler != null)
        {
            targetHandler.NumberofProjectilesPerShot *= 2;
        }
    }
    public void InactiveDoubleWeapon()
    {
        if (targetHandler != null)
        {
            targetHandler.NumberofProjectilesPerShot /= 2;
        }
    }
}
