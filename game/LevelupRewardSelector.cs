using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace game
{
	/**
	* LevelupRewardSelector
	* 레벨업 시 보상 아이템 목록을 추출하는 클래스
	**/
    public class LevelupRewardSelector
    {
        public List< ItemData > reward_list = new List< ItemData >();

		/**
		* 보상 목록 추출
		**/
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

				//스킬의 만렙이 7이므로 (6인 이유는 0부터 시작해서) 7인 스킬이 뽑혔을 경우 다른 스킬을 찾는다.
                const int MAX_SKILL_LEVEL = 6;
                if( reward.level == MAX_SKILL_LEVEL )
                {
                    keys.RemoveAt( random_index );
                    continue;
                }

				//그 외인 경우 레벨을 1씩 증가시킨다.
                reward.level += 1;

                reward_list.Add( reward );
                keys.RemoveAt( random_index );
            }
        }

		/**
		* 보상 목록 삭제 후 다시 추가
		**/
        public void reset()
        {
            clear();
            add();
        }

		/**
		* 보상 목록 삭제
		**/
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