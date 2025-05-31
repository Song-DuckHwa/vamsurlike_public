using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace game
{
    public class MoveableObject : Entity
    {
        public int move_speed;

        new public Rigidbody2D rigidbody = null;
        public Vector3 direction;

        public CircleCollider2D ccol = null;
        public SpriteRenderer spr = null;
        protected Animator ani = null;

        public int state = (int)STATE.NONE;

        public virtual void die()
        {
        }
    }

    enum STATE
    {
        NONE = 0,
        IDLE,
        RUN,
        DEAD,
        MOVE,
        MAX,
    }
}