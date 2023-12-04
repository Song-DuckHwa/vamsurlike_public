using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace game
{
    public class UILevelup : MonoBehaviour
    {
        public List< Button > rewardbutton_list = new List< Button >();

        public Dictionary< string, TextMeshProUGUI > stat_list = new Dictionary< string, TextMeshProUGUI >();

        public GameObject stat_value;

        public Image bg;

        // Start is called before the first frame update
        void Start()
        {
            bg.rectTransform.sizeDelta = new Vector2( GameManager.instance.res_v_half * 2, GameManager.instance.res_h_half * 2 );

            rewardbutton_list[ 0 ].onClick.AddListener( () => OnRewardClick( 0 ) );
            rewardbutton_list[ 1 ].onClick.AddListener( () => OnRewardClick( 1 ) );
            rewardbutton_list[ 2 ].onClick.AddListener( () => OnRewardClick( 2 ) );


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

        public void OnRewardClick( int button_index )
        {
            GameManager.gamelogic.addItemMainPC( button_index );
        }

        public void init()
        {
            GameManager.gamelogic.uimgr.ui_levelup.gameObject.SetActive( false );
        }

        public void setRewards( List< ItemData > reward_list )
        {
            int i = 0;
            int loop_max = reward_list.Count;
            for( ; i < loop_max ; ++i )
            {
                rewardbutton_list[ i ].GetComponent< Image >().color = Color.clear;
                GameObject icon = rewardbutton_list[ i ].GetComponent< UIRewardElement >().icon;
                GameManager.tablemgr.skill.data.TryGetValue( reward_list[ i ].skill_index, out SkillDetailData skill_data );
                if( skill_data == null )
                {
                    Debug.Log( "not has skill data" );
                    continue;
                }

                GameManager.prefabmgr.prefabs.TryGetValue( skill_data.asset_address, out GameObject obj );
                if( obj == null )
                    continue;

                SpriteRenderer spr_renderer = obj.GetComponent< SpriteRenderer >();
                Sprite spr = Instantiate( spr_renderer.sprite );
                icon.GetComponent< Image >().sprite = spr;
                icon.GetComponent< Image >().color = Color.white;
                TextMeshProUGUI desc = rewardbutton_list[ i ].GetComponent< UIRewardElement >().description;
                desc.text = skill_data.description[ reward_list[ i ].level ];
            }
        }
    }
}