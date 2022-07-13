using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapleUtility.Plugins.Models
{
    public class CharacterItem : Notifier
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public bool IsCaptured
        {
            get
            {
                if (MainCaptureBlock == null)
                    return false;
                else
                    return true;
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private BlockItem mainCaptureBlock;
        public BlockItem MainCaptureBlock
        {
            get { return mainCaptureBlock; }
            set
            {
                if (mainCaptureBlock != null)
                    mainCaptureBlock.MainCharacterItem = null;

                mainCaptureBlock = value;

                DrawBlock();
                OnPropertyChanged("MainCaptureBlock");
                OnPropertyChanged("IsCaptured");
            }
        }

        private List<BlockItem> captureBlockList = new List<BlockItem>();
        public List<BlockItem> CaptureBlockList
        {
            get { return captureBlockList; }
            set
            {
                if(captureBlockList != null)
                {
                    foreach (var block in CaptureBlockList)
                    {
                        block.CapturedCharacters.Remove(this);
                        block.NotifyBackgroundColor();
                    }
                }

                captureBlockList = value;

                if(captureBlockList != null)
                {
                    foreach (var block in CaptureBlockList)
                    {
                        block.CapturedCharacters.Add(this);
                        block.NotifyBackgroundColor();
                    }
                }

                OnPropertyChanged("CaptureBlockList");
            }
        }


        private int angle;
        public int Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                if (angle < 0)
                    angle += 360;
                else if (angle > 360)
                    angle -= 360;

                if(MainCaptureBlock != null)
                    CaptureBlockList = CharacterBlockHelper.GetCapturedBlocks(Type, Job, Level, MainCaptureBlock, Angle);

                OnPropertyChanged("Angle");
            }
        }

        public int BlockCount
        {
            get { return CharacterBlockHelper.GetCaptureBlockCount(Type, Job, Level); }
        }

        public CharacterType Type
        {
            get
            {
                return GetCharacterType(Job);
            }
        }

        public CharacterPassiveType PassiveType
        {
            get
            {
                return GetCharacterPassiveType(Job);
            }
        }

        public int CharacterPassiveValue
        {
            get
            {
                return GetCharacterPassiveValue(PassiveType, Level);
            }
        }

        public string TypeString
        {
            get
            {
                switch(Type)
                {
                    case CharacterType.Warrior:
                        return "전사";
                    case CharacterType.Wizard:
                        return "마법사";
                    case CharacterType.Archer:
                        return "궁수";
                    case CharacterType.Thief:
                        return "도적";
                    case CharacterType.Pirate:
                        return "해적";
                    case CharacterType.Hybrid:
                        return "제논";
                    case CharacterType.MapleMobile:
                        return "메이플M";
                    default:
                        return "없음";
                }
            }
        }

        private string job;
        public string Job
        {
            get { return job; }
            set
            {
                // 2차, 3차 변환-_-;;
                string convertValue = value;
                switch(value)
                {
                    case "파이터":
                    case "크루세이더":
                        convertValue = "히어로";
                        break;
                    case "페이지":
                    case "나이트":
                        convertValue = "팔라딘";
                        break;
                    case "스피어맨":
                    case "버서커":
                        convertValue = "다크나이트";
                        break;
                    case "위자드(불,독)":
                    case "메이지(불,독)":
                        convertValue = "아크메이지(불,독)";
                        break;
                    case "위자드(썬,콜)":
                    case "메이지(썬,콜)":
                        convertValue = "아크메이지(썬,콜)";
                        break;
                    case "클레릭":
                    case "프리스트":
                        convertValue = "비숍";
                        break;
                    case "헌터":
                    case "레인저":
                        convertValue = "보우마스터";
                        break;
                    case "사수":
                    case "저격수":
                        convertValue = "신궁";
                        break;
                    case "에인션트아처":
                    case "에인션트 아처":
                    case "체이서":
                        convertValue = "패스파인더";
                        break;
                    case "시프":
                    case "시프마스터":
                        convertValue = "섀도어";
                        break;
                    case "어쌔신":
                    case "허밋":
                        convertValue = "나이트로드";
                        break;
                    case "듀어러":
                    case "듀얼마스터":
                    case "슬래셔":
                    case "듀얼블레이더":
                        convertValue = "듀얼블레이드";
                        break;
                    case "인파이터":
                    case "버커니어":
                        convertValue = "바이퍼";
                        break;
                    case "건슬링거":
                    case "발키리":
                        convertValue = "캡틴";
                        break;
                    case "캐논블래스터":
                    case "캐논마스터":
                        convertValue = "캐논슈터";
                        break;
                    default:
                        break;
                }

                job = convertValue;

                if(MainCaptureBlock != null)
                    CaptureBlockList = CharacterBlockHelper.GetCapturedBlocks(Type, Job, Level, MainCaptureBlock, Angle);

                if (Type == CharacterType.MapleMobile)
                {
                    if (Level > 200)
                        Level = 200;
                }
                else
                {
                    if (Level < 60)
                        Level = 60;
                }

                OnPropertyChanged("Job");
                OnPropertyChanged("Type");
                OnPropertyChanged("TypeString");
            }
        }

        private int level;
        public int Level
        {
            get { return level; }
            set
            {
                if(Type == CharacterType.MapleMobile)
                {
                    if (value >= 30 && value <= 200)
                    {
                        level = value;
                        if (MainCaptureBlock != null)
                            CaptureBlockList = CharacterBlockHelper.GetCapturedBlocks(Type, Job, Level, MainCaptureBlock, Angle);
                    }
                }
                else
                {
                    if (value >= 60 && value <= 275)
                    {
                        level = value;
                        if (MainCaptureBlock != null)
                            CaptureBlockList = CharacterBlockHelper.GetCapturedBlocks(Type, Job, Level, MainCaptureBlock, Angle);
                    }
                }
                OnPropertyChanged("Level");
                OnPropertyChanged("MainCaptureBlock");
            }
        }

        public void DrawBlock()
        {
            if (MainCaptureBlock != null)
            {
                MainCaptureBlock.MainCharacterItem = this;
                CaptureBlockList = CharacterBlockHelper.GetCapturedBlocks(Type, Job, Level, MainCaptureBlock, Angle);
            }
            else
                CaptureBlockList = null;
        }

        public CharacterType GetCharacterType(string Job)
        {
            switch (Job)
            {
                case "전사":
                case "히어로":
                case "팔라딘":
                case "다크나이트":
                case "소울마스터":
                case "미하일":
                case "블래스터":
                case "데몬슬레이어":
                case "데몬어벤져":
                case "아란":
                case "카이저":
                case "아델":
                case "제로":
                    return CharacterType.Warrior;
                case "마법사":
                case "아크메이지(불,독)":
                case "아크메이지(썬,콜)":
                case "비숍":
                case "플레임위자드":
                case "배틀메이지":
                case "에반":
                case "루미너스":
                case "일리움":
                case "키네시스":
                    return CharacterType.Wizard;
                case "궁수":
                case "보우마스터":
                case "신궁":
                case "패스파인더":
                case "윈드브레이커":
                case "와일드헌터":
                case "메르세데스":
                    return CharacterType.Archer;
                case "도적":
                case "나이트로드":
                case "섀도어":
                case "듀얼블레이드":
                case "나이트워커":
                case "팬텀":
                case "카데나":
                case "호영":
                    return CharacterType.Thief;
                case "해적":
                case "바이퍼":
                case "캡틴":
                case "캐논슈터":
                case "스트라이커":
                case "메카닉":
                case "은월":
                case "엔젤릭버스터":
                case "아크":
                    return CharacterType.Pirate;
                case "제논":
                    return CharacterType.Hybrid;
                case "메이플M":
                    return CharacterType.MapleMobile;
                default:
                    return CharacterType.None;
            }
        }

        public CharacterPassiveType GetCharacterPassiveType(string Job)
        {
            switch (Job)
            {
                case "히어로":
                case "팔라딘":
                case "바이퍼":
                case "캐논슈터":
                case "스트라이커":
                case "카이저":
                case "아델":
                case "아크":
                    return CharacterPassiveType.STR;
                case "아크메이지(썬,콜)":
                case "비숍":
                case "배틀메이지":
                case "루미너스":
                case "플레임위자드":
                case "키네시스":
                case "일리움":
                    return CharacterPassiveType.INT;
                case "보우마스터":
                case "패스파인더":
                case "윈드브레이커":
                case "엔젤릭버스터":
                    return CharacterPassiveType.DEX;
                case "섀도어":
                case "듀얼블레이드":
                case "나이트워커":
                case "카데나":
                case "호영":
                    return CharacterPassiveType.LUK;
                case "신궁":
                case "나이트로드":
                    return CharacterPassiveType.CriticalPercentage;
                case "다크나이트":
                    return CharacterPassiveType.HPPercentage;
                case "아크메이지(불,독)":
                    return CharacterPassiveType.MPPercentage;
                case "캡틴":
                    return CharacterPassiveType.SummonCreatureTime;
                case "블래스터":
                    return CharacterPassiveType.ArmorPenetration;
                case "와일드헌터":
                    return CharacterPassiveType.AttackDamagePercentageIncrease;
                case "메카닉":
                    return CharacterPassiveType.BuffTime;
                case "제논":
                    return CharacterPassiveType.STR_DEX_LUK;
                case "데몬슬레이어":
                    return CharacterPassiveType.CrowdControlImmune;
                case "데몬어벤져":
                    return CharacterPassiveType.BossDamage;
                case "아란":
                    return CharacterPassiveType.AttackHPPercentageHeal;
                case "에반":
                    return CharacterPassiveType.AttackMPPercentageHeal;
                case "메르세데스":
                    return CharacterPassiveType.SkillCoolTime;
                case "팬텀":
                    return CharacterPassiveType.MesoIncrease;
                case "은월":
                    return CharacterPassiveType.CriticalDamage;
                case "소울마스터":
                case "미하일":
                    return CharacterPassiveType.HPValue;
                case "제로":
                    return CharacterPassiveType.EXP;
                case "메이플M":
                    return CharacterPassiveType.AttackAndMagic;
                default:
                    return CharacterPassiveType.None;
            }
        }

        public int GetCharacterPassiveValue(CharacterPassiveType passiveType, int Value)
        {
            switch (passiveType)
            {
                case CharacterPassiveType.STR:
                case CharacterPassiveType.INT:
                case CharacterPassiveType.DEX:
                case CharacterPassiveType.LUK:
                    if (level >= 250)
                        return 100;
                    if (level >= 200)
                        return 80;
                    if (level >= 140)
                        return 40;
                    if (level >= 100)
                        return 20;
                    else
                        return 10;
                case CharacterPassiveType.HPPercentage:
                case CharacterPassiveType.MPPercentage:
                case CharacterPassiveType.SkillCoolTime:
                    if (level >= 250)
                        return 6;
                    if (level >= 200)
                        return 5;
                    if (level >= 140)
                        return 4;
                    if (level >= 100)
                        return 3;
                    else
                        return 2;
                case CharacterPassiveType.CriticalPercentage:
                case CharacterPassiveType.MesoIncrease:
                case CharacterPassiveType.CrowdControlImmune:
                    if (level >= 250)
                        return 5;
                    if (level >= 200)
                        return 4;
                    if (level >= 140)
                        return 3;
                    if (level >= 100)
                        return 2;
                    else
                        return 1;
                case CharacterPassiveType.SummonCreatureTime:
                case CharacterPassiveType.EXP:
                    if (level >= 250)
                        return 12;
                    if (level >= 200)
                        return 10;
                    if (level >= 140)
                        return 8;
                    if (level >= 100)
                        return 6;
                    else
                        return 4;
                case CharacterPassiveType.ArmorPenetration:
                case CharacterPassiveType.CriticalDamage:
                case CharacterPassiveType.BossDamage:
                    if (level >= 250)
                        return 6;
                    if (level >= 200)
                        return 5;
                    if (level >= 140)
                        return 3;
                    if (level >= 100)
                        return 2;
                    else
                        return 1;
                case CharacterPassiveType.AttackDamagePercentageIncrease:
                    if (level >= 250)
                        return 20;
                    if (level >= 200)
                        return 16;
                    if (level >= 140)
                        return 12;
                    if (level >= 100)
                        return 8;
                    else
                        return 4;
                case CharacterPassiveType.BuffTime:
                    if (level >= 250)
                        return 25;
                    if (level >= 200)
                        return 20;
                    if (level >= 140)
                        return 15;
                    if (level >= 100)
                        return 10;
                    else
                        return 5;
                case CharacterPassiveType.STR_DEX_LUK:
                    if (level >= 250)
                        return 50;
                    if (level >= 200)
                        return 40;
                    if (level >= 140)
                        return 20;
                    if (level >= 100)
                        return 10;
                    else
                        return 5;
                case CharacterPassiveType.AttackHPPercentageHeal:
                case CharacterPassiveType.AttackMPPercentageHeal:
                    if (level >= 250)
                        return 10;
                    if (level >= 200)
                        return 8;
                    if (level >= 140)
                        return 6;
                    if (level >= 100)
                        return 4;
                    else
                        return 2;
                case CharacterPassiveType.HPValue:
                    if (level >= 250)
                        return 2500;
                    if (level >= 200)
                        return 2000;
                    if (level >= 140)
                        return 1000;
                    if (level >= 100)
                        return 500;
                    else
                        return 250;
            }

            return 0;
        }
    }
}
