using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace game
{
    public class ItemData
    {
        public int skill_index = 0;
        public int level = -1;
    }
    public class Inventory 
    {
        public Dictionary< int, ItemData > inven = new Dictionary< int, ItemData >();

        public void add( int skill_index )
        {
            ItemData data = new ItemData();
            data.level = 0;
            data.skill_index = skill_index;

            inven.Add( skill_index, data );
            GameManager.gamelogic.uimgr.ui_inventory.add( inven.Count - 1, skill_index );            
        }

        public void levelUp( int skill_index )
        {
            inven.TryGetValue( skill_index, out ItemData data );
            if( data == null )
            {
                add( skill_index );
                return;
            }                

            data.level += 1;
        }

        public void clear()
        {
            int i = 0;
            List< int > keys = inven.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                inven.Remove( key_i );
            }
        }
    }
}