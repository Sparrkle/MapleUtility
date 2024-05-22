using MapleUtility.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleUtility.Plugins.Helpers
{
    public class InitialHelper
    {
        public static ObservableCollection<SoundItem> InitializeSoundList()
        {
            return new ObservableCollection<SoundItem>()
            {
                new SoundItem(){ Priority = 1, Name = "Attention", Path="attention-required.mp3" },
                new SoundItem(){ Priority = 2, Name = "Birdroid", Path="birdroid.mp3" },
                new SoundItem(){ Priority = 3, Name = "Chimes", Path="chimes.mp3" },
                new SoundItem(){ Priority = 4, Name = "Cyberincident", Path="cyberincident.mp3" },
                new SoundItem(){ Priority = 5, Name = "Gesture", Path="gesture.mp3" },
                new SoundItem(){ Priority = 6, Name = "Glitchy Tone", Path="glitchy-tone.mp3" },
                new SoundItem(){ Priority = 7, Name = "I Demand Attention", Path="i-demand-attention.mp3" },
                new SoundItem(){ Priority = 8, Name = "Insight", Path="insight.mp3" },
                new SoundItem(){ Priority = 9, Name = "Jammed", Path="jammed.mp3" },
                new SoundItem(){ Priority = 10, Name = "Just Like Magic", Path="just-like-magic.mp3" },
                new SoundItem(){ Priority = 11, Name = "Long Expected", Path="long-expected.mp3" },
                new SoundItem(){ Priority = 12, Name = "Lots of Data", Path="lots-of-data.mp3" },
                new SoundItem(){ Priority = 13, Name = "Maidenly", Path="maidenly.mp3" },
                new SoundItem(){ Priority = 14, Name = "Munchausen", Path="munchausen.mp3" },
                new SoundItem(){ Priority = 15, Name = "News Bringer", Path="news-bringer.mp3" },
                new SoundItem(){ Priority = 16, Name = "Pedantic", Path="pedantic-2.mp3" },
                new SoundItem(){ Priority = 17, Name = "Push And Go", Path="push-and-go.mp3" },
                new SoundItem(){ Priority = 18, Name = "Reload", Path="reload.mp3" },
                new SoundItem(){ Priority = 19, Name = "Rising To The Surface", Path="rising-to-the-surface.mp3" },
                new SoundItem(){ Priority = 20, Name = "Scratch", Path="scratch.mp3" },
                new SoundItem(){ Priority = 21, Name = "Tasty", Path="tasty.mp3" },
                new SoundItem(){ Priority = 22, Name = "Up to Speed", Path="up-to-speed.mp3" }
            };
        }
    }
}
