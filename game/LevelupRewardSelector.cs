using System.Collections.Generic;
using UnityEngine;

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
            List< int > pc_can_learn_skills = new List< int >( GameManager.tablemgr.PCCanLearnSkills );
            List< int > item_uid = new List< int >( GameManager.tablemgr.ItemSkills );

            int loop_max = GameManager.gamelogic.uimgr.ui_levelup.rewardbutton_list.Count;
            for( ; reward_list.Count < loop_max ; )
            {
                int random_index = 0;
                int random_skill_index = 0;
                bool add_item = false;

                int generate_item = Random.Range( 0, 100 );

                //더 이상 배울 스킬이 없을 경우 아이템으로 대체
                //10% 확률로 아이템으로 대체
                if( pc_can_learn_skills.Count != 0 && generate_item > 9 )
                {
                    random_index = Random.Range( 0, pc_can_learn_skills.Count );
                    random_skill_index = pc_can_learn_skills[ random_index ];
                }
                else
                {
                    random_index = Random.Range( 0, item_uid.Count );
                    random_skill_index = item_uid[ random_index ];
                    add_item = true;
                }

                GameManager.gamelogic.inventory.inven.TryGetValue( random_skill_index , out ItemData inven );

                ItemData reward = new ItemData();
                reward.skill_index = (inven != null) ? inven.skill_index : random_skill_index;
                reward.level = (inven != null) ? inven.level : -1;

                SkillDetailData skill = GameManager.tablemgr.Get< SkillDetailData >( random_skill_index );
                if( skill == null )
                {
#if UNITY_EDITOR
                    Debug.Log( $"cant find skill data - {random_skill_index}" );
#endif
                    break;
                }

                if( add_item == false )
                {
                    int max_level_data = skill.level_data.Count;
                    if( (reward.level + 1) >= max_level_data )
                    {
                        pc_can_learn_skills.RemoveAt( random_index );
                        continue;
                    }

                    reward.level += 1;
                    pc_can_learn_skills.RemoveAt( random_index );
                }
                else
                {
                    reward.level = 0;
                }

                reward_list.Add( reward );
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