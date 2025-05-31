using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class Entity : MonoBehaviour
    {
        float scale = 1f;

        public float SCALE { get{ return scale; } }

        public int collide_mask = (int)CollideMask.NONE;
        public int collide_type = (int)CollideMask.NONE;

        public Component compo = null;

        public ObjectPool pool;

        public virtual void release()
        {
            pool.release( this.gameObject );
        }
    }
}
