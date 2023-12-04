using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace game
{

    public class OnActionCollideArg : EventArgs
    {
        public Tile tile;
        public int quadrant;
    }
    public class OnActionTileCollideStart : OnActionCollideArg
    {
    }

    public class OnActionTileCollideIng : OnActionCollideArg
    {
    }

    public class OnActionTileCollideEnd : OnActionCollideArg
    {
    }

    public class Tile : Entity
    {
        private int collide_state = (int)CollideState.NONE;
        private int mainpc_quadrant = 0;

        public UnityAction< OnActionTileCollideStart > OnCollideStart;
        public UnityAction< OnActionTileCollideIng > OnCollideIng;
        public UnityAction< OnActionTileCollideEnd > OnCollideEnd;

        public int calcQuadrant()
        {
            Vector3 mainch_pos = GameManager.mainch.transform.position;

            /*
             * 2 1
             * 3 4
             */

            int quadrant = 0;

            if( mainch_pos.x >= transform.position.x )
            {
                quadrant = 4;
                if( mainch_pos.y >= transform.position.y )
                    quadrant = 1;
            }
            else
            {
                quadrant = 3;
                if( mainch_pos.y >= transform.position.y )
                    quadrant = 2;
            }


            return quadrant;
        }

        public void Update()
        {
            if( GameManager.mainch == null )
                return;

            Vector3 mainch_pos = GameManager.mainch.transform.position;
            float dist = Vector2.Distance( transform.position, mainch_pos );
            //magic
            //사각형 대각선 길이로 체크
            float diagonal = 1024f * Mathf.Sqrt( 2f );
            if( dist > diagonal )
                return;

            BoxCollider2D box = GetComponent< BoxCollider2D >();

            Rect rect = new Rect();
            rect.xMin = transform.position.x - (box.size.x * 0.5f);
            rect.xMax = transform.position.x + (box.size.x * 0.5f);
            rect.yMin = transform.position.y - (box.size.y * 0.5f);
            rect.yMax = transform.position.y + (box.size.y * 0.5f);

            bool result = GameManager.collidefuncs.pointToRectCollide( mainch_pos, rect );
            if( result )
            {
                int chpos_quadrant = calcQuadrant();

                switch( collide_state )
                {
                    case (int)CollideState.NONE :
                        collide_state = (int)CollideState.START;
                        OnCollideStart?.Invoke( new OnActionTileCollideStart { tile = this, quadrant = chpos_quadrant } );
                        break;
                    case (int)CollideState.START :
                    case (int)CollideState.ING :
                        collide_state = (int)CollideState.ING;

                        if( mainpc_quadrant != chpos_quadrant )
                            OnCollideIng?.Invoke( new OnActionTileCollideIng { tile = this, quadrant = chpos_quadrant } );
                        break;
                    default :
                        collide_state = (int)CollideState.NONE;
                        break;
                }

                mainpc_quadrant = chpos_quadrant;
            }
            else
            {
                switch( collide_state )
                {
                    case (int)CollideState.START :
                    case (int)CollideState.ING :
                        collide_state = (int)CollideState.END;
                        OnCollideEnd?.Invoke( new OnActionTileCollideEnd { } );
                        break;
                    case (int)CollideState.END :
                        collide_state = (int)CollideState.NONE;
                        break;
                    default :
                        collide_state = (int)CollideState.NONE;
                        break;
                }
            }
        }
    }

    enum CollideState
    {
        NONE = 0,
        START,
        ING,
        END,
    }
}