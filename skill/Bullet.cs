using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class Bullet : Skill
    {
        Vector3 direction;
        int move_speed;
        private int knockback_dist = 0;
        Bullet parent;

        void Awake()
        {
            move_speed = 1000;
            repeat_time_msec = 0;
        }

        private void FixedUpdate()
        {
            Vector3 dest_vec = direction;
            dest_vec.Normalize();
            dest_vec *= move_speed;
            //프레임 보간
            dest_vec *= Time.deltaTime;

            transform.Translate( dest_vec );
        }

        private void Update()
        {
            if( GameManager.mainch == null )
                return;

            Rect camera_rect = new Rect();
            camera_rect.xMin = GameManager.mainch.transform.position.x - GameManager.instance.res_v_half;
            camera_rect.xMax = GameManager.mainch.transform.position.x + GameManager.instance.res_v_half;
            camera_rect.yMin = GameManager.mainch.transform.position.y - GameManager.instance.res_h_half;
            camera_rect.yMax = GameManager.mainch.transform.position.y + GameManager.instance.res_h_half;

            if( transform.position.x < camera_rect.xMin || camera_rect.xMax < transform.position.x ||
                transform.position.y < camera_rect.yMin || camera_rect.yMax < transform.position.y
                )
            {
                parent.ins_list.Remove( this );
                die();
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
                        Npc actor = GameManager.charmgr.find( actor_uid );
                        if( actor == null )
                            return;

                        ch.takeDamage( actor.gameObject, knockback_dist );

                        parent.ins_list.Remove( this );
                        die();
                        return;
                    }
                }
            }
        }

        public override bool active()
        {
            SkillDetailData table = getTableData();
            if( table == null )
                return true;

            direction = GameManager.mainch.direction;
            if( direction == Vector3.zero )
                direction = new Vector3( 1f, 0f, 0f );

            GameObject ins = GameManager.poolmgr.instanceGet( table.asset_address );
            if( ins == null )
                return true;

            Bullet script = ins.GetComponent< Bullet >();
            script.collider_compo = ins.GetComponent< BoxCollider2D >();
            script.parent = this;

            ins.transform.position = GameManager.mainch.transform.position;
            script.direction = direction;
            ins.SetActive( true );

            GameManager.soundmgr.sfxs[ SFX.BULLET ].Play();

            //magic
            repeat_time_msec = GameManager.getCurrentGameTime() + 500;

            ins_list.Add( script );

            return false;
        }
    }
}