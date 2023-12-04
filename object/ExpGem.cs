using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

namespace game
{
    public class ExpGem : MoveableObject
    {
        public int uid;
        NPC target;
        public int exp;

        private bool gen_complete = false;

        void OnEnable()
        {
            move_speed = 0;

            target = GameManager.charmgr.find( 0 );
            if( target == null )
                return;

            gen_complete = false;
            StartCoroutine( "GenerateAni" );
        }

        IEnumerator GenerateAni()
        {
            //0~359
            int random_spawn_degree = Random.Range( 0, 360 );
            float random_spawn_radian = (float)random_spawn_degree * mathlib.DEG_TO_RAD;
            Vector3 start_pos = transform.position;
            float start_time = GameManager.getCurrentGameTime();

            yield return null;

            for( ; ; )
            {
                float current_msec = GameManager.getCurrentGameTime();
                float inverse_lerp = Mathf.InverseLerp( start_time, start_time + 500f, current_msec );
                //ease quint out - https://easings.net/ko#easeOutQuint
                float ease = 1 - Mathf.Pow( 1 - inverse_lerp, 5 );
                float dist = 100f;
                float new_dist = dist * ease;

                float pos_x = (new_dist * Mathf.Cos( random_spawn_radian )) + start_pos.x;
                float pos_y = (new_dist * Mathf.Sin( random_spawn_radian )) + start_pos.y;

                transform.position = new Vector3( pos_x, pos_y, 0f );

                if( inverse_lerp >= 1 )
                {
                    gen_complete = true;
                    StopCoroutine( "GenerateAni" );
                }

                yield return null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //move stop
            if( target == null || target.gameObject == null )
                return;

            if( gen_complete == true )
                followCharacter();
        }

        public void followCharacter()
        {
            float dist = Vector2.Distance( target.transform.position, transform.position );
            //magic -> pc.exp_collect_dist
            if( dist <= 200 )
            {
                Vector3 dest = target.transform.position;
                Vector3 current_pos = transform.position;
                Vector3 velocity = new Vector3( dest.x - current_pos.x, dest.y - current_pos.y );
                velocity.Normalize();
                velocity *= move_speed;
                //프레임 보간
                velocity *= Time.deltaTime;
                transform.Translate( velocity );
                //magic
                move_speed += 20;

                if( dist <= 50 )
                    die();
            }
            else
            {
                //거리가 멀어질때 다시 초기화
                move_speed = 0;
            }
        }

        public override void die()
        {
            GameManager.gamelogic.addMainPCExp( exp );
            GameManager.charmgr.deleteExpGem( uid );
            release();
        }
    }
}