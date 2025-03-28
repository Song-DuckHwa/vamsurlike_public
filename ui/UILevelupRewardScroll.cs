using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    /**
    * UILevelupRewardScroll
    * 레벨업 시 나오는 보상 목록이 있는 scroll rect를 관리
    **/
    public class UILevelupRewardScroll : MonoBehaviour
    {
        public GraphicRaycaster canvas_raycaster;
        public GameObject content;
        public List< UIRewardElement > elements = new List< UIRewardElement >();
        public RectTransform content_rect;
        public ScrollRect scroll_rect;

        public float top_pos;
        public float bottom_pos;

        public float prev_scroll_move;
        public int scroll_move_dist;

        public int top_uid;
        public int bottom_uid;

        public float button_size;
        public int button_count;

        private void Awake()
        {
            top_pos = 0;
            bottom_pos = 0;

            button_count = elements.Count;

            top_uid = 0;
            bottom_uid = button_count - 1;

            content_rect = content.GetComponent< RectTransform >();
            scroll_rect = GetComponent< ScrollRect >();

            button_size = Mathf.Abs( elements[ 2 ].transform.position.y - elements[ 1 ].transform.position.y );
        }

        /**
        * 활성화 될 때 스크롤의 초기화 및 자동 움직임을 세팅
        **/
        private void OnEnable()
        {
            top_uid = 0;
            bottom_uid = button_count - 1;
            prev_scroll_move = 0;
            scroll_move_dist = 0;
            scroll_rect.vertical = true;
            content.transform.localPosition = new Vector2( 0f, 0f );

            int i = 0;
            int loop_max = elements.Count;
            for( ; i < loop_max ; ++i )
            {
                elements[ i ].transform.localPosition = new Vector2( elements[ i ].transform.localPosition.x, 220 - (button_size * i) );
            }

            scroll_rect.velocity = new Vector2( 0f, 5000f );
            //element가 회전 중일 때에는 상호작용을 안해야 한다
            canvas_raycaster.enabled = false;
            //StartCoroutine( ScrollStop() );
            ScrollStop().Forget();
        }

        /**
        * unitask로 프레임별로 체크하면서 특정 속도에 이르렀을때 정확한 위치에 멈추는 애니메이션 실행
        **/
        public async UniTaskVoid ScrollStop()
        {
            while( true )
            {
                if( scroll_rect.velocity.y < 100 )
                {
                    scroll_rect.velocity = new Vector2( 0f, 0f );
                    ScrollLastAni().Forget();
                    return;
                }

                await UniTask.NextFrame();
            }
        }

        /**
        * 스크롤의 속도가 특정 속도 이하로 내려갔을 경우 1씩 움직이며 정확한 위치에서 스톱
        **/
        public async UniTaskVoid ScrollLastAni()
        {
            int dist = 0;
            int content_pos = (int)Mathf.Round( content.transform.localPosition.y );
            content.transform.localPosition = new Vector2( content.transform.localPosition.x, content_pos );
            dist = (int)button_size - (content_pos % (int)button_size);

            while( true )
            {
                dist -= 1;
                content.transform.localPosition = new Vector2( content.transform.localPosition.x, content.transform.localPosition.y + 1 );
                if( dist == 0 )
                {
                    //element가 회전 중이 끝났을 때 다시 상호작용 on
                    canvas_raycaster.enabled = true;
                    scroll_rect.vertical = false;
                    return;
                }

                await UniTask.NextFrame();
            }
        }

        /**
        * 스크롤의 속도가 특정 속도 이하로 내려갔을 경우 1씩 움직이며 정확한 위치에서 스톱
        **/
        public void OnValueChanged( Vector2 pos )
        {
            int round_pos_y = (int)Mathf.Round( content.transform.localPosition.y );
            content.transform.localPosition = new Vector2( content.transform.localPosition.x, round_pos_y );

            int delta = (int)Mathf.Round( prev_scroll_move - content_rect.localPosition.y );
            scroll_move_dist += delta;

            //아래 스크롤
            if( scroll_move_dist >= button_size )
            {
                Vector2 element_localpos = elements[ bottom_uid ].gameObject.transform.localPosition;
                elements[ bottom_uid ].gameObject.transform.localPosition = new Vector2( element_localpos.x, element_localpos.y + (button_size * elements.Count) );
                top_uid = bottom_uid;
                bottom_uid--;
                if( bottom_uid < 0 )
                    bottom_uid = button_count - 1;

                // +값이므로 -해야 값이 줄어들음
                scroll_move_dist -= (int)button_size;
            }

            //위 스크롤
            if( scroll_move_dist <= -button_size )
            {
                Vector2 element_localpos = elements[ top_uid ].gameObject.transform.localPosition;
                elements[ top_uid ].gameObject.transform.localPosition = new Vector2( element_localpos.x, element_localpos.y + (-button_size * elements.Count) );
                bottom_uid = top_uid;
                top_uid++;
                if( top_uid > button_count - 1 )
                    top_uid = 0;

                // -값이므로 +해야 값이 줄어들음
                scroll_move_dist += (int)button_size;
            }

            prev_scroll_move = content_rect.localPosition.y;
        }
    }
}