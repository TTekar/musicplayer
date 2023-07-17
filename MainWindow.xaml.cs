using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Timers;
using System.Threading;
using System.Diagnostics;


//  Made by Tekar

namespace MusicPlayer
{
    public partial class MainWindow : Window
    {

        bool playAudio = true;
        public static bool loopThisAudio = false;
        public static bool ShufflePlaylist = false;
        double StartingVolume;

        //int[] playlists = SettingsWindow.playlists;

        public MainWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += new EventHandler(ticktock);
            _timer.Start();

            string[] files = Directory.GetFiles(@"data\Playlists", "*.txt");

            foreach (var file in files)
            {
                if(Path.GetFileNameWithoutExtension(file) == "temp")
                {
                    continue;
                }
                var playlistAdd = file.Split("\\").Last();
                playlistAdd = playlistAdd.Substring(0, playlistAdd.LastIndexOf("."));
                //playlist_list.Items.Add($"Playlist {Int32.Parse(playlistAdd) + 1}");
                playlist_list.Items.Add(playlistAdd);
            }

            BorderMainWindow.Visibility = Visibility.Visible;

            try
            {
                TextReader tr = new StreamReader("data/SavedSettings.txt");
                ShufflePlaylist = Convert.ToBoolean(tr.ReadLine());
                loopThisAudio = Convert.ToBoolean(tr.ReadLine());
                StartingVolume = Convert.ToDouble(tr.ReadLine());


                mediaMusic.Volume = StartingVolume;
                VolumeLabel.Content = Math.Round(StartingVolume * 700).ToString() + "%";
                track_volume.Value = StartingVolume * 700;
                SetVolumeLabel.Content = Math.Round(StartingVolume * 700).ToString() + "%";
                track_SetVolume.Value = StartingVolume * 700;
            }
            catch
            {

            }

            if (ShufflePlaylist)
            {
                BackgroundRec.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipse.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipse.Margin = new Thickness(11.25, 1, 1, 1);
            }
            else
            {
                BackgroundRec.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipse.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipse.Margin = new Thickness(1, 1, 11.25, 1);
            }

            if (loopThisAudio)
            {
                BackgroundRecLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipseLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipseLoop.Margin = new Thickness(11.25, 1, 1, 1);
                
            }else
            {
                BackgroundRecLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipseLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipseLoop.Margin = new Thickness(1, 1, 11.25, 1);
            }

        }

        TimeSpan _position;
        DispatcherTimer _timer = new DispatcherTimer();

