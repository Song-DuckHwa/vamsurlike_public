using UnityEngine;
using UnityEngine.InputSystem;

namespace game
{
    /**
    * PCMain
    * 플레이어가 조종하는 pc 클래스
    **/
    public class PCMain : PC
    {
        public int current_exp = 0;
        public int max_exp = 0;
        public int collect_dist = 0;

        /**
        * 초기화
        **/
        public override void init()
        {
            level = 1;
            hp = 50;
            move_speed = 200;
            direction = Vector3.zero;

            collide_mask |= (int)CollideMask.NPC;
            collide_mask |= (int)CollideMask.MOB;
            collide_mask |= (int)CollideMask.BULLET;
            collide_mask |= (int)CollideMask.OBSTACLE;

            collide_type = (int)CollideMask.PC;

            attack_dir_effect.transform.localPosition = new Vector3( 60f, 0, 0 );

            skillmgr.clearSkill();

            invincible_duration_msec = 400;
            invincible_time_msec = 0;

            current_exp = 0;
        }

        /**
        * 스탯 세팅
        **/
        public void setStat( Level stat )
        {
            hp = (int)stat.hp;
            atk = (int)stat.atk;
            move_speed = (int)stat.speed;
            max_exp = (int)stat.exp;
        }

        /**
        * 레벨업 시 hp 동기화
        **/
        public void equalizingHP()
        {
            current_hp = hp;
        }

        /*
         * dpad에서 받은 데이터를 이용하여 캐릭터의 이동 방향을 전달
         * @value - 버추얼 패드 이동값 데이터 - vector로 옴
         */
        private void OnMove( InputValue value )
        {
            Vector2 input = value.Get<Vector2>();
            if( input != null )
            {
                if( input == Vector2.zero )
                {
                    velocity = 0;
                    state = (int)STATE.IDLE;
                    return;
                }

                direction = new Vector3( input.x, input.y, 0 );
                velocity = 1;
                state = (int)STATE.RUN;
            }
        }
    }
}