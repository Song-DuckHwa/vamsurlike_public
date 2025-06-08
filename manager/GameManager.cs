using System.Collections.Generic;
using System.Linq;
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

        public SkillManager skillmgr_ = new SkillManager();
        public static SkillManager skillmgr => instance.skillmgr_;

        public GameLogic gamelogic_;
        public static GameLogic gamelogic => instance.gamelogic_;

        public PrefabManager prefabmgr_ = new PrefabManager();
        public static PrefabManager prefabmgr => instance.prefabmgr_;

        public MapManager mapmgr_ = new MapManager();
        public static MapManager mapmgr => instance.mapmgr_;

        public PoolManager poolmgr_ = new PoolManager();
        public static PoolManager poolmgr => instance.poolmgr_;

        public SoundManager soundmgr_ = new SoundManager();
        public static SoundManager soundmgr => instance.soundmgr_;



        public PCMain mainch_;
        public static PCMain mainch => instance.mainch_;

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
            //default 30frame
            Application.targetFrameRate = 60;

            res_h_half = Camera.main.orthographicSize;
            res_v_half = res_h_half * Screen.width / Screen.height;

            //프리팹 로드가 끝날때 까지 대기
            await prefabmgr.start();

            //prefab pool add
            if( poolmgr.add_complete == false )
            {
                int i = 0;
                List< string > key_list = prefabmgr.prefabs.Keys.ToList();
                int loop_max = key_list.Count;
                for( ; i < loop_max ; ++i )
                {
                    string key = key_list[ i ];
                    poolmgr.addPool( key );
                }

                poolmgr.add_complete = true;
            }

            charmgr.init();

            await TableManager.Instance.loadTable();
            bool result = TableManager.Instance.load_complete;
            if( result )
                SettingComplete();
        }

        /*
         * 모든 테이블 및 프리팹 로딩이 완료되었다면 게임 씬을 로딩함
         */
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