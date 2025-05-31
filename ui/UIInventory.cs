using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    public class UIInventory : MonoBehaviour
    {
        //게임 오브젝트가 아니라 아이템 데이터가 있어야 함
        public List< GameObject > weapon_list;
        public List< GameObject > item_list;

        Sprite empty_inven_spr;
        Color empty_inven_color;

        public void init()
        {
            if( empty_inven_spr == null )
            {
                Image img = weapon_list[ 0 ].GetComponent< Image >();
                empty_inven_spr = img.sprite;
                empty_inven_color = new Color( img.color.r, img.color.g, img.color.b, img.color.a );
            }

            for( int i = 0 ; i < weapon_list.Count ; ++i )
            {
                Image img = weapon_list[ i ].GetComponent< Image >();
                img.sprite = empty_inven_spr;
                img.color = new Color( empty_inven_color.r, empty_inven_color.g, empty_inven_color.b, empty_inven_color.a );
            }
        }

        public void add( int inven_index, int skill_index )
        {
            SkillDetailData skill_data = TableManager.Instance.Get< SkillDetailData >( skill_index );
            if( skill_data == null )
            {
                Debug.Log( $"can't found skill data - {skill_index}" );
                return;
            }

            GameManager.prefabmgr.prefabs.TryGetValue( skill_data.asset_address, out GameObject obj );
            if( obj == null )
            {
                Debug.Log( $"can't found skill asset - {skill_data.asset_address}" );
                return;
            }

            SpriteRenderer spr_renderer = obj.GetComponent< SpriteRenderer >();
            Sprite spr = Instantiate( spr_renderer.sprite );
            weapon_list[ inven_index ].GetComponent< Image >().sprite = spr;
            weapon_list[ inven_index ].GetComponent< Image >().color = Color.white;
        }
    }
}