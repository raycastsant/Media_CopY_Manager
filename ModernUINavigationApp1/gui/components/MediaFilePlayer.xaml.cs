using System;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.components
{
    /// <summary>
    /// Interaction logic for MediaFilePlayer.xaml
    /// </summary>
    public partial class MediaFilePlayer : UserControl
    {
        public MediaFilePlayer()
        {
            InitializeComponent();
        }

        public void LoadMediaFile(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                Media.Source = new Uri(filePath);
                Media.Play();
                SetPlayMode();
                this.Visibility = Visibility.Visible;
            }
        }

        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
        }

        private void SetPlayMode()
        {
            BtnPlay.Width = 0;
            BtnPause.Width = 35;
        }

        private void SetPauseMode()
        {
            BtnPlay.Width = 35;
            BtnPause.Width = 0;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source != null)
            {
                Media.Play();
                SetPlayMode();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.CanPause)
            {
                Media.Pause();
                SetPauseMode();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source != null)
            {
                Media.Stop();
                SetPauseMode();
            }
        }

        public void Stop()
        {
            if (Media.Source != null)
            {
                Media.Stop();
                Media.Source = null;
                SetPauseMode();
            }
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            Media.IsMuted = !Media.IsMuted;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Volume = VolumeSlider.Value;
        }

        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.SpeedRatio = SpeedSlider.Value;
        }
    }
}