        void ticktock(object sender, EventArgs e)
        {
            if (!musicProgression.IsMouseCaptureWithin)
                musicProgression.Value = mediaMusic.Position.TotalSeconds;

            
            
            string[] files = Directory.GetFiles(@"data\Playlists", "*.txt");

            playlist_list.Items.Clear();

            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file) == "temp")
                {
                    continue;
                }
                var playlistAdd = file.Split("\\").Last();
                playlistAdd = playlistAdd.Substring(0, playlistAdd.LastIndexOf("."));
                //playlist_list.Items.Add($"Playlist {Int32.Parse(playlistAdd) + 1}");
                playlist_list.Items.Add(playlistAdd);

            }

            bool IsPlaying()
            {
                var pos1 = mediaMusic.Position;
                System.Threading.Thread.Sleep(1);
                var pos2 = mediaMusic.Position;

                return pos2 != pos1;
            }

            if (IsPlaying())
            {
                playAudio = true;
                PlayAudioButton.Visibility = Visibility.Collapsed;
                PauseAudioButton.Visibility = Visibility.Visible;
            }
            else
            {
                playAudio = false;
                PlayAudioButton.Visibility = Visibility.Visible;
                PauseAudioButton.Visibility = Visibility.Collapsed;
            }

        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            _position = mediaMusic.NaturalDuration.TimeSpan;
            musicProgression.Minimum = 0;
            musicProgression.Maximum = _position.TotalSeconds;

            var timer = new System.Timers.Timer { Interval = 1000 }; // in milliseconds
            timer.Elapsed += (s, e) =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (mediaMusic.Position == mediaMusic.NaturalDuration)
                            timer.Stop();
                        do1();
                    });
                }catch
                {

                }
                
            };

            void do1()
            {
                changeStatus();
            }

            timer.Start();
        }

        private void sliderSeek_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(musicProgression.Value);
            mediaMusic.Position = new TimeSpan(0, 0, 0, pos, 0);
            
        }

        public string Milliseconds_to_Minute(long milliseconds)
        {
            int minute = (int)(milliseconds / (1000 * 60));
            int seconds = (int)(milliseconds / 1000);
            seconds = seconds - minute * 60;
            if (seconds < 10)
            {
                string secondsS = "0" +seconds.ToString();
                
                return (minute + " : " + secondsS);
            }
            return (minute + " : " + seconds);

            
        }

        void changeStatus()
        {
            
            Duration.Text = Milliseconds_to_Minute((long)mediaMusic.Position.TotalMilliseconds);
        }

        private void sliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicProgression.IsMouseCaptureWithin)
            {
                int pos = Convert.ToInt32(musicProgression.Value);
                mediaMusic.Position = new TimeSpan(0, 0, 0, pos, 0);
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClick_Close(object sender, RoutedEventArgs e)
        { 
            System.Windows.Application.Current.Shutdown();
        }

        string[] paths, files;

        private void btnClick_OpenMusic(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Mp3 Files|*.mp3|Avi Files|*.avi|Wav Files|*.wav";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                paths = ofd.FileNames;
                files = ofd.FileNames;
                for (int x = 0; x < files.Length; x++)
                {
                    string[] listPicesText = files[x].Split('\\');
                    string shortName =  listPicesText.Last();

                    track_list.Items.Add(shortName);
                }
            }

        }


        



        private void list_selection_changed(object sender, SelectionChangedEventArgs e)
        {
            int currentPlaylist = playlist_list.SelectedIndex;

            

            if (currentPlaylist == -1)
            {
                return;
            }

            mediaMusic.Stop();
            TextReader trP1 = new StreamReader($"data/Playlists/{playlist_list.Items[currentPlaylist]}.txt");
            readP1 = trP1.ReadLine();
            track_list.Items.Clear();

            if (paths != null)
            {

                Array.Clear(paths);
            }

            List<string> list = new List<string>();


            while (readP1 != null)
            {
                string lineToAdd = readP1;

                string[] listPicesText = lineToAdd.Split('\\');
                string shortName = listPicesText.Last();


                track_list.Items.Add(shortName);


                list.Add(lineToAdd);



                readP1 = trP1.ReadLine();
            }
            trP1.Close();

            paths = list.ToArray();
            
            
            

            
            
        }

        private void btnClick_PlayAudio(object sender, RoutedEventArgs e)
        {
            if (playAudio)
            {
                mediaMusic.Pause();
                playAudio = false;
            }else
            {
                mediaMusic.Play();
                playAudio = true;
            }
        }

        private void btnClick_NextAudio(object sender, RoutedEventArgs e)
        {
            if (ShufflePlaylist == true)
            {
                PlayNextShuffle();
            }else
            {
                if (track_list.SelectedIndex < track_list.Items.Count - 1)
                {
                    track_list.SelectedIndex = track_list.SelectedIndex + 1;
                }
                else if (track_list.SelectedIndex == track_list.Items.Count - 1)
                {
                    track_list.SelectedIndex = 0;
                }
            }
        }


        private void track_list_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (track_list.SelectedIndex != -1)
            {
                mediaMusic.Source = new Uri(paths[track_list.SelectedIndex]);
                mediaMusic.Play();
            }
            
        }


        private void track_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaMusic.Volume = (double)track_volume.Value / 700;

            VolumeLabel.Content = Math.Round(track_volume.Value).ToString() + "%";
        }

        private void btnClick_Min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClick_PrevAudio(object sender, RoutedEventArgs e)
        {
            if (track_list.SelectedIndex > 0)
            {
                track_list.SelectedIndex = track_list.SelectedIndex - 1;
            }
            else if (track_list.SelectedIndex == 0)
            {
                track_list.SelectedIndex = track_list.Items.Count - 1;
            }
        }

        private void btnClick_LoopAudio(object sender, RoutedEventArgs e)
        {
            if (loopThisAudio)
            {
                loopThisAudio = false;

            }
            else if(ShufflePlaylist == false && loopThisAudio == false)
            {
                loopThisAudio = true;

            }
        }

        private void btnClick_ShuffleAudio(object sender, RoutedEventArgs e)
        {
            if (ShufflePlaylist == true)
            {
                ShufflePlaylist = false;

            }
            else if (ShufflePlaylist == false && loopThisAudio == false)
            {
                ShufflePlaylist = true;

            }

        }

        private void btnClick_MixAudio(object sender, RoutedEventArgs e)
        {
            mediaMusic.Stop();
            track_list.SelectedIndex = -1;
            Random r = new Random();
            int w = track_list.Items.Count;
            while (w > 1)
            {
                w--;
                int u = r.Next(w + 1);
                object value = track_list.Items[u];
                object valuePaths = paths[u];
                track_list.Items[u] = track_list.Items[w];
                paths[u] = paths[w];
                track_list.Items[w] = value;
                paths[w] = (string)valuePaths;
            }
        }

        private void btnClick_ClearTrack(object sender, RoutedEventArgs e)
        {
            mediaMusic.Stop();
            track_list.Items.Clear();
        }

        private void btnClick_OpenSettigs(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWin = new SettingsWindow();
            setWin.Show();
        }


        private void StartTriangleAnimation(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DropdownMenuTriangle.Height = 13;
            DropdownMenuTriangle.Width = 26;
        }

        private void EndTriangleAnimation(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DropdownMenuTriangle.Height = 12;
            DropdownMenuTriangle.Width = 25;
        }

        private void SwitchRandomClick(object sender, MouseButtonEventArgs e)
        {
            if (ShufflePlaylist)
            {
                BackgroundRec.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipse.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipse.Margin = new Thickness(1, 1, 11.25, 1);
                ShufflePlaylist = false;
            }
            else
            {
                BackgroundRec.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipse.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipse.Margin = new Thickness(11.25, 1, 1, 1);
                ShufflePlaylist = true;
            }
        }

        private void SwitchLoopClick(object sender, MouseButtonEventArgs e)
        {   
            if (loopThisAudio)
            {
                BackgroundRecLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipseLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipseLoop.Margin = new Thickness(1, 1, 11.25, 1);

                loopThisAudio = false;
            }
            else
            {
                BackgroundRecLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 104, 255));
                MovingElipseLoop.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 38, 49, 178));
                MovingElipseLoop.Margin = new Thickness(11.25, 1, 1, 1);

                loopThisAudio = true;
            }
        }

        private void ToggleDropdown(object sender, RoutedEventArgs e)
        {
            if (DropdownMenu.Visibility == Visibility.Visible)
            {
                DropdownMenu.Visibility = Visibility.Collapsed;
                ((ScaleTransform)((TransformGroup)DropdownMenuTriangle.RenderTransform).Children[0]).ScaleY = -1;
                track_volume.Visibility = Visibility.Visible;
                VolumeLabel.Visibility = Visibility.Visible;

            }
            else if (DropdownMenu.Visibility == Visibility.Collapsed)
            {
                DropdownMenu.Visibility = Visibility.Visible;
                ((ScaleTransform)((TransformGroup)DropdownMenuTriangle.RenderTransform).Children[0]).ScaleY = 1;
                track_volume.Visibility = Visibility.Collapsed;
                VolumeLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void btnClick_SaveSettings(object sender, RoutedEventArgs e)
        {
            TextWriter tw = new StreamWriter("data/SavedSettings.txt");

            tw.WriteLine(ShufflePlaylist);
            tw.WriteLine(loopThisAudio);
            tw.WriteLine(StartingVolume);

            tw.Close();
        }

        private void track_SetVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StartingVolume = track_SetVolume.Value / 700;

            SetVolumeLabel.Content = Math.Round(track_SetVolume.Value).ToString() + "%";
        }

        private void btnClick_OpenPlaylistSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow setWin = new SettingsWindow();
            setWin.Show();
        }


        
        

        string readP1 = "";


        



        private void ButtonEnterEffectPrev(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PrevButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 140, 255));
        }

        private void ButtonLeaveEffectPrev(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PrevButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 105, 255));
        }

        private void ButtonEnterEffectNext(object sender, System.Windows.Input.MouseEventArgs e)
        {
            NextButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 140, 255));
        }

        private void ButtonLeaveEffectNext(object sender, System.Windows.Input.MouseEventArgs e)
        {
            NextButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 105, 255));
        }

        private void ButtonEnterEffectPlay(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayAudioButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 140, 255));
            PauseAudioButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 140, 255));
        }

        private void ButtonLeaveEffectPlay(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PlayAudioButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 105, 255));
            PauseAudioButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 105, 255));
        }

        private void ButtonEnterEffectPSettings(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonPSettings.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 140, 255));
        }

        private void ButtonLeaveEffectPSettings(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonPSettings.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 71, 105, 255));
        }

        

        int listIndex = 0;
        int[] shuffledList;

        private void PlayNextShuffle()
        {
            if (listIndex != 0)
            {
                if (listIndex < shuffledList.Length - 1)
                {
                    track_list.SelectedIndex = shuffledList[listIndex];
                    listIndex++;
                }
                else if (listIndex == shuffledList.Length - 1)
                {
                    Shuffle(shuffledList);
                    listIndex = 0;
                    track_list.SelectedIndex = shuffledList[listIndex];
                    listIndex++;
                }
                mediaMusic.Play();
            }
            else
            {
                shuffledList = new int[track_list.Items.Count];

                for (int i = 0; i < shuffledList.Length; i++)
                {
                    shuffledList[i] = i;
                }

                Shuffle(shuffledList);

                if (listIndex < shuffledList.Length - 1)
                {
                    track_list.SelectedIndex = shuffledList[listIndex];
                    listIndex++;
                }
                else if (listIndex == shuffledList.Length - 1)
                {
                    Shuffle(shuffledList);
                    listIndex = 0;
                    track_list.SelectedIndex = shuffledList[listIndex];
                    listIndex++;
                }
            }
        }

        private void MusicStopped(object sender, RoutedEventArgs e)
        {
            

            if (loopThisAudio)
            {

                mediaMusic.Source = new Uri(paths[track_list.SelectedIndex], UriKind.Relative);
                mediaMusic.Play();
            }else if (ShufflePlaylist)
            {
                PlayNextShuffle();
            }
            else
            {
                if (track_list.SelectedIndex < track_list.Items.Count - 1)
                {
                    track_list.SelectedIndex = track_list.SelectedIndex + 1;
                }
                else if (track_list.SelectedIndex == track_list.Items.Count - 1)
                {
                    track_list.SelectedIndex = 0;
                }
            }

            
        }

        private void CreditsText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://tekar.tk",
                UseShellExecute = true
            });
        }

        public static void Shuffle(int[] array)
        {
            var rng = new Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }


    }

}
