using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using MelonLoader;
using System.Collections;

namespace SongBrowser.src.UI.Buttons
{
    internal static class RandomSongButton
    {
        public static bool panelButtonsCreated = false;
        public static bool buttonsBeingCreated = false;

        private static GameObject filterMainButton = null;
        private static GameObject randomSongButton = null;

        private static Vector3 randomButtonPos = new Vector3(10.2f, -9.4f, 24.2f);
        private static Vector3 randomButtonRot = new Vector3(0f, 0f, 0f);
        private static Vector3 randomButtonScale = new Vector3(2f, 2f, 2f);

        public static void CreateSongPanelButton()
        {
            buttonsBeingCreated = true;
            filterMainButton = GameObject.FindObjectOfType<MainMenuPanel>().buttons[1];

            randomSongButton = CreateButton(filterMainButton, "Random Song", OnRandomSongShot, randomButtonPos, randomButtonRot, randomButtonScale);
            panelButtonsCreated = true;
            SetRandomSongButtonActive(true);
        }

        private static GameObject CreateButton(GameObject buttonPrefab, string label, Action onHit, Vector3 position, Vector3 eulerRotation, Vector3 scale)
        {
            GameObject buttonObject = UnityEngine.Object.Instantiate(buttonPrefab);
            buttonObject.transform.rotation = Quaternion.Euler(eulerRotation);
            buttonObject.transform.position = position;
            buttonObject.transform.localScale = scale;

            UnityEngine.Object.Destroy(buttonObject.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = buttonObject.GetComponentInChildren<TextMeshPro>();
            buttonText.text = label;
            GunButton button = buttonObject.GetComponentInChildren<GunButton>();
            button.destroyOnShot = false;
            button.disableOnShot = false;
            button.SetSelected(false);
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(onHit);

            return buttonObject.gameObject;
        }

        public static IEnumerator SetRandomSongButtonActive(bool active)
        {
            if (active) yield return new WaitForSeconds(.65f);
            else yield return null;
            randomSongButton.SetActive(active);
        }

        private static void OnRandomSongShot()
        {
            RandomSong.GetRandomSong();
        }
    }
}
