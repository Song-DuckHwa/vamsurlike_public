using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class CameraScript : MonoBehaviour
    {
        private Vector3 offset;

        public GameObject following_obj;

        // Start is called before the first frame update
        void Start()
        {
            offset = new Vector3( 0f, 0f, -10f );
        }

        private void Update()
        {
            if( following_obj != null )
            {
                transform.position = following_obj.transform.position + offset;
            }
        }

        public void setFollowObject( GameObject obj )
        {
            following_obj = obj;
        }

        public void stopFollow()
        {
            following_obj = null;
        }
    }
}