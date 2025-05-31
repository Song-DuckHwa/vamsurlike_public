using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace game
{
    public class UIResult : MonoBehaviour
    {
        public TextMeshProUGUI result_text;
        public Button retry_button;
        public Button continue_button;
        public Button title_button;
        public Image bg;

        // Start is called before the first frame update
        void Start()
        {
            retry_button.onClick.AddListener( OnRetryClick );
            continue_button.onClick.AddListener( OnContinueClick );
            title_button.onClick.AddListener( OnTitleClick );
        }

        void OnRetryClick()
        {
            GameManager.gamelogic.restart();
        }

        void OnContinueClick()
        {
            GameManager.gamelogic.pauseGame( false );
            gameObject.SetActive( false );
        }

        void OnTitleClick()
        {
            GameManager.gamelogic.gotoTitle();
        }

        public void pause()
        {
            result_text.text = "Pause";
            retry_button.gameObject.SetActive( false );
            continue_button.gameObject.SetActive( true );
            gameObject.SetActive( true );
        }

        public void stageClear()
        {
            result_text.text = "Stage Clear";
            retry_button.gameObject.SetActive( true );
            continue_button.gameObject.SetActive( false );
            gameObject.SetActive( true );
        }

        public void gameOver()
        {
            result_text.text = "Game Over";
            retry_button.gameObject.SetActive( true );
            continue_button.gameObject.SetActive( false );
            gameObject.SetActive( true );
        }
    }
}