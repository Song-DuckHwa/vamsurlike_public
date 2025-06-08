using System.Collections;
using UnityEngine;

namespace game
{
    /**
    * PC
    * 플레이어가 조종하지 않는 pc 클래스
    **/
    public class PC : Npc
    {
        public GameObject attack_effect;
        public GameObject attack_dir_effect;

        public int velocity = 0;

        public int exp_collect_dist = 100;

        public int invincible_duration_msec;
        public int invincible_time_msec;

        void Awake()
        {
            initCompos();
        }

        private void OnEnable()
        {
            init();
        }

        /**
        * 필요한 컴포넌트 등록
        **/
        protected override void initCompos()
        {
            GameObject sprite = transform.GetChild( 0 ).gameObject;
            spr = sprite.GetComponent< SpriteRenderer >();
            ani = sprite.GetComponent< Animator >();
            rigidbody = GetComponent< Rigidbody2D >();
            ccol = GetComponent< CircleCollider2D >();
        }

        public virtual void init()
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

            invincible_duration_msec = 400;
            invincible_time_msec = 0;
        }

        private void Update()
        {
            NpcUpdate();
            rotateAttackDir();
        }

        void LateUpdate()
        {
            ani.SetInteger( "velocity", velocity );
        }

        /**
        * 리지드바디를 사용할 경우 이동 로직
        **/
        public override void moveWithRigidBody()
        {
            //Move.Action();
            //direction
            if( direction.x > 0 )
                transform.localScale = new Vector3( -1, 1, 1 );
            else if( direction.x < 0 )
                transform.localScale = new Vector3( 1, 1, 1 );

            Vector3 dest_vec = direction;
            dest_vec.Normalize();
            dest_vec *= move_speed;

            GetComponent<Rigidbody2D>().velocity = dest_vec;
        }

        /**
        * 리지드바디를 사용하지 않는 경우 이동 로직
        **/
        public override void moveWithoutRigidBody()
        {
            spr.flipX = (direction.x < 0);

            Vector3 dest_vec = direction;
            dest_vec.Normalize();
            float dist = Vector3.Distance( dest_vec, Vector3.zero );
            dest_vec *= move_speed * velocity;
            //프레임 보간
            dest_vec *= Time.deltaTime;

            transform.Translate( dest_vec );

            if( velocity == 0 )
                state = (int)STATE.IDLE;
            else
                state = (int)STATE.RUN;
        }

        /**
        * 바라보는 방향으로 공격 방향 회전
        **/
        public void rotateAttackDir()
        {
            Vector2 dir = direction;
            float rad = Mathf.Atan2( dir.y, dir.x );
            float deg = rad * Mathf.Rad2Deg;
            Vector2 hitarea_pos = new Vector2();
            int hitarea_dist = 60;
            hitarea_pos.x = hitarea_dist * Mathf.Cos( rad );
            hitarea_pos.y = hitarea_dist * Mathf.Sin( rad );

            attack_dir_effect.transform.localPosition = hitarea_pos;
            attack_dir_effect.transform.eulerAngles = new Vector3( 0, 0, deg );
        }

        /**
        * 리지드바디를 사용할 경우 적과의 충돌
        **/
        public override void OnTriggerEnter2D( Collider2D collision )
        {
            if( collision.gameObject.CompareTag( "enemy" ) )
            {
                //takeDamage();
            }
        }

        /**
        * 리지드바디를 사용하지 않는 경우 적과의 충돌
        **/
        public override void enemyCollide()
        {
            int currect_game_time = GameManager.getCurrentGameTime();
            if( currect_game_time >= invincible_time_msec )
            {
                takeDamage();
            }
        }

        /**
        * 이동 방향
        * @h - horizontal
        * @v - vertical
        **/
        public void move( int h, int v )
        {
            direction = new Vector3( h, v, 0 );
            //direction = new Vector3( 1, 1, 0 );
        }

        /**
        * 데미지를 입었을 경우 로직 처리
        * @hitter - 때린 사람
        * @knockback_dist - 넉백 거리
        **/
        public override void takeDamage( GameObject hitter, int knockback_dist )
        {
            GameManager.soundmgr.sfxs[ SFX.PCDAMAGE ].Play();
            GameManager.gamelogic.calcCurrentHPMainPC( -1 );
            if( current_hp <= 0 )
            {
                die();
                return;
            }

            invincible_time_msec = GameManager.getCurrentGameTime() + invincible_duration_msec;
            ani.SetTrigger( "Damage" );
        }

        /**
        * 테스트 용 데미지를 입었을 경우 로직 처리
        **/
        public virtual void takeDamage()
        {
            GameManager.soundmgr.sfxs[ SFX.PCDAMAGE ].Play();
            GameManager.gamelogic.calcCurrentHPMainPC( -1 );
            if( current_hp <= 0 )
            {
                die();
                return;
            }

            invincible_time_msec = GameManager.getCurrentGameTime() + invincible_duration_msec;
            ani.SetTrigger( "Damage" );
        }

        /**
        * 테스트 용 데미지를 입었을 경우 로직 처리
        **/
        public virtual void takeDamage_test()
        {
            GameManager.soundmgr.sfxs[ SFX.PCDAMAGE ].Play();
            GameManager.gamelogic.calcCurrentHPMainPC( -10 );
            if( current_hp <= 0 )
            {
                die();
                return;
            }

            invincible_time_msec = GameManager.getCurrentGameTime() + invincible_duration_msec;
            ani.SetTrigger( "Damage" );
        }

        /**
        * 캐릭터 죽음 처리
        **/
        public override void die()
        {
            ani.SetTrigger( "Dead" );
            state = (int)STATE.DEAD;
        }

        /**
        * 캐릭터 공격 - 현재 사용하지 않음( 일반 공격까지 스킬로 만들어서 스킬매니저로 빠짐 )
        **/
        public void attack()
        {
            Vector2 dir = direction;
            float rad = Mathf.Atan2( dir.y, dir.x );
            float deg = rad * Mathf.Rad2Deg;
            Vector2 hitarea_pos = new Vector2();
            int hitarea_dist = 100;
            hitarea_pos.x = hitarea_dist * Mathf.Cos( rad );
            hitarea_pos.y = hitarea_dist * Mathf.Sin( rad );

            attack_effect.transform.localPosition = hitarea_pos;
            attack_effect.transform.eulerAngles = new Vector3( 0, 0, deg );
            attack_effect.SetActive( true );
            /*
            StopCoroutine( "attack_end" );
            StartCoroutine( "attack_end" );
            */
        }

        /**
        * 공격 종료 시 공격 판정 끄는 코루틴 - 현재 사용하지 않음
        **/
        IEnumerator attack_end()
        {
            Vector3 effect_pos = attack_effect.transform.position;
            attack_hitbox.transform.position = effect_pos;
            attack_hitbox.SetActive( true );
            yield return new WaitForSeconds( 0.1f );
            attack_effect.SetActive( false );
            attack_hitbox.SetActive( false );
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere( transform.position, 25 );
        }
    }
}
