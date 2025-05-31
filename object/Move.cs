using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace game
{
    public class Move
    {
        public void Action( Npc ch )
        {
            Vector3 dest_vec = ch.direction;
            dest_vec.Normalize();
            float dist = Vector3.Distance( dest_vec, Vector3.zero );
            dest_vec *= ch.move_speed;
            //프레임 보간
            dest_vec *= Time.deltaTime;

            ch.transform.Translate( dest_vec );
        }
    }
}