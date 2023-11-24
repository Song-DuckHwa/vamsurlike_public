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
        CharacterData target;
        public int exp;

        public IObjectPool< GameObject > pool;

        void OnEnable()
        {
            move_speed = 0;

            target = GameManager.charmgr.find( 0 );
            if( target == null )
                return;
        }

        // Update is called once per frame
        void Update()
        {
            //move stop
            if( target.gameobj == null )
                return;

            followCharacter();
        }

        public void followCharacter()
        {
            float dist = Vector2.Distance( target.gameobj.transform.position, transform.position );
            //magic -> pc.exp_collect_dist
            if( dist <= 200 )
            {
                Vector3 dest = target.gameobj.transform.position;
                Vector3 current_pos = transform.position;
                Vector3 velocity = new Vector3( dest.x - current_pos.x, dest.y - current_pos.y );
                velocity.Normalize();
                velocity *= move_speed;
                //프레임 보간
                velocity *= Time.deltaTime;
                transform.Translate( velocity );
                //magic
                move_speed += 2;

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
            pool.Release( this.gameObject );
        }
    }
}