﻿
using csvorbis;
using System.Collections.Generic;
using System.IO;
using MelonLoader.TinyJSON;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using MelonLoader;

namespace AudicaModding
{
    internal static class FilterPanel
    {
        static GameObject glass;
        static GameObject highlights;
        static GameObject filterButton;
        static GameObject favoritesButton;
        static GameObject searchButton;

        public static GameObject notificationPanel;
        static TextMeshPro notificationText;

        private static GameObject favoritesButtonSelectedIndicator;
        private static GameObject searchButtonSelectedIndicator;

        public static Favorites favorites;

        public static bool firstTime = true;

        public static bool filteringFavorites { get; private set; }
        public static bool filteringSearch    { get; private set; }

        static string favoritesPath = Application.dataPath + "/../" + "/UserData/"+ "SongBrowserFavorites.json";

        private static SongSelect       songSelect       = null;
        private static SongListControls songListControls = null;

        public static void Initialize()
        {
            if (firstTime)
            {
                songSelect       = GameObject.FindObjectOfType<SongSelect>();
                songListControls = GameObject.FindObjectOfType<SongListControls>();
                GetReferences();
                firstTime = false;
                notificationPanel.transform.localPosition = new Vector3(0f, -17.5f, 0f);
                glass.transform.localScale = new Vector3(10.9f, 20.52f, 3);
                glass.transform.localPosition = new Vector3(0f, -4.27f, 0.15f);
                highlights.transform.localScale = new Vector3(1f, 1.4272f, 1f);
                highlights.transform.localPosition = new Vector3(0f, -14.36f, 0f);
                PrepareSearchButton();
                searchButtonSelectedIndicator = searchButton.transform.GetChild(3).gameObject;
                searchButtonSelectedIndicator.SetActive(false);
                PrepareFavoritesButton();
                favoritesButtonSelectedIndicator = favoritesButton.transform.GetChild(3).gameObject;
                favoritesButtonSelectedIndicator.SetActive(false);
                filterButton.GetComponentInChildren<GunButton>().onHitEvent.AddListener(new Action(() => 
                { 
                    DisableCustomFilters();
                    songSelect.ShowSongList();
                }));
            }
        }

        private static void PrepareSearchButton()
        {
            searchButton = GameObject.Instantiate(filterButton, filterButton.transform.parent);
            searchButton.transform.localPosition = new Vector3(0f, -8.09f, 0f);
            GameObject.Destroy(searchButton.GetComponentInChildren<Localizer>());
            GunButton button = searchButton.GetComponentInChildren<GunButton>();
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { FilterSearch(); }));
            searchButton.GetComponentInChildren<TextMeshPro>().text = "search";
        }

        private static void PrepareFavoritesButton()
        {
            favoritesButton = GameObject.Instantiate(filterButton, filterButton.transform.parent);
            favoritesButton.transform.localPosition = new Vector3(0f, -11.65f, 0f);
            GameObject.Destroy(favoritesButton.GetComponentInChildren<Localizer>());
            GunButton button = favoritesButton.GetComponentInChildren<GunButton>();
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { FilterFavorites(); }));
            favoritesButton.GetComponentInChildren<TextMeshPro>().text = "favorites";
        }

        public static void DisableCustomFilters()
        {
            DisableFavoritesFilter();
            DisableSearchFilter();
        }
        private static void DisableFavoritesFilter()
        {
            filteringFavorites = false;
            favoritesButtonSelectedIndicator.SetActive(false);
        }
        private static void DisableSearchFilter()
        {
            filteringSearch = false;
            searchButtonSelectedIndicator.SetActive(false);
            SongSearchButton.HideSearchButton();
        }

        public static void FilterSearch()
        {
            songListControls.FilterExtras(); // this seems to fix duplicated songs;
            if (!filteringSearch)
            {
                filteringSearch = true;
                searchButtonSelectedIndicator.SetActive(true);
                SongSearchButton.ShowSearchButton();

                DisableFavoritesFilter();
            }
            else
            {
                DisableSearchFilter();
            }
            songSelect.ShowSongList();
        }

        public static void FilterFavorites()
        {
            songListControls.FilterExtras(); // this seems to fix duplicated songs;
            if (!filteringFavorites)
            {
                filteringFavorites = true;
                favoritesButtonSelectedIndicator.SetActive(true);

                DisableSearchFilter();
            }
            else
            {
                DisableFavoritesFilter();
            }
            songSelect.ShowSongList();
        }

        public static void GetReferences()
        {
            glass = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/Glass");
            highlights = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/PanelFrame/highlights");
            filterButton = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/FilterExtras");
            notificationPanel = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/ShellPanel_SongListNotification");
            notificationText = notificationPanel.GetComponentInChildren<TextMeshPro>();
        }

        public static void SetNotificationText(string text)
        {
            if (notificationText != null)
            {
                notificationText.text = text; 
            }
        }

        public static void LoadFavorites()
        {
            if (File.Exists(favoritesPath))
            {
                string text = File.ReadAllText(favoritesPath);
                favorites = JSON.Load(text).Make<Favorites>();
            }
            else
            {
                favorites = new Favorites();
                favorites.songIDs = new List<string>();
            }
        }

        public static void SaveFavorites()
        {
            string text = JSON.Dump(favorites);
            File.WriteAllText(favoritesPath, text);
        }

        public static bool IsFavorite(string songID)
        {
            return favorites.songIDs.Contains(songID);
        }

        public static void AddFavorite(string songID)
        {
            var song = SongList.I.GetSong(songID);
            if (!song.extrasSong) return;
            if (favorites.songIDs.Contains(songID))
            {
                RandomSong.FavouritesChanged(songID, false);
                favorites.songIDs.Remove(songID);
                SongBrowser.DebugText($"Removed {song.title} from favorites!");
                SaveFavorites();
            }
            else
            {
                RandomSong.FavouritesChanged(songID, true);
                favorites.songIDs.Add(songID);
                SongBrowser.DebugText($"Added {song.title} to favorites!");
                SaveFavorites();
            }

            
        }
    }
}

[Serializable]
class Favorites
{
    public List<string> songIDs;
}