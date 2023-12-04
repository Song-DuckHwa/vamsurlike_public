using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class Orbit : Skill
    {
        public float pos_deg;
        public int move_speed;
        private int knockback_dist = 20;

        private void FixedUpdate()
        {
            calcOrbitPos();
        }

        // Update is called once per frame
        void Update()
        {
            //데미지 주기 조절 생각해야함
            //이렇게 되면 업데이트 될때마다 공격함
            List< int > hitted_targets = GameManager.charmgr.attackCollideCheck( (CircleCollider2D)collider_compo );
            if( hitted_targets.Count > 0 )
            {
                int i = 0;
                int loop_max = hitted_targets.Count;
                for( ; i < loop_max ; ++i )
                {
                    NPC ch = GameManager.charmgr.find( hitted_targets[ i ] );
                    if( ch != null )
                    {
                        NPC actor = GameManager.charmgr.find( actor_uid );
                        if( actor == null )
                            return;

                        ch.takeDamage( actor.gameObject, knockback_dist );
                    }
                }
            }
        }
        public override bool active()
        {
            SkillDetailData table = getTableData();
            if( table == null )
                return true;

            destroyInsList();

            NPC actor = GameManager.charmgr.find( actor_uid );
            if( actor == null )
                return true ;

            SkillLevelData table_level_data = table.level_data[ level - 1 ];

            int i = 0;
            int loop_max = table_level_data.objcount;
            for( ; i < loop_max ; ++i )
            {
                GameObject ins = GameManager.poolmgr.instanceGet( table.asset_address );
                if( ins == null )
                    return true;

                Orbit script = ins.GetComponent< Orbit >();
                //magic
                script.move_speed = 100;
                script.collider_compo = ins.GetComponent< CircleCollider2D >();

                float deg = i * (360 / loop_max);
                script.pos_deg = deg;
                float rad = deg * Mathf.Deg2Rad;
                Vector3 hitarea_pos = new Vector3();
                //magic
                int hitarea_dist = 100;
                hitarea_pos.x = hitarea_dist * Mathf.Cos( rad );
                hitarea_pos.y = hitarea_dist * Mathf.Sin( rad );

                ins.transform.localPosition = hitarea_pos + actor.transform.position;
                ins.transform.SetParent( actor.transform );
                ins.SetActive( true );

                ins_list.Add( script );
            }

            //magic
            repeat_time_msec += 99999999;

            return false;
        }

        public void calcOrbitPos()
        {
            pos_deg += move_speed * Time.deltaTime;
            if( pos_deg >= 360 )
                pos_deg = 0f;

            float rad = pos_deg * Mathf.Deg2Rad;
            Vector2 hitarea_pos = new Vector2();
            //magic
            int hitarea_dist = 100;
            hitarea_pos.x = hitarea_dist * Mathf.Cos( rad );
            hitarea_pos.y = hitarea_dist * Mathf.Sin( rad );

            transform.localPosition = hitarea_pos;
        }
    }
}