using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Models
{
    public class RankItem
    {
        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int CharacterCount { get; set; }

        public RankItem(string name, int requiredLevel, int characterCount)
        {
            Name = name;
            RequiredLevel = requiredLevel;
            CharacterCount = characterCount;
        }
    }
}
