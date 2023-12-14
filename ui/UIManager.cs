using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    public class UIManager : MonoBehaviour
    {
        public Canvas canvas;

        public UIEXP exp_bar;
        public UIHP hp_bar;

        public TextMeshProUGUI time_text;
        public int game_start_time_msec;
        public int current_game_time_msec;

        public UIInventory ui_inventory;
        public UILevelup ui_levelup;
        public UIResult ui_result;

        public Dpad dpad;

        // Update is called once per frame
        void Update()
        {
            int minute = (int)(current_game_time_msec * 0.001) / 60;
            int second = (int)(current_game_time_msec * 0.001) % 60;
            string time_str = string.Format( "{0:00}:{1:00}",minute, second );
            time_text.text = time_str;
        }

        public void initUI()
        {
            exp_bar.init();
            exp_bar.level_text.text = $"LV. {exp_bar.level}";

            hp_bar.max_hp = GameManager.mainch.hp;
            hp_bar.current_hp = GameManager.mainch.current_hp;
            hp_bar.increaseHp( 0 );
            hp_bar.hp_text.text = $"{GameManager.mainch.current_hp} / {GameManager.mainch.hp}";

            ui_inventory.init();
            ui_levelup.init();
        }

        public void levelUp()
        {
            hp_bar.max_hp = GameManager.mainch.hp;
            hp_bar.current_hp = GameManager.mainch.current_hp;
            hp_bar.hp_text.text = $"{GameManager.mainch.current_hp} / {GameManager.mainch.hp}";
            hp_bar.increaseHp( 0 );

            exp_bar.levelup();

            ui_levelup.refreshStat();
            ui_levelup.setRewards( GameManager.gamelogic.levelup_reward_selector.reward_list );
            ui_levelup.gameObject.SetActive( true );
        }

        public void setCurrentTime( int game_time_msec )
        {
            current_game_time_msec = game_time_msec;
        }
    }
}