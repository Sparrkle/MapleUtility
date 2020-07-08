using EnumsNET;
using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Lib;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views.UnionRelocate;
using MapleUtility.Plugins.Views.UserControls;
using MapleUtility.Plugins.Views.Windows;
using MapleUtility.Plugins.Views.Windows.UnionRelocate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace MapleUtility.Plugins.ViewModels.UserControls
{
    public class ViewModelUCUnionRelocateHelper : Notifier
    {
        private ObservableCollection<CharacterItem> characterList;
        public ObservableCollection<CharacterItem> CharacterList
        {
            get { return characterList; }
            set
            {
                characterList = value;
                ChangeCharacterEvent();
            }
        }

        private ObservableCollection<string> jobList;
        public ObservableCollection<string> JobList
        {
            get { return jobList; }
            set
            {
                jobList = value;
                OnPropertyChanged("JobList");
            }
        }

        private ObservableCollection<RankItem> rankList;
        public ObservableCollection<RankItem> RankList
        {
            get { return rankList; }
            set
            {
                rankList = value;
                OnPropertyChanged("RankList");
                OnPropertyChanged("VisibleRankList");
            }
        }

        public IEnumerable<RankItem> VisibleRankList
        {
            get
            {
                if(TotalLevel < 500)
                {
                    if (CharacterList.Any(o => o.Level >= 200))
                        return RankList.OrderBy(o => o.RequiredLevel).Take(1);
                }
                return RankList.Where(o => o.RequiredLevel < TotalLevel).OrderBy(o => o.RequiredLevel);
            }
        }

        private RankItem selectedRank;
        public RankItem SelectedRank
        {
            get { return selectedRank; }
            set
            {
                selectedRank = value;

                if (selectedRank == null)
                    BlockManager.BlockHandicap = 5;
                else if (selectedRank.RequiredLevel > 6000)
                    BlockManager.BlockHandicap = 0;
                else
                {
                    var count = (selectedRank.RequiredLevel / 1000) - 1;
                    if (count < 0)
                        count = 0;

                    BlockManager.BlockHandicap = 5 - count;
                }

                RedrawBlockEvent();
                OnPropertyChanged("SelectedRank");
                OnPropertyChanged("RankTotalCharacters");
                OnPropertyChanged("UnionSize");
            }
        }

        public int RankTotalCharacters
        {
            get
            {
                if (SelectedRank == null)
                    return 0;
                else
                    return SelectedRank.CharacterCount;
            }
        }

        public int TotalLevel
        {
            get { return CharacterList.Where(o => o != null && o.Type != CharacterType.MapleMobile).OrderByDescending(o => o.Level).Take(40).Sum(o => o.Level); }
        }

        public int BlockCharacterCount
        {
            get { return CharacterList.Count(o => o.IsCaptured); }
        }

        public bool IsCharacterAllChecked
        {
            get
            {
                if (CharacterList == null || CharacterList.Count() == 0)
                    return false;
                else
                    return CharacterList.Where(o => o.IsChecked).Count() == CharacterList.Count();
            }
            set
            {
                if (CharacterList == null || CharacterList.Count() == 0)
                    return;

                foreach (var character in CharacterList)
                    character.IsChecked = value;

                CheckEvent();
            }
        }

        public bool IsRemoveCharacterEnabled
        {
            get
            {
                if (CharacterList == null || CharacterList.Count() == 0 || CharacterList.Where(o => o.IsChecked).Count() == 0)
                    return false;
                else
                    return true;
            }
        }

        public BlockManagerItem BlockManager
        {
            get { return App.BlockManager; }
            set
            {
                App.BlockManager = value;
                OnPropertyChanged("BlockManager");
            }
        }

        private CenterUnionCaptureModel[] centerUnionCaptureModelList;
        public CenterUnionCaptureModel[] CenterUnionCaptureModelList
        {
            get { return centerUnionCaptureModelList; }
            set
            {
                centerUnionCaptureModelList = value;
                OnPropertyChanged("CenterUnionCaptureModelList");
            }
        }

        public Rect UnionSize
        {
            get
            {
                var handicap = BlockManager.BlockHandicap;
                return new Rect(handicap * 22, handicap * 22, 484 - (handicap * 44), 440 - (handicap * 44));
            }
        }

        public string CharacterPassiveText
        {
            get
            {
                var characters = CharacterList.Where(o => o.IsCaptured).GroupBy(o => o.PassiveType);
                var textString = "";

                var xenonCharacter = characters.Where(o => o.Key == CharacterPassiveType.STR_DEX_LUK).FirstOrDefault();
                int strValue = 0;
                int dexValue = 0;
                int lukValue = 0;
                if (xenonCharacter != null)
                    strValue = dexValue = lukValue = xenonCharacter.FirstOrDefault().CharacterPassiveValue;

                foreach(var character in characters)
                {
                    if (character.Key == CharacterPassiveType.STR_DEX_LUK)
                        continue;

                    if (character.Key == CharacterPassiveType.STR)
                    {
                        strValue += character.Sum(o => o.CharacterPassiveValue);
                        continue;
                    }
                    if (character.Key == CharacterPassiveType.DEX)
                    {
                        dexValue += character.Sum(o => o.CharacterPassiveValue);
                        continue;
                    }
                    if (character.Key == CharacterPassiveType.LUK)
                    {
                        lukValue += character.Sum(o => o.CharacterPassiveValue);
                        continue;
                    }

                    string description = character.Key.AsString(EnumFormat.Description);
                    var value = character.Sum(o => o.CharacterPassiveValue);

                    textString = StockLib.AddString(textString, string.Format(description, value), "\n", false);
                }

                if (lukValue > 0)
                    textString = StockLib.AddString(string.Format(CharacterPassiveType.LUK.AsString(EnumFormat.Description), lukValue), textString, "\n", false);
                if (dexValue > 0)
                    textString = StockLib.AddString(string.Format(CharacterPassiveType.DEX.AsString(EnumFormat.Description), dexValue), textString, "\n", false);
                if (strValue > 0)
                    textString = StockLib.AddString(string.Format(CharacterPassiveType.STR.AsString(EnumFormat.Description), strValue), textString, "\n", false);

                return textString;
            }
        }

        public string CaptureEffectText
        {
            get
            {
                return "";
            }
        }

        #region Button Command Variables
        public ICommand CenterUnionCaptureChangeCommand { get; set; }
        public ICommand ChangeJobCommand { get; set; }
        public ICommand SyncCharacterCommand { get; set; }
        public ICommand AddCharacterCommand { get; set; }
        public ICommand RemoveCharacterCommand { get; set; }
        public ICommand CheckCommand { get; set; }
        #endregion

        public ViewModelUCUnionRelocateHelper()
        {
            CharacterList = new ObservableCollection<CharacterItem>();
            JobList = new ObservableCollection<string>(new List<string>()
            {
                "히어로",
                "팔라딘",
                "다크나이트",
                "아크메이지(불,독)",
                "아크메이지(썬,콜)",
                "비숍",
                "보우마스터",
                "신궁",
                "패스파인더",
                "나이트로드",
                "섀도어",
                "듀얼블레이드",
                "바이퍼",
                "캡틴",
                "캐논슈터",
                "블래스터",
                "배틀메이지",
                "와일드헌터",
                "메카닉",
                "제논",
                "데몬슬레이어",
                "데몬어벤져",
                "아란",
                "에반",
                "루미너스",
                "메르세데스",
                "팬텀",
                "은월",
                "소울마스터",
                "미하일",
                "플레임위자드",
                "윈드브레이커",
                "나이트워커",
                "스트라이커",
                "카이저",
                "엔젤릭버스터",
                "카데나",
                "제로",
                "키네시스",
                "아델",
                "일리움",
                "아크",
                "호영",
                "메이플M",
            }.OrderBy(o => o));
            RankList = new ObservableCollection<RankItem>()
            {
                new RankItem("노비스 1단계", 500, 9),
                new RankItem("노비스 2단계", 1000, 10),
                new RankItem("노비스 3단계", 1500, 11),
                new RankItem("노비스 4단계", 2000, 12),
                new RankItem("노비스 5단계", 2500, 13),
                new RankItem("베테랑 1단계", 3000, 18),
                new RankItem("베테랑 2단계", 3500, 19),
                new RankItem("베테랑 3단계", 4000, 20),
                new RankItem("베테랑 4단계", 4500, 21),
                new RankItem("베테랑 5단계", 5000, 22),
                new RankItem("마스터 1단계", 5500, 27),
                new RankItem("마스터 2단계", 6000, 28),
                new RankItem("마스터 3단계", 6500, 29),
                new RankItem("마스터 4단계", 7000, 30),
                new RankItem("마스터 5단계", 7500, 31),
                new RankItem("그랜드 마스터 1단계", 8000, 36),
                new RankItem("그랜드 마스터 2단계", 8500, 37),
                new RankItem("그랜드 마스터 3단계", 9000, 38),
                new RankItem("그랜드 마스터 4단계", 9500, 39),
                new RankItem("그랜드 마스터 5단계", 10000, 40)
            };

            var centerUnionCaptureModelList = new CenterUnionCaptureModel[8];
            var index = 0;

            foreach (var centerUnion in CenterUnionCaptureModel.CenterUnionItems)
            {
                centerUnionCaptureModelList[index] = new CenterUnionCaptureModel(centerUnion);
                index++;
            }
            CenterUnionCaptureModelList = centerUnionCaptureModelList;

            CenterUnionCaptureChangeCommand = new RelayCommand(o => CenterUnionCaptureChangeEvent(o));
            ChangeJobCommand = new RelayCommand(o => ChangeCharacterEvent());
            SyncCharacterCommand = new RelayCommand(o => SyncCharacterEvent((Window) o));
            AddCharacterCommand = new RelayCommand(o => AddCharacterEvent());
            RemoveCharacterCommand = new RelayCommand(o => RemoveCharacterEvent());
            CheckCommand = new RelayCommand(o => CheckEvent());
        }

        public void Initialize(SettingItem settingItem)
        {
            if (settingItem.CharacterList == null)
                CharacterList = new ObservableCollection<CharacterItem>();
            else
                CharacterList = new ObservableCollection<CharacterItem>(settingItem.CharacterList.Where(o => o != null).ToList());

            if (settingItem.BlockManager != null)
                BlockManager = settingItem.BlockManager;

            if(CharacterList.Count() >= 5)
            {
                CharacterList[0].MainCaptureBlock = BlockManager.GetBlockItem(9, 8);
                CharacterList[1].MainCaptureBlock = BlockManager.GetBlockItem(9, 10);
                CharacterList[2].MainCaptureBlock = BlockManager.GetBlockItem(9, 5);
                CharacterList[3].MainCaptureBlock = BlockManager.GetBlockItem(9, 3);
                CharacterList[4].MainCaptureBlock = BlockManager.GetBlockItem(9, 1);
                CharacterList[4].Angle = 90;

                ChangeCharacterEvent();
            }
        }

        private void CenterUnionCaptureChangeEvent(object value)
        {
            var items = (value as object[]).Cast<string>().ToList();

            foreach(var item in CenterUnionCaptureModelList)
            {
                if(item.Name == items[0])
                    item.Name = items[1];
                else if (item.Name == items[1])
                    item.Name = items[0];
            }
        }

        private void SyncCharacterEvent(Window window)
        {
            var syncWindow = new WindowUnionRelocateSyncSelect();
            syncWindow.Left = window.Left + (window.ActualWidth - syncWindow.Width) / 2;
            syncWindow.Top = window.Top + (window.ActualHeight - syncWindow.Height) / 2;

            var targetList = CharacterList.Where(o => !(o.Type == CharacterType.None || o.Type == CharacterType.MapleMobile) && o.IsChecked).ToList();

            var vm = syncWindow.DataContext as ViewModelUnionRelocateSyncSelect;
            vm.IsUtilitySyncEnabled = targetList.Count() > 0;

            syncWindow.ShowDialog();

            if(syncWindow.DialogResult.Value == true)
            {
                List<string> characterList;
                if (vm.IsMapleDataSyncChecked)
                    characterList = vm.SyncData.Split('\n').ToList();
                else
                    characterList = targetList.Select(o => o.Name).ToList();

                RoutedEventHandler progressHandler = null;
                DoWorkEventHandler doWorkHandler = null;

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

                var progress = WindowProgress.Instance as WindowProgress;
                progress.Initialize();
                progress.Worker = backgroundWorker;
                progress.Title = "Sync All Characters";

                progress.Update(ProgressType.START, "Sync All Character");
                doWorkHandler = async (sender, args) =>
                {
                    backgroundWorker.DoWork -= doWorkHandler;

                    ObservableCollection<CharacterItem> newCharacterList = new ObservableCollection<CharacterItem>();
                    var index = 1;
                    var count = characterList.Count();
                    var prevTime = new TimeSpan(0, 0, 0);
                    foreach (var character in characterList)
                    {
                        try
                        {
                            if(backgroundWorker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }
                            progress.Update(ProgressType.UPDATE, "Sync " + character + "...(" + index + "/" + count + ")");

                            System.Threading.Thread.Sleep(2001 - prevTime.Milliseconds);

                            if (backgroundWorker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

                            var time = DateTime.Now;
                            var unionData = await MapleDataHelper.GetUnionData(character);
                            if (backgroundWorker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }
                            prevTime = DateTime.Now - time;

                            // 2차전직 이상, 레벨 60 이상
                            if (unionData != null && unionData.Level >= 60 && unionData.Job != "시티즌" && unionData.Job != "노블레스" && unionData.Job != "초보자")
                            {
                                if (vm.IsUtilitySyncChecked)
                                {
                                    var item = CharacterList.Where(o => o.Name == character).FirstOrDefault();
                                    item.Level = unionData.Level;
                                    item.Job = unionData.Job;
                                }
                                else
                                    newCharacterList.Add(unionData);
                            }
                            else if(vm.IsUtilitySyncChecked)
                            {
                                var item = CharacterList.Where(o => o.Name == character).FirstOrDefault();
                                CharacterList.Remove(item);
                            }
                        }
                        catch (MaplestoryTimeOutException)
                        {
                            newCharacterList = null;
                            progress.Dispatcher.Invoke(() =>
                            {
                                progress.Close();
                                RadWindow.Alert("메이플스토리 홈페이지 접속에 실패하였습니다. 나중에 다시 시도해주세요.");
                            });
                            break;
                        }
                        catch (Exception)
                        {
                            progress.Update(ProgressType.UPDATE, "Sync Error " + character + ".");
                        }

                        index++;
                    }
                    progress.Update(ProgressType.END, "Sync All Character");

                    if (newCharacterList != null && vm.IsMapleDataSyncChecked)
                        CharacterList = newCharacterList;

                    IsCharacterAllChecked = false;
                };
                progressHandler = (sender, args) =>
                {
                    progress.Loaded -= progressHandler;
                    backgroundWorker.RunWorkerAsync();
                };
                progress.Loaded += progressHandler;
                backgroundWorker.DoWork += doWorkHandler;
                progress.ShowDialog();
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;

            var progress = WindowProgress.Instance as WindowProgress;
            progress.Close();
        }

        private void RedrawBlockEvent()
        {
            foreach(var character in characterList.Where(o => o.IsCaptured))
            {
                var mainBlock = character.MainCaptureBlock;
                if (!BlockManager.IsAllowedBlock(mainBlock.Row, mainBlock.Column))
                    character.MainCaptureBlock = null;
                else
                    character.DrawBlock();
            }
            ChangeCharacterEvent();
        }

        private void AddCharacterEvent()
        {
            var newUnion = new CharacterItem();
            newUnion.Level = 60;
            CharacterList.Add(newUnion);

            ChangeCharacterEvent();
            CheckEvent();
        }

        private void RemoveCharacterEvent()
        {
            foreach (var timer in CharacterList.Where(o => o.IsChecked).ToList())
                CharacterList.Remove(timer);

            ChangeCharacterEvent();
            CheckEvent();
        }

        public void ChangeCharacterEvent()
        {
            OnPropertyChanged("CharacterList");
            OnPropertyChanged("TotalLevel");
            OnPropertyChanged("RankList");
            OnPropertyChanged("VisibleRankList");
            OnPropertyChanged("CharacterPassiveText");
            OnPropertyChanged("BlockCharacterCount");
        }

        private void CheckEvent()
        {
            OnPropertyChanged("IsCharacterAllChecked");
            OnPropertyChanged("IsRemoveCharacterEnabled");
        }
    }
}
