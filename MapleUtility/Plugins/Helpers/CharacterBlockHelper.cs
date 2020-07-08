using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public class CharacterBlockHelper
    {
        public static List<RowColumnItem> GetCharacterBlockPosition(CharacterType characterType, string job, int level, int angle)
        {
            var resultItems = new List<RowColumnItem>();
            resultItems.Add(new RowColumnItem(0, 0));

            switch (characterType)
            {
                case CharacterType.Warrior:
                    if (job == "제로")
                    {
                        if (level >= 160)
                            resultItems.Add(new RowColumnItem(0, 1));
                        if (level >= 180)
                            resultItems.Add(new RowColumnItem(1, 0));
                    }
                    else
                    {
                        if (level >= 100)
                            resultItems.Add(new RowColumnItem(0, 1));
                        if (level >= 140)
                            resultItems.Add(new RowColumnItem(1, 0));
                    }
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(1, 1));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(0, 2));
                    break;
                case CharacterType.Wizard:
                    if (level >= 100)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 140)
                        resultItems.Add(new RowColumnItem(0, -1));
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(1, 0));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(-1, 0));
                    break;
                case CharacterType.Archer:
                    if (level >= 100)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 140)
                        resultItems.Add(new RowColumnItem(0, -1));
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(0, 2));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(0, -2));
                    break;
                case CharacterType.MapleMobile:
                    if (level >= 50)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 70)
                        resultItems.Add(new RowColumnItem(0, -1));
                    if (level >= 120)
                        resultItems.Add(new RowColumnItem(0, 2));
                    break;
                case CharacterType.Thief:
                    if (level >= 100)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 140)
                        resultItems.Add(new RowColumnItem(0, -1));
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(1, 1));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(-1, 1));
                    break;
                case CharacterType.Pirate:
                    if (level >= 100)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 140)
                        resultItems.Add(new RowColumnItem(1, 0));
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(-1, 1));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(-2, 1));
                    break;
                case CharacterType.Hybrid:
                    if (level >= 100)
                        resultItems.Add(new RowColumnItem(0, 1));
                    if (level >= 140)
                        resultItems.Add(new RowColumnItem(0, -1));
                    if (level >= 200)
                        resultItems.Add(new RowColumnItem(1, 1));
                    if (level >= 250)
                        resultItems.Add(new RowColumnItem(-1, -1));
                    break;
            }

            if (angle > 0)
                return RotateBlockPosition(resultItems, angle);
            else
                return resultItems;
        }

        public static List<BlockItem> GetCapturedBlocks(CharacterType characterType, string job, int level, BlockItem mainBlock, int angle)
        {
            var resultBlocks = new List<BlockItem>();
            var blockManager = App.BlockManager;

            var currentRow = mainBlock.Row;
            var currentColumn = mainBlock.Column;

            var positionList = GetCharacterBlockPosition(characterType, job, level, angle);

            foreach(var position in positionList)
                resultBlocks.Add(blockManager.GetBlockItem(currentRow + position.Row, currentColumn + position.Column));

            return resultBlocks.Where(o => o != null).ToList();
        }

        public static List<RowColumnItem> RotateBlockPosition(List<RowColumnItem> rowColumnItems, int angle)
        {
            var radian = angle * Math.PI / 180.0;
            var resultBlocks = new List<RowColumnItem>();

            foreach (var rowColumn in rowColumnItems)
            {
                var row = rowColumn.Row;
                var column = rowColumn.Column;

                float cosa = (float) Math.Cos(radian);
                float sina = (float) Math.Sin(radian);

                var rotateRow = (int)Math.Round(column * sina + row * cosa, 1);
                var rotateColumn = (int) Math.Round(column * cosa - row * sina, 1);

                resultBlocks.Add(new RowColumnItem(rotateRow, rotateColumn));
            }

            return resultBlocks;
        }
    }
}
