using game;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace game
{
    public class Swing : Skill
    {
        Vector3 direction;
        private int knockback_dist = 20;
        int hit_end_time;
        public GameObject sprite;

        // Update is called once per frame
        void Update()
        {
            //타격 시간이 지나면 타격하지 않음
            if( gameObject.activeSelf == true && GameManager.getCurrentGameTime() >= hit_end_time )
            {
                return;
            }

            List< int > hitted_targets = GameManager.charmgr.attackCollideCheck( (BoxCollider2D)collider_compo );
            if( hitted_targets.Count > 0 )
            {
                int i = 0;
                int loop_max = hitted_targets.Count;
                for( ; i < loop_max ; ++i )
                {
                    //collide mask로 변경해야 함
                    if( hitted_targets[ i ] == actor_uid )
                        continue;

                    Npc ch = GameManager.charmgr.find( hitted_targets[ i ] );
                    if( ch != null )
                    {
                        hitted_targets_set.TryGetValue( hitted_targets[ i ], out int target_uid );
                        if( target_uid == 0 )
                        {
                            Npc actor = GameManager.charmgr.find( actor_uid );
                            if( actor == null )
                                return;

                            ch.takeDamage( actor.gameObject, knockback_dist );

                            hitted_targets_set.Add( hitted_targets[ i ] );
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
                return true;

            SkillLevelData table_level_data = table.level_data[ level - 1 ];

            int i = 0;
            int loop_max = table_level_data.objcount;
            for( ; i < loop_max ; ++i )
            {
                direction = GameManager.mainch.direction;
                if( direction == Vector3.zero )
                    direction = new Vector3( 1f, 0f, 0f );

                direction.Normalize();
                GameObject ins = GameManager.poolmgr.instanceGet( table.asset_address );
                if( ins == null )
                    return true;

                Swing script = ins.GetComponent< Swing >();
                script.collider_compo = ins.GetComponent< BoxCollider2D >();
                script.hitted_targets_set.Clear();

                ins.transform.SetParent( GameManager.mainch.transform );

                SpriteRenderer skill_sps_renderer = script.sprite.GetComponent< SpriteRenderer >();
                float scale = table_level_data.radius / ( (skill_sps_renderer.sprite.bounds.size.y * 0.5f) + Mathf.Abs( script.sprite.transform.localPosition.x ) );
                ins.transform.localScale = new Vector3( scale, 1f, 1f );

                Vector2 dir = direction;
                int total_deg = 20 * (loop_max - 1);
                float offset_deg = (i * 20) - (total_deg * 0.5f);
                float rad = Mathf.Atan2( dir.y, dir.x ) + (offset_deg * Mathf.Deg2Rad);
                float deg = rad * Mathf.Rad2Deg;
                Vector2 hitarea_pos = new Vector2();
                hitarea_pos.x = Mathf.Cos( rad );
                hitarea_pos.y = Mathf.Sin( rad );

                ins.transform.eulerAngles = new Vector3( 0, 0, deg );
                ins.transform.localPosition = hitarea_pos;

                script.hit_end_time = GameManager.getCurrentGameTime() + 300;

                ins.SetActive( true );
                Animator ani = script.sprite.GetComponent< Animator >();
                ani.SetTrigger( "play" );
                GameManager.soundmgr.sfxs[ SFX.SWING ].Play();

                ins_list.Add( script );
            }

            repeat_time_msec += table_level_data.attack_tick;

            return false;
        }
    }
}