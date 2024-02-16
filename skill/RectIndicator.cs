using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
	/**
	* RectIndicator
	* 보스의 스킬 등 사정거리나 타이밍을 보여주는 인디케이터
	**/
    public class RectIndicator : MonoBehaviour
    {
        public GameObject timer;
        public GameObject edge;
        public SpriteRenderer spr_renderer;

        public GameObject target;

        public Vector2 size = Vector2.one;

        public int duration = 0;
        public float start_time = 0;
        public float end_time = 0;

        public int follow_uid;
        public SpriteRenderer follower_spr_renderer;

        private void Awake()
        {
            spr_renderer = edge.GetComponent< SpriteRenderer >();
        }

		/**
		* 인디케이터 활성화 시 초기화
		**/
        private void OnEnable()
        {
            start_time = GameManager.getCurrentGameTime();
            end_time = GameManager.getCurrentGameTime() + duration;
            spr_renderer.size = new Vector2( size.x, size.y );
            //이미지 사이즈가 100 x 100
            timer.transform.localScale = new Vector3( (size.x * 0.01f), 0f, 1f );
        }

        // Update is called once per frame
        void Update()
        {
            if( target.activeSelf == false )
            {
                die();
                return;
            }

            if( GameManager.getCurrentGameTime() >= end_time )
            {
                die();
                return;
            }

            followFlip();

			//시전 시간에 따라 타이머의 스케일을 조절하여 언제 쯤 공격하는지 알려준다
            float lerp = Mathf.Lerp( 0, (size.y * 0.01f), (GameManager.getCurrentGameTime() - start_time) / (end_time - start_time) );
            timer.transform.localScale = new Vector3( (size.x * 0.01f), lerp, 1f );
        }

		/**
		* 모든 위치 및 스케일의 값이 확정난 이후 스케일 조정
		**/
        void LateUpdate()
        {
            Vector3 new_scale = new Vector3(
                transform.localScale.x / transform.parent.localScale.x,
                transform.localScale.y / transform.parent.localScale.y,
                transform.localScale.z / transform.parent.localScale.z
            );
            transform.localScale = new_scale;
        }

        public void setRectSize( int width, int height )
        {
            size = new Vector2( width, height );
        }

		/**
		* 캐릭터에서 발사되는 경우 캐릭터를 따라다니게 하기 위한 기능
		* @uid - 시전자의 uid
		**/
        public void attachCharacter( int uid )
        {
            Npc actor = GameManager.charmgr.find( uid );
            if( actor == null )
                return;

            target = actor.gameObject;
            follower_spr_renderer = actor.spr;
            followFlip();
        }

		/**
		* 시전 캐릭터가 flip 시 같이 flip
		**/
        public void followFlip()
        {
            if( follower_spr_renderer.flipX == true )
                transform.localScale = new Vector3( -1f, 1f, 1f );
            else
                transform.localScale = new Vector3( 1f, 1f, 1f );
        }

        public void setPosition( Vector3 pos )
        {
            edge.transform.position = pos;
            timer.transform.position = pos;
        }

		/**
		* 사용 종료 후 destroy
		**/
        public void die()
        {
            target = null;
            gameObject.SetActive( false );
            Destroy( this.gameObject );
        }
    }
}