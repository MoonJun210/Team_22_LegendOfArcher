using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : RangeWeaponModifier
{
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

    public void RangeUp()
    {
        ModifyField("range", '+', 1.5f);
    }
    public void TearUp()
    {
        ModifyField("delay", '-', 0.1f);
    }
}
