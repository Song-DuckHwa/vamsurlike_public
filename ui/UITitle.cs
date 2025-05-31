using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace game
{
    public class StartScene : MonoBehaviour
    {
        public Button start_button;
        public Button quit_button;

        // Start is called before the first frame update
        void Start()
        {
            start_button.onClick.AddListener( OnStartClick );
            quit_button.onClick.AddListener( OnQuitClick );
        }

        void OnStartClick()
        {
            SceneManager.LoadScene( "GameManagerSetting" );
        }

        void OnQuitClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationQuit()
        {
            Application.wantsToQuit += AppQuit;
        }

        private bool AppQuit()
        {
#if UNITY_EDITOR
#else
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif

            return true;
        }
    }
}