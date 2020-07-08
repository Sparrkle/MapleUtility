using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Lib;
using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public class MapleDataHelper
    {
        public static async Task<CharacterItem> GetUnionData(string nickName)
        {
            CharacterItem ResultModel = null;

            string url = "https://maplestory.nexon.com/Ranking/World/Total?c=" + nickName;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 10);
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            try
                            {
                                var HTML = await content.ReadAsStringAsync();
                                var idRegex = Regex.Match(HTML, nickName + @"<\/a><\/dt>[\S\s*]*<\/td>");
                                var typeString = idRegex.Value.Split('\n')[1];
                                var levelString = idRegex.Value.Split('\n')[4];

                                var characterTypeRegex = Regex.Match(typeString, @"(?<=<dd>)(.*)(?=<\/dd>)");
                                var levelTypeRegex = Regex.Match(levelString, @"(?<=<td>Lv.)(.*)(?=<\/td>)");

                                var characterInfo = characterTypeRegex.Value.Replace(" ", "").Split('/');
                                var level = Int32.Parse(levelTypeRegex.Value.Replace(" ", ""));
                                if (level < 60)
                                    return null;

                                ResultModel = new CharacterItem()
                                {
                                    Name = nickName,
                                    Job = characterInfo[1],
                                    Level = level
                                };
                            }
                            catch (Exception e)
                            {
                                throw new DataUnmatchedException();
                            }
                        }
                    }
                }
                catch (DataUnmatchedException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new MaplestoryTimeOutException();
                }
            }

            return ResultModel;
        }

        public static string GetCharacterList(string rawData)
        {
            try
            {
                var characterDataRegex = Regex.Match(rawData, @"월드\/캐릭터 선택[\S\s*]*대표캐릭터는 10레벨 이상이어야 지정할 수 있습니다.대표 캐릭터 저장");

                if (!characterDataRegex.Success)
                    return "";

                //0번째는 서버, 1번째는 캐릭터 리스트, 2번째는 쓸모x
                var characterDataArray = characterDataRegex.Value.Split('\n');

                var server = characterDataArray[0].Replace("월드/캐릭터 선택", "").Replace("\r", "");
                var characterDataSplitServer = characterDataArray[characterDataArray.Count()-2].Split(new string[] { server }, StringSplitOptions.None);
                var prevCharacter = characterDataSplitServer[0];

                string result = StockLib.AddString("", prevCharacter, "\n", false);
                for (int i = 1; i < characterDataSplitServer.Count() - 1; i++)
                {
                    var characterArray = characterDataSplitServer[i].Split(new string[] { prevCharacter }, StringSplitOptions.None);
                    string character = "";
                    if (characterArray.Length == 1)
                        throw new Exception("형식이 잘못되었습니다.");

                    if(characterArray.Length > 2) // 중간에 prevCharacter랑 겹침;;
                        character = characterArray[1] + prevCharacter + characterArray[2];
                    else
                        character = characterArray.Last();

                    result = StockLib.AddString(result, character, "\n", false);
                    prevCharacter = character;
                }

                return result;
            }
            catch(Exception)
            {
                return "";
            }
        }
    }
}
