using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        List<string> playlist = new List<string>();
        int currentIndex = -1;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaOpened += MediaOpen;
            mediaPlayer.MediaEnded += MediaEnd;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
        }
        private void MediaOpen(object sender, EventArgs e)
        {
            TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
            sldProgress.Maximum = duration.TotalSeconds;
            lblTotalTime.Content = duration.ToString(@"mm\:ss");
            timer.Start();
        }
        private void MediaEnd(object sender, EventArgs e)
        {
            PlayNext();
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count == 0) return;

            if (mediaPlayer.Source == null)
            {
                Play(playlist[0]);
            }
            else
            {
                mediaPlayer.Play();
            }
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PlayPrevious();
        }

        private void Play(string filePath)
        {
            mediaPlayer.Open(new Uri(filePath));
            mediaPlayer.Play();
            currentIndex = playlist.IndexOf(mediaPlayer.Source.LocalPath);
        }
        private void PlayNext()
        {
            if (currentIndex < playlist.Count - 1)
            {
                Play(playlist[++currentIndex]);
            }
        }
        private void PlayPrevious()
        {
            if (currentIndex > 0)
            {
                Play(playlist[--currentIndex]);
            }
        }
        private void sldProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(sldProgress.Value);
            lblCurrentTime.Content = TimeSpan.FromSeconds(sldProgress.Value).ToString(@"mm\:ss");
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Audió fájlok (*.mp3;*.wav)|*.mp3;*.wav";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                playlist.Add(filePath);
                lbPlaylist.Items.Add(Path.GetFileName(filePath));
            }
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbPlaylist.SelectedIndex != -1)
            {
                playlist.RemoveAt(lbPlaylist.SelectedIndex);
                lbPlaylist.Items.RemoveAt(lbPlaylist.SelectedIndex);
                if (currentIndex >= lbPlaylist.SelectedIndex)
                {
                    currentIndex--;
                }
            }
        }
        private void TimerTick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                sldProgress.Value = mediaPlayer.Position.TotalSeconds;
                lblCurrentTime.Content = mediaPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = (double)sldVolume.Value;
        }
    }
}
