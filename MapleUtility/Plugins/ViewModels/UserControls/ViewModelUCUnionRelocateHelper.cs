using MapleUtility.Plugin.Lib;
using MapleUtility.Plugins.Common;
using MapleUtility.Plugins.Helpers;
using MapleUtility.Plugins.Models;
using MapleUtility.Plugins.ViewModels.Views.UnionRelocate;
using MapleUtility.Plugins.Views.Windows;
using MapleUtility.Plugins.Views.Windows.UnionRelocate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
                OnPropertyChanged("CharacterList");
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

        #region Button Command Variables
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
                CharacterList = settingItem.CharacterList;
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

                            System.Threading.Thread.Sleep(751);

                            if (backgroundWorker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

                            var unionData = await MapleDataHelper.GetUnionData(character);
                            if (backgroundWorker.CancellationPending)
                            {
                                args.Cancel = true;
                                return;
                            }

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

        private void AddCharacterEvent()
        {
            var newUnion = new CharacterItem();
            newUnion.Level = 60;
            CharacterList.Add(newUnion);

            OnPropertyChanged("CharacterList");
            CheckEvent();
        }

        private void RemoveCharacterEvent()
        {
            foreach (var timer in CharacterList.Where(o => o.IsChecked).ToList())
                CharacterList.Remove(timer);

            OnPropertyChanged("CharacterList");
            CheckEvent();
        }

        private void CheckEvent()
        {
            OnPropertyChanged("IsCharacterAllChecked");
            OnPropertyChanged("IsRemoveCharacterEnabled");
        }
    }
}
