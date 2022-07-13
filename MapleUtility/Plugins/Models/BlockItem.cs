using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MapleUtility.Plugins.Models
{
    public class BlockItem : Notifier
    {
        public int F { get { return G + H; } }
        public int G
        {
            get
            {
                if (Parent == null)
                    return 0;

                return Parent.G + 1 - (CaptureType == PrioirtyItem.CaptureEnum ? 1 : 0);
            }
        } // Start ~ Current

        public int H { get; private set; } // Current ~ End

        public Color BackgroundColor
        {
            get
            {
                if (CapturedCharacters.Count() == 0)
                    return Color.FromArgb(255, 21, 21, 21);

                Color color = Color.FromArgb(0, 0, 0, 0);
                foreach (var character in CapturedCharacters)
                {
                    switch (character.Type)
                    {
                        case CharacterType.Warrior:
                            color = Color.Add(color, Color.FromArgb(128, 170, 34, 68));
                            break;
                        case CharacterType.Wizard:
                            color = Color.Add(color, Color.FromArgb(128, 34, 136, 170));
                            break;
                        case CharacterType.Archer:
                            color = Color.Add(color, Color.FromArgb(128, 102, 153, 51));
                            break;
                        case CharacterType.Thief:
                            color = Color.Add(color, Color.FromArgb(128, 102, 68, 204));
                            break;
                        case CharacterType.Pirate:
                            color = Color.Add(color, Color.FromArgb(128, 102, 102, 102));
                            break;
                        case CharacterType.Hybrid:
                            color = Color.Add(color, Color.FromArgb(128, 112, 103, 137));
                            break;
                        case CharacterType.MapleMobile:
                            color = Color.Add(color, Color.FromArgb(128, 204, 85, 0));
                            break;
                    }
                }

                return color;
            }
        }

        public Thickness borderThickness;
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                OnPropertyChanged("BorderThickness");
            }
        }

        private UnionCaptureType captureType;
        public UnionCaptureType CaptureType
        {
            get { return captureType; }
            set
            {
                captureType = value;
                OnPropertyChanged("CaptureType");
            }
        }

        public UnionCaptureType CentreCaptureType;

        public List<CharacterItem> CapturedCharacters = new List<CharacterItem>();

        private CharacterItem mainCharacterItem;
        public CharacterItem MainCharacterItem
        {
            get { return mainCharacterItem; }
            set
            {
                mainCharacterItem = value;
                OnPropertyChanged("MainCharacterItem");
            }
        }

        public int Row;
        public int Column;
        public BlockItem Parent;
        public CapturePriorityItem CapturePriorityItem;

        public CapturePriorityItem PrioirtyItem
        {
            get
            {
                if (Parent == null)
                    return CapturePriorityItem;
                else
                    return Parent.PrioirtyItem;
            }
        }

        public IEnumerable<BlockItem> AllBlocks
        {
            get
            {
                var list = new List<BlockItem>() { this };
                if (Parent == null)
                    return list;
                else
                {
                    return Parent.AllBlocks.Concat(list);
                }
            }
        }

        public BlockItem(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void GenerateH(BlockItem endBlock)
        {
            var rowNum = endBlock.Row - Row;
            var columnNum = endBlock.Column - Column;

            H = (int) Math.Sqrt((rowNum * rowNum) + (columnNum * columnNum));
        }

        public void NotifyBackgroundColor()
        {
            OnPropertyChanged("BackgroundColor");
        }
    }
}
