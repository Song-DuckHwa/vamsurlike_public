using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#endif

namespace game
{
    /**
    * Dpad
    * 캐릭터의 움직임을 관장하는 버츄얼 패드 ui
    **/
    public class Dpad : MonoBehaviour
    {
        public GameObject stick;
        public EventSystem eventsystem;

        private void Awake()
        {
#if UNITY_EDITOR
            EnhancedTouchSupport.Enable();
#endif
        }

        void OnEnable()
        {
            transform.position = new Vector3( -200f, -200f, 0f );
            stick.transform.localPosition = new Vector3( 0f, 0f, 0f );
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if( Touch.activeFingers.Count == 1 )
            {
                Touch activeTouch = Touch.activeFingers[0].currentTouch;

                //Debug.Log($"Phase: {activeTouch.phase} | Position: {activeTouch.startScreenPosition}");

                //터치가 들어왔을때 패드의 위치를 터치 위치로 옮김
                if( activeTouch.phase == TouchPhase.Began )
                {
                    Vector2 touch_pos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
                    transform.position = touch_pos;
                }

                //터치 후 움직일 때 조이스틱 내부에 드래그 데이터를 넘겨줌
                if( activeTouch.phase == TouchPhase.Moved )
                {
                    transform.position = activeTouch.startScreenPosition;

                    Vector2 touch_pos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
                    PointerEventData data = new PointerEventData( eventsystem );
                    data.position = touch_pos;
                    stick.GetComponent< OnScreenStick >().OnDrag( data );
                }

                //터치 후 손을 뗐을 때 버츄얼 패드의 위치를 다시 원래대로 되돌림
                if( activeTouch.phase == TouchPhase.Ended )
                {
                    Vector2 touch_pos = new Vector2( -200f, -200f );
                    transform.position = touch_pos;

                    PointerEventData data = new PointerEventData( eventsystem );
                    stick.GetComponent< OnScreenStick >().OnPointerUp( data );
                }
            }
#else
            if( Input.touchCount > 0 )
            {

                Touch input = Input.GetTouch( 0 );

                //터치가 들어왔을때 패드의 위치를 터치 위치로 옮김
                if( input.phase == TouchPhase.Began )
                {
                    Vector2 touch_pos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
                    transform.position = touch_pos;
                }

                //터치 후 움직일 때 조이스틱 내부에 드래그 데이터를 넘겨줌
                if( input.phase == TouchPhase.Moved )
                {
                    transform.position = input.rawPosition;

                    Vector2 touch_pos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
                    PointerEventData data = new PointerEventData( eventsystem );
                    data.position = touch_pos;
                    stick.GetComponent< OnScreenStick >().OnDrag( data );
                }

                //터치 후 손을 뗐을 때 버츄얼 패드의 위치를 다시 원래대로 되돌림
                if( input.phase == TouchPhase.Ended )
                {
                    Vector2 touch_pos = new Vector2( -200f, -200f );
                    transform.position = touch_pos;

                    PointerEventData data = new PointerEventData( eventsystem );
                    stick.GetComponent< OnScreenStick >().OnPointerUp( data );
                }
            }
#endif
        }
    }
}