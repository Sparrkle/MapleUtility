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

    public enum SortType
    {
        [Description("남은 시간 오름차순")]
        RemainTime,
        [Description("남은 시간 내림차순")]
        RemainTimeDesc,
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
        [Description("없음")]
        None,
        [Description("STR {0} 증가")]
        STR,
        [Description("DEX {0} 증가")]
        DEX,
        [Description("INT {0} 증가")]
        INT,
        [Description("LUK {0} 증가")]
        LUK,
        [Description("STR, DEX, LUK {0} 증가")]
        STR_DEX_LUK,
        [Description("최대 HP {0}% 증가")]
        HPPercentage,
        [Description("최대 HP {0} 증가")]
        HPValue,
        [Description("최대 MP {0}% 증가")]
        MPPercentage,
        [Description("크리티컬 확률 {0}% 증가")]
        CriticalPercentage,
        [Description("소환수 지속 시간 {0}% 증가")]
        SummonCreatureTime,
        [Description("방어율 무시 {0}% 증가")]
        ArmorPenetration,
        [Description("공격시 20% 확률로 데미지 {0}% 증가")]
        AttackDamagePercentageIncrease,
        [Description("상태이상 저항 증가")]
        CrowdControlImmune,
        [Description("보스 공격력 {0}% 증가")]
        BossDamage,
        [Description("적 공격마다 70% 확률로 순수 HP의 {0}% 회복")]
        AttackHPPercentageHeal,
        [Description("적 공격마다 70% 확률로 순수 MP의 {0}% 회복")]
        AttackMPPercentageHeal,
        [Description("스킬 재사용 대기시간 {0}% 감소")]
        SkillCoolTime,
        [Description("메소 획득량 {0}% 증가")]
        MesoIncrease,
        [Description("크리티컬 데미지 {0}% 증가")]
        CriticalDamage,
        [Description("경험치 획득량 {0}% 증가")]
        EXP,
        [Description("공격력/마력 {0}% 증가")]
        AttackAndMagic,
        [Description("버프 지속시간 {0}% 증가")]
        BuffTime,
    }

    public enum UnionCaptureType
    {
        [Description("없음")]
        None,
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
        [Description("1시 스텟 유니온")]
        Variable1,
        [Description("2시 스텟 유니온")]
        Variable2,
        [Description("4시 스텟 유니온")]
        Variable4,
        [Description("5시 스텟 유니온")]
        Variable5,
        [Description("7시 스텟 유니온")]
        Variable7,
        [Description("8시 스텟 유니온")]
        Variable8,
        [Description("10시 스텟 유니온")]
        Variable10,
        [Description("11시 스텟 유니온")]
        Variable11,
    }
}
