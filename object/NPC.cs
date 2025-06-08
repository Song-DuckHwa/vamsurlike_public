using UnityEngine;

namespace game
{
    /**
    * Npc
    * NPC 객체 클래스 - 적, npc 전부
    **/
    public class Npc : MoveableObject
    {
        public int hp;
        public int current_hp = 0;
        public int atk;
        public int attack_speed;
        public int level;
        public int exp;

        private int life_time;

        Vector2 attack_dir;
        public Vector2 move_dest;

        Npc target;

        public GameObject attack_hitbox;

        public int uid;

        public SkillManager skillmgr = new SkillManager();

        private void Awake()
        {
            initCompos();
        }

        private void OnEnable()
        {
            current_hp = hp;

            target = GameManager.mainch;
            if( target == null )
                return;

            spr.color = Color.white;

            state = (int)STATE.IDLE;
            if( target != null || move_dest != Vector2.zero )
                state = (int)STATE.RUN;
        }

        /**
        * 시간제한이 있는 객체일 경우 세팅 - 콜로세움 패턴 같은 경우
        **/
        void Start()
        {
            life_time = life_time != 0 ? life_time : int.MaxValue;
        }

        protected virtual void initCompos()
        {
            GameObject sprite = transform.GetChild( 0 ).gameObject;
            spr = sprite.GetComponent< SpriteRenderer >();
            ani = sprite.GetComponent< Animator >();
            rigidbody = GetComponent< Rigidbody2D >();

            ccol = GetComponent<CircleCollider2D>();

            collide_mask |= (int)CollideMask.PC;
            collide_mask |= (int)CollideMask.NPC;
            collide_mask |= (int)CollideMask.MOB;
            collide_mask |= (int)CollideMask.BULLET;
            collide_mask |= (int)CollideMask.OBSTACLE;

            collide_type = (int)CollideMask.MOB;
        }

        public virtual void setStat( Monster stat )
        {
            hp = (int)stat.hp;
            atk = (int)stat.atk;
            move_speed = (int)stat.move_speed;
            exp = (int)stat.exp;
        }

        public void setLifeTime( int lifetime )
        {
            life_time = (int)lifetime;
        }

        /**
        * 시간제한이 있는 객체일 경우 라이프타임 관리
        **/
        private void Update()
        {
            NpcUpdate();
        }

        protected void NpcUpdate()
        {
            if( state != (int)STATE.DEAD && life_time <= 0 )
            {
                die( false );
                return;
            }

            int current_game_time_msec = GameManager.getCurrentGameTime();
            skillmgr.Update( current_game_time_msec );

            if( life_time != int.MaxValue )
            {
                life_time -= (int)(Time.deltaTime * 1000f);
            }

            switch( state )
            {
                case (int)STATE.RUN :
                    moveWithoutRigidBody();
                    break;
            }
        }

        /**
        * 리지드바디가 있는 경우 이동 로직
        **/
        public virtual void moveWithRigidBody()
        {
            //move stop
            if( target == null || target.gameObject == null )
            {
                //if( rigidbody != null )
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                return;
            }

            Vector3 dest = target.transform.position;
            Vector3 current_pos = transform.position;
            Vector3 velocity = new Vector3( dest.x - current_pos.x, dest.y - current_pos.y );
            velocity.Normalize();
            velocity *= move_speed;

            GetComponent<Rigidbody2D>().velocity = velocity;

            //direction
            if( velocity.x > 0 )
                transform.localScale = new Vector3( -1, 1, 1 );
            else
                transform.localScale = new Vector3( 1, 1, 1 );
        }

        /**
        * 리지드바디가 없는 경우 이동 로직
        **/
        public virtual void moveWithoutRigidBody()
        {
            //move stop
            if( target == null || target.gameObject == null )
            {
                move_speed = 0;
                state = (int)STATE.IDLE;
            }

            Vector3 dest = Vector3.zero;
            Vector3 current_pos = Vector3.zero;
            Vector3 velocity = Vector3.zero;

            dest = (move_dest != Vector2.zero) ? move_dest : target.transform.position;
            current_pos = transform.position;
            velocity = new Vector3( dest.x - current_pos.x, dest.y - current_pos.y );
            velocity.Normalize();
            velocity *= move_speed;
            //프레임 보간
            velocity *= Time.deltaTime;
            transform.Translate( velocity );
            direction = velocity;

            spr.flipX = (velocity.x < 0);
            state = (int)STATE.RUN;
        }

        /**
        * 리지드바디가 있는 경우 피격 로직
        **/
        public virtual void OnTriggerEnter2D( Collider2D collision )
        {
            if( collision.gameObject.CompareTag( "pc_attack" ) )
            {
                //takeDamage();
            }
        }

        /**
        * 리지드바디가 없는 경우 피격 로직
        * @hitter - 때린 사람
        * @knockback_dist - 넉백 거리
        **/
        public virtual void takeDamage( GameObject hitter, int knockback_dist )
        {
            GameManager.soundmgr.sfxs[ SFX.DAMAGE ].Play();
            current_hp -= 10;
            if( current_hp <= 0 )
            {
                die();
                return;
            }

            CollideFuncs.knockbackPosition( hitter, gameObject, knockback_dist );
            ani.SetTrigger( "Damage" );
        }

        public virtual void enemyCollide()
        {

        }

        public virtual void die( bool gem_drop = true )
        {
            if( gem_drop == true )
            {
                GameManager.gamelogic.generateExpGem( uid, exp );
                GameManager.gamelogic.generateHealItem( uid );
            }

            //잡으면 스테이지 클리어
            if( GameManager.gamelogic.stage_clear_key_monsters.TryGetValue( uid, out int stage_clear_key ) == true )
            {
                if( stage_clear_key == uid )
                {
                    GameManager.gamelogic.stage_clear_key_monsters.Remove( uid );
                    GameManager.soundmgr.sfxs[ SFX.BOSSKILL ].Play();
                }
            }


            //이 애니 끝에 이벤트로 release가 들어가있음
            ani.SetTrigger( "Dead" );

            //target = null;
            state = (int)STATE.DEAD;
        }

        public override void release()
        {
            GameManager.charmgr.delete( uid );
            skillmgr.clearSkill();
            life_time = int.MaxValue;
            move_dest = Vector2.zero;
            pool.release( this.gameObject );
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere( transform.position, 25 );
        }
    }
}
