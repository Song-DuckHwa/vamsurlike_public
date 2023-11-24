using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace game
{
    /*
     * singleton manager
	 * 여기는 테이블이나 매니징 관련된 것들만 관리
     * 룰이나 시스템은 gamelogic에서 관리
     */
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        public CharacterManager charmgr_ = new CharacterManager();
        public static CharacterManager charmgr => instance.charmgr_;

        public TableManager tablemgr_ = new TableManager();
        public static TableManager tablemgr => instance.tablemgr_;

        public SkillManager skillmgr_ = new SkillManager();
        public static SkillManager skillmgr => instance.skillmgr_;

        public CharacterData mainch_;
        public static CharacterData mainch => instance.mainch_;

        public GameLogic gamelogic_;
        public static GameLogic gamelogic => instance.gamelogic_;

        public PrefabManager prefabmgr_ = new PrefabManager();
        public static PrefabManager prefabmgr => instance.prefabmgr_;

        public MapManager mapmgr_ = new MapManager();
        public static MapManager mapmgr => instance.mapmgr_;

        public Camera main_camera_;
        public static Camera main_camera => instance.main_camera_;

        public float res_v_half;
        public float res_h_half;

        private void Awake()
        {
            if( instance == null )
            {
                instance = this;
                DontDestroyOnLoad( instance );
            }
            else
            {
                if( instance != this )
                {
                    Destroy( this.gameObject );
                }
            }

            instance.init();
        }

        private async void init()
        {
            res_h_half = Camera.main.orthographicSize;
            res_v_half = res_h_half * Screen.width / Screen.height;

            //프리팹 로드가 끝날때 까지 대기
            await prefabmgr.start();

            charmgr.init();
            charmgr.createExpGemPool();

            bool result = tablemgr.loadTable();
            if( result )
                SettingComplete();
        }

        private void OnMove( InputValue value )
        {
            Vector2 input = value.Get<Vector2>();
            if( input != null )
            {
                if( mainch == null )
                    return;

                if( input == Vector2.zero )
                {
                    ((PC)mainch.script).velocity = 0;
                    return;
                }

                ((PC)mainch.script).direction = new Vector3( input.x, input.y, 0 );
                ((PC)mainch.script).velocity = 1;
            }
        }

        private void SettingComplete()
        {
            SceneManager.sceneLoaded += SceneChanged;
            SceneManager.LoadScene( "Game" );
        }

        private void SceneChanged( Scene scene, LoadSceneMode mode )
        {
            if( scene.name == "Game" )
            {
                //에디터에서 게임씬이 로드 되어 있기 때문에 gamelogic의 start가 에셋 로드 전에 호출된다.
                //그래서 일단 gamelogic을 비활성화 시킨 후 씬이 넘어간 순간에 활성화 해서 게임이 정상 순서로 돌게 한다.
                /*
                    todo : ui가 생기면서 해당 배열에 너무 많은 오브젝트가 생성된다. 다른 방법을 찾아봐야 한다.
                */
                GameObject gamelogic_obj = null;
                Transform[] objs = Resources.FindObjectsOfTypeAll< Transform >() as Transform[];
                for( int i = 0 ; i < objs.Length ; ++i )
                {
                    if( objs[ i ].hideFlags == HideFlags.None )
                    {
                        if( objs[ i ].CompareTag( "gamelogic" ) )
                        {
                            gamelogic_obj = objs[ i ].gameObject;
                        }
                    }
                }

                //GameObject gamelogic = GameObject.FindGameObjectWithTag( "gamelogic" );
                //GameLogic gl = FindObjectOfType< GameLogic >();
                if( gamelogic_obj == null )
                    return;

                gamelogic_ = gamelogic_obj.GetComponent< GameLogic >();
                gamelogic_obj.SetActive( true );
                gameObject.SetActive( true );

                main_camera_ = Camera.main;
            }
            else
            {
                gameObject.SetActive( false );
            }
        }

        public static int getCurrentGameTime()
        {
            return gamelogic.current_game_time_msec;
        }
    }

    enum CollideMask
    {
        NONE = 0,
        PC = 1,
        NPC = 2,
        MOB = 4,
        BULLET = 8,
        OBSTACLE = 16,
    }
}