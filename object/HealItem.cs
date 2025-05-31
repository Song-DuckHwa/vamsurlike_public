using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace game
{
    public class HealItem : MoveableObject
    {


        // Update is called once per frame
        void Update()
        {
            float dist = Vector2.Distance( GameManager.mainch.transform.position, transform.position );
            //magic -> pc.exp_collect_dist
            if( dist <= 50 )
            {
                die();
            }
        }

        public override void die()
        {
            GameManager.gamelogic.calcCurrentHPMainPC( 10 );
            Destroy( gameObject );
        }
    }
}