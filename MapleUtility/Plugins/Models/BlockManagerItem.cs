using MapleUtility.Plugins.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace MapleUtility.Plugins.Models
{
    public class BlockManagerItem : Notifier
    {
        public int Rows
        {
            get { return 20; }
        }

        public int Columns
        {
            get { return 22; }
        }

        private int blockHandicap;
        public int BlockHandicap
        {
            get { return blockHandicap; }
            set
            {
                blockHandicap = value;
                OnPropertyChanged("BlockHandicap");
            }
        }

        private ObservableCollection<BlockItem> blocks;
        public ObservableCollection<BlockItem> Blocks
        {
            get { return blocks; }
            set
            {
                blocks = value;
                OnPropertyChanged("Blocks");
            }
        }

        public BlockManagerItem()
        {
            Blocks = new ObservableCollection<BlockItem>();

            var columnCenter = (Columns / 2) - 1;
            var rowCenter = (Rows / 2) - 1;

            for (int i = 0; i<Rows * Columns; i++)
            {
                var row = i / Columns;
                var column = i % Columns;
                var block = new BlockItem(row, column);

                #region Border 설정
                var thickness = new Thickness(0.0);

                if(row <= 9)
                {
                    if (column == row) // 좌상 Border
                    {
                        thickness.Top = 0.5;
                        thickness.Right = 0.5;
                    }
                    else if(column == Columns - 1 - row) // 우상 Border
                    {
                        thickness.Left = 0.5;
                        thickness.Top = 0.5;
                    }
                }
                else
                {
                    if (column == 20 - row - 1) // 좌하 Border
                    {
                        thickness.Right = 0.5;
                        thickness.Bottom = 0.5;
                    }
                    else if (column == row + 2) // 우하 Border
                    {
                        thickness.Left = 0.5;
                        thickness.Bottom = 0.5;
                    }
                }

                if (column == columnCenter) // 가로 중간 Border
                    thickness.Right = 0.5;
                if (row == rowCenter) // 세로 중간 Border
                    thickness.Bottom = 0.5;

                if (row == 5 && column >= 5 && column <= 16) // STR - 공격력 Top Border
                    thickness.Top = 0.5;
                else if (row == 14 && column >= 5 && column <= 16) // MP - Dex Bottom Border
                    thickness.Bottom = 0.5;

                if (column == 5 && row >= 5 && row <= 14) // 마력 - HP Left Border
                    thickness.Left = 0.5;
                else if (column == 16 && row >= 5 && row <= 14) // Int - Luk Right Border
                    thickness.Right = 0.5;

                block.BorderThickness = thickness;
                #endregion

                #region Union 점령 타입 설정
                if(row <= rowCenter)
                {
                    if(column <= columnCenter)
                    {
                        if (row >= column)
                        {
                            if (column <= 4) // 크뎀
                                block.CaptureType = UnionCaptureType.CriticalDamage;
                            else // 마력
                            {
                                block.CaptureType = UnionCaptureType.MagicAttack;
                                block.CentreCaptureType = UnionCaptureType.Variable7;
                            }
                        }
                        else
                        {
                            if (row <= 4) // 상태이상내성
                                block.CaptureType = UnionCaptureType.CrowControlImmune;
                            else // STR
                            {
                                block.CaptureType = UnionCaptureType.STR;
                                block.CentreCaptureType = UnionCaptureType.Variable11;
                            }
                        }
                    }
                    else
                    {
                        if(column >= Columns - 1 - row)
                        {
                            if (column >= 17) // 크확
                                block.CaptureType = UnionCaptureType.CriticalPercentage;
                            else // INT
                            {
                                block.CaptureType = UnionCaptureType.INT;
                                block.CentreCaptureType = UnionCaptureType.Variable2;
                            }
                        }
                        else
                        {
                            if (row <= 4) // EXP
                                block.CaptureType = UnionCaptureType.EXP;
                            else // 공격력
                            {
                                block.CaptureType = UnionCaptureType.AttackPoint;
                                block.CentreCaptureType = UnionCaptureType.Variable5;
                            }
                        }
                    }
                }
                else
                {
                    if(column <= columnCenter)
                    {
                        if(column <= 20 - row - 1)
                        {
                            if (column <= 4) // 방무
                                block.CaptureType = UnionCaptureType.ArmorPenetration;
                            else // HP
                            {
                                block.CaptureType = UnionCaptureType.HP;
                                block.CentreCaptureType = UnionCaptureType.Variable8;
                            }
                        }
                        else
                        {
                            if (row >= 15) // 벞지
                                block.CaptureType = UnionCaptureType.BuffTime;
                            else // MP
                            {
                                block.CaptureType = UnionCaptureType.MP;
                                block.CentreCaptureType = UnionCaptureType.Variable10;
                            }
                        }
                    }
                    else
                    {
                        if(column <= row + 1)
                        {
                            if (row >= 15) // 스탠스
                                block.CaptureType = UnionCaptureType.Stance;
                            else // DEX
                            {
                                block.CaptureType = UnionCaptureType.DEX;
                                block.CentreCaptureType = UnionCaptureType.Variable1;
                            }
                        }
                        else
                        {
                            if (column >= 17) // 보공
                                block.CaptureType = UnionCaptureType.BossDamage;
                            else // LUK
                            {
                                block.CaptureType = UnionCaptureType.LUK;
                                block.CentreCaptureType = UnionCaptureType.Variable4;
                            }
                        }
                    }
                }
                #endregion
                Blocks.Add(block);
            }
        }

        public bool IsAllowedBlock(int row, int column)
        {
            if (row < 0 + blockHandicap)
                return false;
            if (column < 0 + blockHandicap)
                return false;

            if (row >= Rows - blockHandicap)
                return false;
            if (column >= Columns - blockHandicap)
                return false;

            return true;
        }

        public BlockItem GetBlockItem(int row, int column)
        {
            if (!IsAllowedBlock(row, column))
                return null;

            return blocks[row * Columns + column];
        }
    }
}
