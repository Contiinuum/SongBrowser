using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;

namespace SongBrowser.src
{
    public class RandomSong : MelonMod
    {
        private static int randomSongBagSize = 10;
        private static int mainSongCount = 33;
        private static bool availableSongListsSetup = false;
        private static List<int> availableMainSongs = new List<int>();
        private static List<int> availableExtrasSongs = new List<int>();
        private static List<int> lastPickedSongs = new List<int>();
        private static List<int> availableSongs = new List<int>();

        private static SongSelect songSelect = null;

        private static Il2CppSystem.Collections.Generic.List<SongSelectItem> songs = new Il2CppSystem.Collections.Generic.List<SongSelectItem>();

        private void CreateConfig()
        {
            ModPrefs.RegisterPrefInt("SongBrowser", "RandomSongBagSize", randomSongBagSize);

        }

        private void LoadConfig()
        {
            randomSongBagSize = ModPrefs.GetInt("SongBrowser", "RandomSongBagSize");
            if (randomSongBagSize > mainSongCount) randomSongBagSize = mainSongCount;

        }

        public static void SaveConfig()
        {
            ModPrefs.SetInt("SongBrowser", "RandomSongBagSize", randomSongBagSize);
        }

        public override void OnLevelWasLoaded(int level)
        {

            if (!ModPrefs.HasKey("RandomSong", "RandomSongBagSize"))
            {
                CreateConfig();
            }
            else
            {
                LoadConfig();

            }
        }

        public static void GetRandomSong()
        {
            songSelect = GameObject.FindObjectOfType<SongSelect>();
            songs = songSelect.songSelectItems.mItems;
            int maxLength = songs.Count - 1;
            if (!availableSongListsSetup)
            {
                availableSongListsSetup = true;

                for (int i = 0; i < mainSongCount; i++)
                {
                    availableMainSongs.Add(i);
                }

                for (int i = mainSongCount; i < maxLength; i++)
                {
                    availableExtrasSongs.Add(i);
                }

                for (int i = 0; i < maxLength; i++)
                {
                    availableSongs.Add(i);
                }
            }
            SongSelect.Filter filter = songSelect.GetListFilter();

            var rand = new System.Random();
            int index;
            if (filter == SongSelect.Filter.All)
            {
                index = availableSongs[rand.Next(0, availableSongs.Count - 1)];
            }
            else if (filter == SongSelect.Filter.Main)
            {
                index = availableMainSongs[rand.Next(0, availableMainSongs.Count - 1)];
                if (availableMainSongs.Count > 0) availableMainSongs.Remove(index);
            }
            else
            {
                index = availableExtrasSongs[rand.Next(0, availableExtrasSongs.Count - 1)];
                if (availableExtrasSongs.Count > 0) availableExtrasSongs.Remove(index);
            }
            songs[index].OnSelect();
            lastPickedSongs.Add(index);
            if (availableSongs.Count > 0) availableSongs.Remove(index);


            if (lastPickedSongs.Count > randomSongBagSize)
            {
                int oldestIndex = lastPickedSongs[0];
                lastPickedSongs.Remove(oldestIndex);
                availableSongs.Add(oldestIndex);
                if (oldestIndex < 33) availableMainSongs.Add(index);
                else availableExtrasSongs.Add(index);
            }
        }
    }


}
