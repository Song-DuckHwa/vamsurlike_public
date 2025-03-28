using Cysharp.Threading.Tasks;
using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

namespace game
{
    /**
    * ExpGem
    * 경험치 보석 오브젝트
    **/
    public class ExpGem : MoveableObject
    {
        public int uid;
        Npc target;
        public int exp;

        private bool gen_complete = false;

        void OnEnable()
        {
            move_speed = 0;

            target = GameManager.charmgr.find( 0 );
            if( target == null )
                return;

            gen_complete = false;
            GenerateAni().Forget();
        }

        /**
        * 생성 되는 순간 튀어나가는 효과를 보이는 이동 코루틴
        **/
        async UniTaskVoid GenerateAni()
        {
            //0~359
            int random_spawn_degree = Random.Range( 0, 360 );
            float random_spawn_radian = (float)random_spawn_degree * mathlib.DEG_TO_RAD;
            Vector3 start_pos = transform.position;
            float start_time = GameManager.getCurrentGameTime();
            float dist = 100f;
            float current_msec = 0f;
            float inverse_lerp = 0f;
            float ease = 0f;
            float new_dist = 0f;
            Vector3 new_pos = new Vector3( 0f, 0f, 0f );

            while( true )
            {
                current_msec = GameManager.getCurrentGameTime();
                inverse_lerp = Mathf.InverseLerp( start_time, start_time + 500f, current_msec );
                ease = Ease.Quint.Out( inverse_lerp );
                new_dist = dist * ease;

                new_pos.x = (new_dist * Mathf.Cos( random_spawn_radian )) + start_pos.x;
                new_pos.y = (new_dist * Mathf.Sin( random_spawn_radian )) + start_pos.y;

                transform.position = new Vector3( new_pos.x, new_pos.y, 0f );

                if( inverse_lerp >= 1f )
                {
                    gen_complete = true;
                    return;
                }

                await UniTask.NextFrame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //move stop
            if( target == null || target.gameObject == null )
                return;

            //생성이 완료 되었을 경우 주변에 캐릭터가 있는지 확인하여 캐릭터가 있다면 캐릭터를 추적함
            if( gen_complete == true )
                followCharacter();
        }

        /**
        * 생성이 완료 되었을 경우 주변에 캐릭터가 있는지 확인하여 캐릭터가 있다면 캐릭터를 추적함
        **/
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
            GameManager.soundmgr.sfxs[ SFX.GEM ].Play();
            release();
        }
    }
}