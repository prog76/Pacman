using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GeniusPacman.Core
{
    public interface IAudio
    {
		  void PlayGhostScared();
		  void PlayPill();
		  void PlayExtraPac();
		  void PlayBigPill();
		  void PlayGoodPill(double speed);
        void PlayBonus();
        void PlayDeath();
		  void PlayKill();
        void PlayEatGhost();
        void PlayIntro();
		  void PlayWrongPill();
		  void PlayMenuOver();
		  void PlayMenuPress();
		  void MusicMenu();
		  void MusicGame();
		  void MusicMute(bool mute);
		  void SoundMute(bool mute);
    }
}
