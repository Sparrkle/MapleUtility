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
                new SoundItem(){ Name = "Attention", Path="attention-required.mp3" },
                new SoundItem(){ Name = "Birdroid", Path="birdroid.mp3" },
                new SoundItem(){ Name = "Chimes", Path="chimes.mp3" },
                new SoundItem(){ Name = "Cyberincident", Path="cyberincident.mp3" },
                new SoundItem(){ Name = "Gesture", Path="gesture.mp3" },
                new SoundItem(){ Name = "Glitchy Tone", Path="glitchy-tone.mp3" },
                new SoundItem(){ Name = "I Demand Attention", Path="i-demand-attention.mp3" },
                new SoundItem(){ Name = "Insight", Path="insight.mp3" },
                new SoundItem(){ Name = "Jammed", Path="jammed.mp3" },
                new SoundItem(){ Name = "Just Like Magic", Path="just-like-magic.mp3" },
                new SoundItem(){ Name = "Long Expected", Path="long-expected.mp3" },
                new SoundItem(){ Name = "Lots of Data", Path="lots-of-data.mp3" },
                new SoundItem(){ Name = "Maidenly", Path="maidenly.mp3" },
                new SoundItem(){ Name = "Munchausen", Path="munchausen.mp3" },
                new SoundItem(){ Name = "News Bringer", Path="news-bringer.mp3" },
                new SoundItem(){ Name = "Pedantic", Path="pedantic-2.mp3" },
                new SoundItem(){ Name = "Push And Go", Path="push-and-go.mp3" },
                new SoundItem(){ Name = "Reload", Path="reload.mp3" },
                new SoundItem(){ Name = "Rising To The Surface", Path="rising-to-the-surface.mp3" },
                new SoundItem(){ Name = "Scratch", Path="scratch.mp3" },
                new SoundItem(){ Name = "Tasty", Path="tasty.mp3" },
                new SoundItem(){ Name = "Up to Speed", Path="up-to-speed.mp3" }
            };
        }
    }
}
