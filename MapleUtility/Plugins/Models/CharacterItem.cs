using MapleUtility.Plugins.Common;
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

        public CharacterType Type
        {
            get
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
                if(value >= 60 && value <= 275)
                    level = value;
                OnPropertyChanged("Level");
            }
        }
    }
}
