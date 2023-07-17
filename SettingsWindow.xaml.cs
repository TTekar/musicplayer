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

            int playlistNum = 0;

            foreach (var file in files)
            {
                if (System.IO.Path.GetFileNameWithoutExtension(file) == "temp")
                {
                    continue;
                }
                var playlistAdd = file.Split("\\").Last();
                playlistAdd = playlistAdd.Substring(0, playlistAdd.LastIndexOf("."));
                //playlist_List.Items.Add($"Playlist {Int32.Parse(playlistAdd) + 1}");
                playlist_List.Items.Add(playlistAdd);

                playlists = playlists.Concat(new int[] { playlistNum }).ToArray();

                playlistNum++;
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
                using (StreamWriter sw = File.CreateText("data/Playlists/Playlist 1.txt")) ;
                playlist_List.Items.Add("Playlist 1");
            }
            else
            {
                int last = playlists.Last();
                int newPlaylist = last + 1;
                playlists = playlists.Concat(new int[] { newPlaylist }).ToArray();
                using (StreamWriter sw = File.CreateText($"data/Playlists/Playlist {newPlaylist +1}.txt")) ;
                playlist_List.Items.Add($"Playlist {newPlaylist + 1}");
            }

        }

        private void btnClick_DeletePlaylist(object sender, RoutedEventArgs e)
        {
            if(playlists.Length != 0)
            {

                try
                {
                    int indexToRemove = playlist_List.SelectedIndex;

                    File.Delete($"data/Playlists/{playlist_List.Items[indexToRemove]}.txt");

                    playlist_List.Items.Remove($"{playlist_List.Items[indexToRemove]}");

                    List<int> tempList = playlists.ToList();
                    tempList.RemoveAt(indexToRemove);
                    playlists = tempList.ToArray();

                }
                catch
                {

                }
                
            }
        }

        private void ShowPlaylist(object sender, SelectionChangedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;

            musicInPlaylist_list.Items.Clear();

            try
            {
                using (StreamReader sr = new StreamReader($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt"))
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
                TextReader tr = new StreamReader($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt");
                string text = tr.ReadToEnd();
                tr.Close();

                TextWriter tw = new StreamWriter($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt");

                
        
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

            try
            {
                musicInPlaylist_list.Items.RemoveAt(toRemove);
            }catch
            {

            }


            string line = null;
            int line_number = 0;
            int line_to_delete = toRemove+1;

            using (StreamReader reader = new StreamReader($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt"))
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

            File.Delete($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt");
            File.Move($"data/Playlists/temp.txt", $"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt");
            File.Delete($"data/Playlists/temp.txt");


        }

        private void btnClick_ClearPlaylistSongs(object sender, RoutedEventArgs e)
        {
            int currentPlaylist = playlist_List.SelectedIndex;

            File.Delete($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt");
            using (StreamWriter sw = File.CreateText($"data/Playlists/{playlist_List.Items[currentPlaylist]}.txt"))
            musicInPlaylist_list.Items.Clear();
        }
    }
}
