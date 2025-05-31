using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace game
{
    /*
     * 룰이나 시스템은 여기서 관리
     */
    public class GameLogic : MonoBehaviour
    {
        public UIManager uimgr;
        public MonsterSpawnerManager spawnermgr = new MonsterSpawnerManager();

        public int current_game_time_msec;

        public bool stage_clear_trigger = false;
        public SortedSet< int > stage_clear_key_monsters = new SortedSet< int >();

        public Inventory inventory = new Inventory();
        public LevelupRewardSelector levelup_reward_selector = new LevelupRewardSelector();

        public Grid grid;

        // Start is called before the first frame update
        void Start()
        {
            init();
        }

        void init()
        {
            pauseGame( false );
            current_game_time_msec = 0;
            stage_clear_trigger = false;

            GameManager.charmgr.init();
            GameManager.mapmgr.loadTile();
            GameManager.soundmgr.init();

            GameManager.soundmgr.bgm.Play();

            levelup_reward_selector.clear();
            inventory.clear();

            //pc먼저 추가 하고 추가가 되었다면
            //이후 몬스터를 추가
            spawnPlayer();
            //spawnMonsterDebug();
            uimgr.initUI();

            //기본 공격을 스킬로 배우기
            const int NORMAL_ATTACK_INDEX = 1;
            addItemMainPC( NORMAL_ATTACK_INDEX );

            const int STAGE_ONE = 0;
            spawnermgr.init( STAGE_ONE );

            Camera.main.GetComponent< CameraScript >().setFollowObject( GameManager.mainch.gameObject );
        }

        void release()
        {
            GameManager.charmgr.init();
            GameManager.mapmgr.destroyTileSet();
            GameManager.poolmgr.clear();
            GameManager.soundmgr.clear();
        }

        // Update is called once per frame
        void Update()
        {
            if( stage_clear_trigger == true && stage_clear_key_monsters.Count == 0 )
            {
                stage_clear_trigger = false;
                GameClear().Forget();
            }

            Npc player = GameManager.charmgr.find( 0 );
            if( player == null )
                return;

            current_game_time_msec += (int)(Time.deltaTime * 1000f);

            uimgr.setCurrentTime( current_game_time_msec );

            GameManager.charmgr.characterCollideProcess();

            //DebugSpawnLotsOfGem();
            spawnermgr.spawn( current_game_time_msec );
        }

        async UniTaskVoid GameClear()
        {
            await UniTask.WaitForSeconds( 1f );

            uimgr.ui_result.GetComponent< UIResult >().stageClear();
            GameManager.soundmgr.bgm.Stop();
            GameManager.soundmgr.sfxs[ SFX.CLEAR ].Play();
            pauseGame( true );
        }

        void spawnPlayer()
        {
            string prefab_name = "prefabs/object/pcmain";
            GameObject pcmain_ins = GameManager.poolmgr.instanceGet( prefab_name );
            if( pcmain_ins == null )
                return;

            GameManager.instance.mainch_ = (PCMain)GameManager.charmgr.add( pcmain_ins );
            PCMain mainch = GameManager.mainch;
            pcmain_ins.SetActive( true );

            Level table = TableManager.Instance.Get< Level >( mainch.level );

            Level stat = new Level();
            stat.hp = table.hp;
            stat.atk = table.atk;
            stat.speed = table.speed;
            stat.exp = table.exp;
            mainch.setStat( stat );
            mainch.equalizingHP();

            mainch.transform.position = new Vector3( 0f, 0f, 0f );
        }

        void spawnMonsterDebug()
        {
            /*------------- 1 -------------*/
            //collide test
            Vector3 spawn_pos = new Vector3(200, 0, 0);
            string prefab_name = "prefabs/object/enemy";
            GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
            if( enemy_ins == null )
                return;

            GameManager.charmgr.add( enemy_ins );
            enemy_ins.transform.position = spawn_pos;

            Monster monster = new Monster();
            monster.hp = 1000;
            monster.atk = 1;
            monster.move_speed = 0;
            monster.exp = 1;
            enemy_ins.GetComponent<Npc>().setStat( monster );

            enemy_ins.SetActive( true );

            /*------------- 2 -------------*/
            //collide test
            spawn_pos = new Vector3( 100, 100, 0 );
            enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
            if( enemy_ins == null )
                return;

            GameManager.charmgr.add( enemy_ins );
            enemy_ins.transform.position = spawn_pos;

            monster = new Monster();
            monster.hp = 1000;
            monster.atk = 1;
            monster.move_speed = 0;
            monster.exp = 1;
            enemy_ins.GetComponent<Npc>().setStat( monster );

            enemy_ins.SetActive( true );
        }

        void DebugSpawnLotsOfGem()
        {
            GameObject expgem = GameManager.charmgr.addExpGem( 1 );
            Vector2 pos = new Vector2( Random.Range( -640, 640 ), Random.Range( -360, 360 ) );
            expgem.transform.position = pos;
        }

        public void generateExpGem( int uid, int exp )
        {
            Npc ch = GameManager.charmgr.find( uid );
            if( ch == null )
                return;

            GameObject expgem = GameManager.charmgr.addExpGem( exp );
            expgem.transform.position = ch.transform.position;
            expgem.SetActive( true );
        }

        public void generateHealItem( int uid )
        {
            //1% magic
            int random = Random.Range( 0, 1000 );
            if( random > 10 )
                return;

            Npc ch = GameManager.charmgr.find( uid );
            if( ch == null )
                return;

            GameManager.prefabmgr.prefabs.TryGetValue( "prefabs/object/heal", out GameObject heal_item );
            if( heal_item == null )
                return;

            GameObject ins_heal_item = Instantiate( heal_item );
            ins_heal_item.transform.position = ch.transform.position;
            ins_heal_item.SetActive( true );
        }

        public void addMainPCExp( int add_exp )
        {
            if( GameManager.mainch == null )
                return;

            uimgr.exp_bar.addExp( add_exp );

            GameManager.mainch.current_exp += add_exp;
            if( GameManager.mainch.current_exp >= GameManager.mainch.max_exp )
            {
                levelUpMainPC();
            }
        }

        public void levelUpMainPC()
        {
            GameManager.mainch.current_exp = GameManager.mainch.current_exp - GameManager.mainch.max_exp;
            GameManager.mainch.level += 1;

            Level table = TableManager.Instance.Get< Level >( GameManager.mainch.level );

            Level stat = new Level();
            stat.hp = table.hp;
            stat.atk = table.atk;
            stat.speed = table.speed;
            stat.exp = table.exp;
            GameManager.mainch.setStat( stat );

            pauseGame( true );

            levelup_reward_selector.reset();
            uimgr.levelUp();

            GameManager.soundmgr.bgm.volume = 0.1f;
            GameManager.soundmgr.sfxs[ SFX.LEVELUP ].Play();
        }

        public void addItemMainPC( int reward_index )
        {
            int skill_index = reward_index;
            if( levelup_reward_selector.reward_list.Count > 0 )
                skill_index = levelup_reward_selector.reward_list[ reward_index ].skill_index;

            GameManager.mainch.skillmgr.learnSkill( skill_index, GameManager.mainch.uid );

            SkillDetailData table = TableManager.Instance.Get< SkillDetailData >( skill_index );
            if( table == null )
            {
#if UNITY_EDITOR
                Debug.Log( $"cant find data - { skill_index }" );
#endif
                return;
            }

            if( table.usage != SkillUsageType.ITEM )
                GameManager.gamelogic.inventory.levelUp( skill_index );

            GameManager.gamelogic.uimgr.ui_levelup.gameObject.SetActive( false );
            GameManager.gamelogic.pauseGame( false );
            GameManager.soundmgr.bgm.volume = 0.5f;
        }

        public void damagedMainPC()
        {
            uimgr.hp_bar.decreaseHp( 1 );
            //죽었을때 리스너로 빼야할듯
            if( GameManager.mainch.current_hp <= 0 )
            {
                uimgr.ui_result.GetComponent< UIResult >().gameOver();
                pauseGame( true );
                //GameManager.instance.mainch_ = null;
                Camera.main.GetComponent< CameraScript >().stopFollow();

                GameManager.soundmgr.bgm.Stop();
                GameManager.soundmgr.sfxs[ SFX.FAIL ].Play();
            }
        }

        public void calcCurrentHPMainPC( int value )
        {
            uimgr.hp_bar.calcHP( value );
            GameManager.mainch.current_hp += value;
            //죽었을때 리스너로 빼야할듯
            if( GameManager.mainch.current_hp <= 0 )
            {
                uimgr.ui_result.GetComponent< UIResult >().gameOver();
                pauseGame( true );
                //GameManager.instance.mainch_ = null;
                Camera.main.GetComponent< CameraScript >().stopFollow();

                GameManager.soundmgr.bgm.Stop();
                GameManager.soundmgr.sfxs[ SFX.FAIL ].Play();
            }
        }

        public void pauseGame( bool isPause )
        {
            if( isPause )
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;

            uimgr.dpad.gameObject.SetActive( !isPause );
        }

        public void restart()
        {
            uimgr.ui_result.gameObject.SetActive( false );
            init();
        }

        public void gotoTitle()
        {
            release();
            SceneManager.LoadScene( "Title" );
        }
    }
}