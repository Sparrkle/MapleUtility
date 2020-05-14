using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public class MapleDataHelper
    {
        public static async Task<UnionDataModel> GetUnionData(string nickName)
        {
            UnionDataModel ResultModel = null;

            string url = "https://maplestory.nexon.com/Ranking/World/Total?c=" + nickName;

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        var HTML = await content.ReadAsStringAsync();
                        var idRegex = Regex.Match(HTML, nickName + @"<\/a><\/dt>[\S\s*]*<\/dd>");
                        var typeString = idRegex.Value.Split('\n')[1];
                        var levelString = idRegex.Value.Split('\n')[4];

                        var characterTypeRegex = Regex.Match(typeString, @"(?<=<dd>)(.*)(?=<\/dd>)");
                        var levelTypeRegex = Regex.Match(levelString, @"(?<=<td>Lv.)(.*)(?=<\/td>)");

                        var characterInfo = characterTypeRegex.Value.Replace(" ", "").Split('/');
                        ResultModel = new UnionDataModel()
                        {
                            Name = nickName,
                            CharacterType = characterInfo[0],
                            Job = characterInfo[1],
                            Level = Int32.Parse(levelTypeRegex.Value.Replace(" ", ""))
                        };
                    }
                }
            }

            return ResultModel;
        }
    }
}
