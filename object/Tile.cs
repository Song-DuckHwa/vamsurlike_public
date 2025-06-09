using System;
using UnityEngine;
using UnityEngine.Events;

namespace game
{
    /**
    * OnActionCollideArg
    * 타일과 충돌했을 경우 액션
    * @tile - 충돌한 타일
    * @quadrant - 충돌한 타일의 사분면
    **/
    public class OnActionCollideArg : EventArgs
    {
        public Tile tile;
        public int quadrant;
    }

    /**
    * OnActionTileCollideStart
    * 타일에 충돌을 시작했을때 액션
    **/
    public class OnActionTileCollideStart : OnActionCollideArg
    {
    }

    /**
    * OnActionTileCollideIng
    * 타일에 충돌 시작한 다음의 액션
    **/
    public class OnActionTileCollideIng : OnActionCollideArg
    {
    }

    /**
    * OnActionTileCollideEnd
    * 타일에서 빠져나갔을때 액션
    **/
    public class OnActionTileCollideEnd : OnActionCollideArg
    {
    }

    /**
    * Tile
    * 1024 x 1024 타일 객체
    **/
    public class Tile : Entity
    {
        private CollideState collide_state = CollideState.NONE;
        private int mainpc_quadrant = 0;

        public UnityAction< OnActionTileCollideStart > OnCollideStart;
        public UnityAction< OnActionTileCollideIng > OnCollideIng;
        public UnityAction< OnActionTileCollideEnd > OnCollideEnd;

        /**
        * 캐릭터가 현재 타일의 어느 사분면에 있는지 계산
        **/
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

        /**
        * 캐릭터가 현재 타일의 어느 사분면에 있는지 찾아 나머지 타일들을 이동시켜 무한맵 처럼 보이게 함
        **/
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

            bool result = CollideFuncs.pointToRectCollide( mainch_pos, rect );
            if( result )
            {
                int chpos_quadrant = calcQuadrant();

                switch( collide_state )
                {
                    case CollideState.NONE :
                        collide_state = CollideState.START;
                        OnCollideStart?.Invoke( new OnActionTileCollideStart { tile = this, quadrant = chpos_quadrant } );
                        break;
                    case CollideState.START :
                    case CollideState.ING :
                        collide_state = CollideState.ING;

                        if( mainpc_quadrant != chpos_quadrant )
                            OnCollideIng?.Invoke( new OnActionTileCollideIng { tile = this, quadrant = chpos_quadrant } );
                        break;
                    default :
                        collide_state = CollideState.NONE;
                        break;
                }

                mainpc_quadrant = chpos_quadrant;
            }
            else
            {
                switch( collide_state )
                {
                    case CollideState.START :
                    case CollideState.ING :
                        collide_state = CollideState.END;
                        OnCollideEnd?.Invoke( new OnActionTileCollideEnd { } );
                        break;
                    case CollideState.END :
                        collide_state = CollideState.NONE;
                        break;
                    default :
                        collide_state = CollideState.NONE;
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