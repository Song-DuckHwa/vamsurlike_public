using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace game
{
    public class LevelupRewardSelector
    {
        public List< ItemData > reward_list = new List< ItemData >();

        public void add()
        {
            List< int > keys = GameManager.tablemgr.skill.data.Keys.ToList();

            int loop_max = GameManager.gamelogic.uimgr.ui_levelup.rewardbutton_list.Count;
            for( ; reward_list.Count < loop_max ; )
            {
                int random_index = Random.Range( 0, keys.Count );

                GameManager.gamelogic.inventory.inven.TryGetValue( keys[ random_index ] , out ItemData inven );

                ItemData reward = new ItemData();
                reward.skill_index = (inven != null) ? inven.skill_index : keys[ random_index ];
                reward.level = (inven != null) ? inven.level : -1;


                const int MAX_SKILL_LEVEL = 6;
                if( reward.level == MAX_SKILL_LEVEL )
                {
                    keys.RemoveAt( random_index );
                    continue;
                }

                reward.level += 1;

                reward_list.Add( reward );
                keys.RemoveAt( random_index );
            }
        }

        public void reset()
        {
            clear();
            add();
        }

        public void clear()
        {
            int i = reward_list.Count - 1;
            for( ; i > -1 ; --i )
            {
                reward_list.RemoveAt( i );
            }
        }
    }
}