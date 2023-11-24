using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class NPC : MoveableObject
    {
        public int hp;
        public int current_hp = 0;
        public int atk;
        public int attack_speed;
        public int level;
        public int exp;

        Vector2 attack_dir;

        CharacterData target;
        PC pc_compo;

        public GameObject attack_hitbox;

        public int uid;

        public SkillManager skillmgr = new SkillManager();

        // Start is called before the first frame update
        void Start()
        {
            //target = GameManager.charmgr.find( 0 );
            target = GameManager.mainch;
            if( target == null )
                return;

            pc_compo = (PC)target.script;
            col = GetComponent<CircleCollider2D>();

            collide_mask |= (int)CollideMask.PC;
            collide_mask |= (int)CollideMask.NPC;
            collide_mask |= (int)CollideMask.MOB;
            collide_mask |= (int)CollideMask.BULLET;
            collide_mask |= (int)CollideMask.OBSTACLE;

            collide_type = (int)CollideMask.MOB;

            compo = GetComponent<NPC>();

            spr = GetComponent< SpriteRenderer >();

            current_hp = hp;
        }

        public virtual void setStat( Monster stat )
        {
            hp = (int)stat.hp;
            atk = (int)stat.atk;
            move_speed = (int)stat.move_speed;
            exp = (int)stat.exp;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            int current_game_time_msec = GameManager.getCurrentGameTime();
            skillmgr.Update( current_game_time_msec );
        }

        //use rigidbody
        private void FixedUpdate()
        {
            //moveWithRigidBody();
            moveWithoutRigidBody();
        }

        public virtual void moveWithRigidBody()
        {
            //move stop
            if( target.gameobj == null )
            {
                //if( rigidbody != null )
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                return;
            }

            Vector3 dest = target.gameobj.transform.position;
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

        public virtual void moveWithoutRigidBody()
        {
            //move stop
            if( target.gameobj == null )
            {
                move_speed = 0;
                return;
            }

            Vector3 dest = target.gameobj.transform.position;
            Vector3 current_pos = transform.position;
            Vector3 velocity = new Vector3( dest.x - current_pos.x, dest.y - current_pos.y );
            velocity.Normalize();
            velocity *= move_speed;
            //프레임 보간
            velocity *= Time.deltaTime;
            transform.Translate( velocity );
            direction = velocity;

            spr.flipX = (velocity.x < 0);
        }

        public virtual void OnTriggerEnter2D( Collider2D collision )
        {
            if( collision.gameObject.CompareTag( "pc_attack" ) )
            {
                takeDamage();
            }
        }

        public virtual void takeDamage()
        {
            current_hp -= 10;
            if( current_hp <= 0 )
            {
                die();
            }
        }

        public virtual void enemyCollide()
        {

        }

        public override void die()
        {
            GameManager.gamelogic.generateExpGem( uid, exp );
            GameManager.gamelogic.generateHealItem( uid );
            Destroy( this.gameObject );
            int stage_clear_key = 0;
            GameManager.gamelogic.stage_clear_key_monsters.TryGetValue( uid, out stage_clear_key );
            if( stage_clear_key == uid )
                GameManager.gamelogic.stage_clear_key_monsters.Remove( uid );

            GameManager.charmgr.delete( uid );
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere( transform.position, 25 );
        }
    }
}
