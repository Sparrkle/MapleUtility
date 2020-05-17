using System.ComponentModel;

namespace MapleUtility.Plugins.Common
{
    public enum ProgressType
    {
        [Description("Start Progress")]
        START,
        [Description("Update Progress")]
        UPDATE,
        [Description("End Progress")]
        END
    }

    public enum CharacterType
    {
        [Description("무관")]
        None,
        [Description("전사")]
        Warrior,
        [Description("궁수")]
        Archer,
        [Description("마법사")]
        Wizard,
        [Description("도적")]
        Thief,
        [Description("해적")]
        Pirate,
        [Description("하이브리드")]
        Hybrid,
        [Description("메이플M")]
        MapleMobile,
    }

    // 공격대원 효과
    public enum CharacterPassiveType
    {
        [Description("STR 증가")]
        STR,
        [Description("DEX 증가")]
        DEX,
        [Description("INT 증가")]
        INT,
        [Description("LUK 증가")]
        LUK,
        [Description("최대 HP 증가 (퍼센트)")]
        HPPercentage,
        [Description("최대 HP 증가 (수치)")]
        HPValue,
        [Description("최대 MP 증가")]
        MP,
        [Description("크리티컬 확률 증가")]
        CriticalPercentage,
        [Description("소환수 지속 시간 증가")]
        SummonCreatureTime,
        [Description("방어율 무시 증가")]
        ArmorPenetration,
        [Description("공격시 20% 확률로 데미지 증가")]
        AttackDamagePercentageIncrease,
        [Description("상태이상 저항 증가")]
        CrowdControlImmune,
        [Description("보스 공격력 증가")]
        BossDamage,
        [Description("타격 성공 시 70% 확률로 최대 HP 회복")]
        AttackHPPercentageHeal,
        [Description("타격 성공 시 70% 확률로 최대 MP 회복")]
        AttackMPPercentageHeal,
        [Description("스킬 재사용 대기시간 감소")]
        SkillCoolTime,
        [Description("메소 획득량 증가")]
        MesoIncrease,
        [Description("크리티컬 데미지 증가")]
        CriticalDamage,
        [Description("경험치 획득량 증가")]
        EXP,
        [Description("공격력 / 마력 증가")]
        AttackAndMagic,
    }

    public enum UnionCaptureType
    {
        [Description("STR 증가")]
        STR,
        [Description("DEX 증가")]
        DEX,
        [Description("INT 증가")]
        INT,
        [Description("LUK 증가")]
        LUK,
        [Description("HP 증가")]
        HP,
        [Description("MP 증가")]
        MP,
        [Description("공격력 증가")]
        AttackPoint,
        [Description("마력 증가")]
        MagicAttack,
        [Description("상태이상 내성 증가")]
        CrowControlImmune,
        [Description("획득 경험치 증가")]
        EXP,
        [Description("크리티컬 확률 증가")]
        CriticalPercentage,
        [Description("보스 데미지 증가")]
        BossDamage,
        [Description("스탠스 증가")]
        Stance,
        [Description("버프 지속 시간 증가")]
        BuffTime,
        [Description("방어율 무시 증가")]
        ArmorPenetration,
        [Description("크리티컬 데미지 증가")]
        CriticalDamage,
    }
}
