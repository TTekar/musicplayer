using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicPlayer
{
    public partial class SettingsWindow : Window
    {


        public SettingsWindow()
        {

            InitializeComponent();

            MainWindow mainWin = new MainWindow();

            string[] files = Directory.GetFiles(@"data\Playlists", "*.txt");

            foreach (var file in files)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(file) == "temp")
                {
                    continue;
                }
                var playlistAdd = file.Split("\\").Last();
                playlistAdd = playlistAdd.Substring(0, playlistAdd.LastIndexOf("."));
                playlist_List.Items.Add($"Playlist {Int32.Parse(playlistAdd) + 1}");

                playlists = playlists.Concat(new int[] { Int32.Parse(playlistAdd) }).ToArray();
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClick_Close(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }



        public static string[] paths, files;


        

        public static int[] playlists = {};

        private void btnClick_MakePlaylist(object sender, RoutedEventArgs e)
        {
            if (playlists.Length == 0)
            {
                playlists = playlists.Concat(new int[] { 0 }).ToArray();
                using (StreamWriter sw = File.CreateText("data/Playlists/0.txt")) ;
                playlist_List.Items.Add("Playlist 1");
            }
            else
            {
                int last = playlists.Last();
                int newPlaylist = last + 1;
                playlists = playlists.Concat(new int[] { newPlaylist }).ToArray();
                using (StreamWriter sw = File.CreateText($"data/Playlists/{newPlaylist}.txt")) ;
                playlist_List.Items.Add($"Playlist {newPlaylist + 1}");
            }

        }

        private void btnClick_DeletePlaylist(object sender, RoutedEventArgs e)
        {
            if(playlists.Length != 0)
            {
                int last = playlists.Last();

                playlists = playlists.Where(e => e != last).ToArray();

                File.Delete($"data/Playlists/{last}.txt");

                if (playlist_List.Items.Count == 1)
                {
                    playlist_List.Items.RemoveAt(0);
                }
                playlist_List.Items.Remove($"Playlist {last+1}");
            }
        }

        private void ShowPlaylist(object sender, SelectionChangedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;

            musicInPlaylist_list.Items.Clear();

            try
            {
                using (StreamReader sr = new StreamReader($"data/Playlists/{currentPlaylist}.txt"))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        string songArr = line.Split("\\").Last();
                        musicInPlaylist_list.Items.Add(songArr);
                    }
                }
            }
            catch
            {

            }
            
        }

        private void btnClick_AddSongToPlaylist(object sender, RoutedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Mp3 Files|*.mp3|Avi Files|*.avi|Wav Files|*.wav";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextReader tr = new StreamReader($"data/Playlists/{currentPlaylist}.txt");
                string text = tr.ReadToEnd();
                tr.Close();

                TextWriter tw = new StreamWriter($"data/Playlists/{currentPlaylist}.txt");

                
        
                paths = ofd.FileNames;
                files = ofd.FileNames;

                tw.Write(text);
        
                for (int x = 0; x < files.Length; x++)
                {
                    tw.WriteLine(files[x]);

                    string[] listPicesText = files[x].Split('\\');
                    string shortName = listPicesText.Last();

                    musicInPlaylist_list.Items.Add(shortName);

                }

                tw.Close();

                text = "";
            }
        }

        private void btnClick_RemoveSongFromPlaylist(object sender, RoutedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;
            int toRemove = musicInPlaylist_list.SelectedIndex;

            musicInPlaylist_list.Items.RemoveAt(toRemove);


            //string[] result = File.ReadAllLines($"data/Playlists/{currentPlaylist}.txt").ToArray();

            //string[] newPlaylist = { };


            //for (int x = 0; x < musicInPlaylist_list.Items.Count; x++)
            //{
            //    if (x == toRemove)
            //    {
            //        string removedSong = result[x];
            //    }else
            //    {
            //        newPlaylist.Concat(new string[] { result[x] }).ToArray();
            //    }

            //}

            string line = null;
            int line_number = 0;
            int line_to_delete = toRemove+1;

            using (StreamReader reader = new StreamReader($"data/Playlists/{currentPlaylist}.txt"))
            {
                using (StreamWriter writer = new StreamWriter($"data/Playlists/temp.txt"))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line_number++;

                        if (line_number == line_to_delete)
                            continue;

                        writer.WriteLine(line);
                    }
                }
            }

            File.Delete($"data/Playlists/{currentPlaylist}.txt");
            File.Move($"data/Playlists/temp.txt", $"data/Playlists/{currentPlaylist}.txt");
            File.Delete($"data/Playlists/temp.txt");

            //string tempFile = System.IO.Path.GetTempFileName();

            //using (var sr = new StreamReader($"data/Playlists/{currentPlaylist}.txt"))
            //using (var sw = new StreamWriter(tempFile))
            //{
            //    string line;

            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        if (line != "removeme")
            //            sw.WriteLine(line);
            //    }
            //}

            //File.Delete($"data/Playlists/{currentPlaylist}.txt");
            //File.Move(tempFile, $"data/Playlists/{currentPlaylist}.txt");

            //File.Delete($"data/Playlists/{currentPlaylist}.txt");

            //using (TextWriter writer = File.CreateText($"data/Playlists/{currentPlaylist}.txt"))
            //{

            //    foreach (string i in newPlaylist)
            //    {
            //        writer.WriteLine(i);
            //    }
            //}

        }

        private void btnClick_ClearPlaylistSongs(object sender, RoutedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;

            File.Delete($"data/Playlists/{currentPlaylist}.txt");
            using (StreamWriter sw = File.CreateText($"data/Playlists/{currentPlaylist}.txt"))
            musicInPlaylist_list.Items.Clear();
        }
    }
}
