using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace game
{
    public class SkillManager
    {
        //스킬 리스트에서 배운 것들을 들고 있고
        //이걸 순회하면서 스킬 각기 업데이트를 한다
        public Dictionary< int, Skill > skill_dic = new Dictionary< int, Skill >();

        // Update is called once per frame
        public void Update( int current_game_time )
        {
            List< int > keys = skill_dic.Keys.ToList();
            int i = keys.Count - 1;
            for( ; i > -1 ; --i )
            {
                int key = keys[ i ];
                Skill skill = skill_dic[ key ];
                if( skill != null )
                {
                    if( current_game_time > skill.repeat_time_msec )
                    {
                        bool is_once_skill = skill.active();
                        if( is_once_skill )
                            skill_dic.Remove( key );
                    }
                }
            }
        }

        //리스트에 스킬을 add
        public void learnSkill( int skill_index, int actor_uid )
        {
            SkillDetailData skill_data = TableManager.Instance.Get< SkillDetailData >( skill_index );
            if( skill_data == null )
                return;

            string skill_prefab_address = skill_data.asset_address;
            GameManager.prefabmgr.prefabs.TryGetValue( skill_prefab_address, out GameObject obj );
            if( obj == null )
                return;

            //여기서 프리팹을 불러오게 된다면 메모리에 계속 올라가있는 상태이므로 변수들이 값이 바뀌어진 상태로 계속 접근하게 된다.
            //인스턴스화 해서 새로 만들게 해야 된다.
            GameObject ins = GameObject.Instantiate( obj );
            skill_dic.TryGetValue( skill_index, out Skill skill );
            if( skill != null )
            {
                skill_dic.Remove( skill_index );
            }

            Skill script = (skill != null) ? skill : ins.GetComponent< Skill >();
            script.actor_uid = actor_uid;
            script.level = script.level + 1;
            script.skill_index = skill_index;
            script.repeat_time_msec = 0;

            skill_dic.Add( skill_index, script );
        }

        public void clearSkill()
        {
            List< int > keys = skill_dic.Keys.ToList();
            int i = keys.Count - 1;
            for( ; i > -1 ; --i )
            {
                int key = keys[ i ];
                Skill skill = skill_dic[ key ];
                if( skill != null )
                {
                    skill.destroyInsList();
                    skill_dic.Remove( key );
                }
            }
        }
    }
}