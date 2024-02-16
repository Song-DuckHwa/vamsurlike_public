using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace game
{
	/**
	* UILevelup
	* 레벨업 시 나오는 스탯, 보상을 보여주는 전체 화면 ui
	**/
    public class UILevelup : MonoBehaviour
    {
        public List< Button > rewardbutton_list = new List< Button >();
        public Dictionary< string, TextMeshProUGUI > stat_list = new Dictionary< string, TextMeshProUGUI >();

        public GameObject stat_value;
        public Image bg;

        public void init()
        {
            GameManager.gamelogic.uimgr.ui_levelup.gameObject.SetActive( false );

            for( int i = 0 ; i < rewardbutton_list.Count ; ++i )
                rewardbutton_list[ i ].onClick.RemoveAllListeners();

			//리스너 등록
            rewardbutton_list[ 0 ].onClick.AddListener( () => OnRewardClick( 0 ) );
            rewardbutton_list[ 1 ].onClick.AddListener( () => OnRewardClick( 1 ) );
            rewardbutton_list[ 2 ].onClick.AddListener( () => OnRewardClick( 2 ) );
            rewardbutton_list[ 3 ].onClick.AddListener( () => OnRewardClick( 3 ) );
            rewardbutton_list[ 4 ].onClick.AddListener( () => OnRewardClick( 4 ) );

            stat_list.Clear();
            stat_list.Add( stat_value.transform.GetChild( 0 ).name, stat_value.transform.GetChild( 0 ).gameObject.GetComponent< TextMeshProUGUI >() );
            stat_list.Add( stat_value.transform.GetChild( 1 ).name, stat_value.transform.GetChild( 1 ).gameObject.GetComponent< TextMeshProUGUI >() );
            stat_list.Add( stat_value.transform.GetChild( 2 ).name, stat_value.transform.GetChild( 2 ).gameObject.GetComponent< TextMeshProUGUI >() );
            stat_list.Add( stat_value.transform.GetChild( 3 ).name, stat_value.transform.GetChild( 3 ).gameObject.GetComponent< TextMeshProUGUI >() );
            stat_list.Add( stat_value.transform.GetChild( 4 ).name, stat_value.transform.GetChild( 4 ).gameObject.GetComponent< TextMeshProUGUI >() );

            stat_list[ "hp" ].text = GameManager.mainch.hp.ToString();
            stat_list[ "atk" ].text = GameManager.mainch.atk.ToString();
            stat_list[ "spd" ].text = GameManager.mainch.move_speed.ToString();
            stat_list[ "atkspd" ].text = GameManager.mainch.attack_speed.ToString();
            stat_list[ "dist" ].text = GameManager.mainch.collect_dist.ToString();
        }

		/**
		* 보상 버튼 클릭 시 콜백
		**/
        public void OnRewardClick( int button_index )
        {
            GameManager.gamelogic.addItemMainPC( button_index );
            GameManager.soundmgr.sfxs[ SFX.BUTTONCLICK ].Play();
        }

		/**
		* 보상 버튼에 보상 데이터를 배치
		**/
        public void setRewards( List< ItemData > reward_list )
        {
            int i = 0;
            int loop_max = reward_list.Count;
            for( ; i < loop_max ; ++i )
            {
                rewardbutton_list[ i ].GetComponent< UIRewardElement >().setData( reward_list[ i ] );
            }
        }

		/**
		* 왼쪽 캐릭터 스탯 갱신
		**/
        public void refreshStat()
        {
            stat_list[ "hp" ].text = GameManager.mainch.hp.ToString();
            stat_list[ "atk" ].text = GameManager.mainch.atk.ToString();
            stat_list[ "spd" ].text = GameManager.mainch.move_speed.ToString();
            stat_list[ "atkspd" ].text = GameManager.mainch.attack_speed.ToString();
            stat_list[ "dist" ].text = GameManager.mainch.collect_dist.ToString();
        }
    }
}