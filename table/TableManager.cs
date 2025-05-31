using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace game
{
    /**
     * TableManager
     * 테이블 데이터를 관리하는 클래스
     **/
    public class TableManager : SingletonBase< TableManager >
    {
        #region singleton class region
        TableManager() { }
        public static TableManager Instance
        {
            get
            {
                if( !Initialized )
                    Init( new TableManager() );

                return BasedInstance;
            }
        }
        #endregion

        public bool load_complete = false;

        public List< Task< int > > task_list = new List< Task< int > >();

        public Dictionary< Type, object > tables = new Dictionary< Type, object >();
        List< int > pcCanLearnSkills = new List< int >();
        public List< int > PCCanLearnSkills
        {
            get { return pcCanLearnSkills; }
        }

        List< int > itemSkills = new List< int >();
        public List< int > ItemSkills
        {
            get { return itemSkills; }
        }

        public async Task loadTable()
        {
            SkillTable skill_parser = new SkillTable();
            await skill_parser.start( "skill" );
            Dictionary< int, SkillDetailData > skill_table = skill_parser.parse();
            RegisterTable( skill_table );
            SetSkillDatas();

            MonsterTable monster_parser = new MonsterTable();
            await monster_parser.start( "monster" );
            Dictionary< int, Monster > monster_table = monster_parser.parse();
            RegisterTable( monster_table );

            MonsterWaveTable monster_wave_parser = new MonsterWaveTable();
            await monster_wave_parser.start( "monsterwave" );
            Dictionary< int, List< MonsterSpawnData > > wave_table = monster_wave_parser.parse();
            RegisterTable( wave_table );

            LevelTable level_parser = new LevelTable();
            await level_parser.start( "level" );
            Dictionary< int, Level > level_table = level_parser.parse();
            RegisterTable( level_table );

            SoundTable sound_parser = new SoundTable();
            await sound_parser.start( "sound" );
            Dictionary< int, string > sound_table = sound_parser.parse();
            RegisterTable( sound_table );

            load_complete = true;
        }

        private void RegisterTable< T >( Dictionary< int, T > table )
        {
            tables.Add( typeof( T ), table );
        }

        public T Get< T >( int id )
        {
            Type type = typeof( T );
            if( tables.TryGetValue( type, out var table ) )
            {
                var typed_table = table as Dictionary< int, T >;
                if( typed_table != null && typed_table.TryGetValue( id, out var result ) )
                {
                    return result;
                }
            }

            return default( T );
        }

        public void SetSkillDatas()
        {
            if( !tables.TryGetValue( typeof( SkillDetailData ), out var table ) )
            {
#if UNITY_EDITOR
                Debug.Log( "cant find skill table" );
#endif
                return;
            }

            Dictionary< int, SkillDetailData > skill_table = table as Dictionary< int, SkillDetailData >;
            foreach( var pair in skill_table )
            {
                SkillDetailData data = pair.Value;
                if( data.show_levelup_reward == true )
                {
                    pcCanLearnSkills.Add( pair.Key );
                }

                if( data.usage == SkillUsageType.ITEM )
                {
                    itemSkills.Add( pair.Key );
                }
            }
        }
    }
}