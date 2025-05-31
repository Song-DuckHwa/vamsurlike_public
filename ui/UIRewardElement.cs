using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    public class UIRewardElement : MonoBehaviour
    {
        public TextMeshProUGUI description;
        public Image icon;
        public TextMeshProUGUI level_text;

        public void setData( ItemData reward_data )
        {
            Color bg_color = new Color( 0f, 0f, 0f, 0.50f );
            GetComponent< Image >().color = bg_color;
            SkillDetailData skill_data = TableManager.Instance.Get< SkillDetailData >( reward_data.skill_index );
            if( skill_data == null )
            {
                Debug.Log( "not has skill data" );
                return;
            }

            if( GameManager.prefabmgr.prefabs.TryGetValue( skill_data.asset_address, out GameObject obj ) == false )
                return;

            SpriteRenderer spr_renderer = obj.GetComponent< SpriteRenderer >();
            Sprite spr = Instantiate( spr_renderer.sprite );
            icon.sprite = spr;
            icon.GetComponent< Image >().color = Color.white;
            description.text = skill_data.description[ reward_data.level ];
            level_text.text = $"LV.{reward_data.level + 1}";
        }
    }
}