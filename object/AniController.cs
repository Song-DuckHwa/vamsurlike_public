using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class AniController : MonoBehaviour
    {
        public void OnRelease()
        {
            transform.parent.gameObject.GetComponent< Npc >().release();
        }
    }
}
