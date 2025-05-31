using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class SkillEffectData
    {
        public int start_time;
        public GameObject effect;
    }

    public class SkillHitboxData
    {
        public int start_time;
        public GameObject hitbox;

        public bool repeat;
        public int repeat_time;
    }
    public class SkillData
    {
        public List< SkillEffectData > effects;
        public List< SkillHitboxData > hitboxes;
    }
    public class Skill : Entity
    {
        public int actor_uid;
        public List< SkillData > data;
        public int duration;
        public int repeat_time_msec;
        public int level;
        public int skill_index;
        public List< Skill > ins_list = new List< Skill >();

        public GameObject obj;
        public Collider2D collider_compo;

        public SortedSet< int > hitted_targets_set = new SortedSet< int >();

        //충돌 후 퍼지는 형태라면?

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /**
         * return value - true일 경우 한 번만 실행하고 매니저에서 삭제됨
         **/
        public virtual bool active()
        {
            return true;
        }
        public virtual void die()
        {
            hitted_targets_set.Clear();
            release();
        }

        public virtual void destroyInsList()
        {
            int i = ins_list.Count - 1;
            for( ; i > -1 ; --i )
            {
                ins_list[ i ].release();
                ins_list.RemoveAt( i );
            }
        }

        public SkillDetailData getTableData()
        {
            return TableManager.Instance.Get< SkillDetailData >( skill_index );
        }
    }
}