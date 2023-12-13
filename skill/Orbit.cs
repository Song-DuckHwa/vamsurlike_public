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

        public int attack_tick = 0;

        //uid, next tick
        private Dictionary< int, int > next_hit_ticks = new Dictionary< int, int >();

        private void FixedUpdate()
        {
            calcOrbitPos();
        }

        // Update is called once per frame
        void Update()
        {
            int current_game_time = GameManager.getCurrentGameTime();
            Npc actor = GameManager.charmgr.find( actor_uid );
            if( actor == null )
                return;

            List< int > hit_target_uids = GameManager.charmgr.attackCollideCheck( (CircleCollider2D)collider_compo );
            if( hit_target_uids.Count > 0 )
            {
                int i = 0;
                int loop_max = hit_target_uids.Count;
                for( ; i < loop_max ; ++i )
                {
                    int target_uid = hit_target_uids[ i ];
                    //collide mask로 변경해야 함
                    if( hit_target_uids[ i ] == actor_uid )
                        continue;

                    Npc ch = GameManager.charmgr.find( target_uid );
                    if( ch != null )
                    {
                        //이전에 때렸던 타겟이라면 다음 공격 틱이 되었는지 확인 하고 다시 때린다
                        //아니라면 그냥 때린다
                        if( next_hit_ticks.TryGetValue( target_uid, out int next_hit_tick ) == true )
                        {
                            if( current_game_time >= next_hit_tick )
                            {
                                ch.takeDamage( actor.gameObject, knockback_dist );
                                next_hit_ticks[ target_uid ] = current_game_time + attack_tick;
                            }
                        }
                        else
                        {
                            ch.takeDamage( actor.gameObject, knockback_dist );
                            next_hit_ticks.Add( target_uid, current_game_time + attack_tick );
                        }
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

            Npc actor = GameManager.charmgr.find( actor_uid );
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
                script.actor_uid = actor_uid;
                script.attack_tick = table_level_data.attack_tick;
                //magic
                script.move_speed = 100;
                script.collider_compo = ins.GetComponent< CircleCollider2D >();

                float deg = i * (360 / loop_max);
                script.pos_deg = deg;
                float rad = deg * Mathf.Deg2Rad;
                Vector3 hitarea_pos = new Vector3();
                //magic
                int hitarea_dist = 300;
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
            int hitarea_dist = 300;
            hitarea_pos.x = hitarea_dist * Mathf.Cos( rad );
            hitarea_pos.y = hitarea_dist * Mathf.Sin( rad );

            transform.localPosition = hitarea_pos;
        }
    }
}