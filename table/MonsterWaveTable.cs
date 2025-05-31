using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
    public class StageSpawnData
    {
        public List< MonsterWaveData > stage_spawn_data = new List< MonsterWaveData >();
    }

    public class MonsterWaveData
    {
        public List< MonsterSpawnData > monster_spawn_data = new List< MonsterSpawnData >();
    }

    public class MonsterSpawnData
    {
        public int start_time;
        public int duration;
        public int monster_name;
        public int spawn_interval;
        public int random_spawn_count;
        public string pattern;
        public int stage_clear;
    }

    public class MonsterWaveTable
    {
        public List< Task< int > > task_list = new List< Task< int > >();
        public TextAsset table_org;
        public SkillTableData table;
        public bool load_complete = false;

        public StageSpawnData _parse()
        {
            string json_str = table_org.ToString();

            StageSpawnData data = new StageSpawnData();

            try
            {
                JObject json = JObject.Parse( json_str );

                IList< JToken > stages = json[ "stage" ].Children().ToList();
                int i = 0;
                int loop_max = stages.Count;
                for( ; i < loop_max ; ++i )
                {
                    IList< JToken > stage_spawn_arr = stages[ i ].Children().ToList();
                    int j = 0;
                    int loop_max_ = stage_spawn_arr.Count;

                    MonsterWaveData stage_spawn_data = new MonsterWaveData();

                    for( ; j < loop_max_ ; ++j )
                    {
                        JObject obj = JObject.Parse( stage_spawn_arr[ j ].ToString() );

                        MonsterSpawnData monster_spawn_data = new MonsterSpawnData();

                        monster_spawn_data.start_time = Int32.Parse( obj[ "start_time" ].ToString() );
                        monster_spawn_data.duration = Int32.Parse( obj[ "duration" ].ToString() );
                        monster_spawn_data.monster_name = Int32.Parse( obj[ "monster_name" ].ToString() );
                        monster_spawn_data.spawn_interval = Int32.Parse( obj[ "spawn_interval" ].ToString() );
                        monster_spawn_data.random_spawn_count = Int32.Parse( obj[ "random_spawn_count" ].ToString() );
                        monster_spawn_data.pattern = obj[ "pattern" ].ToString();
                        monster_spawn_data.stage_clear = Int32.Parse( obj[ "stage_clear" ].ToString() );

                        stage_spawn_data.monster_spawn_data.Add( monster_spawn_data );
                    }

                    data.stage_spawn_data.Add( stage_spawn_data );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }


            return data;
        }

        public Dictionary< int, List< MonsterSpawnData > > parse()
        {
            string json_str = table_org.ToString();

            Dictionary< int, List< MonsterSpawnData > > data = new Dictionary< int, List< MonsterSpawnData > >();

            try
            {
                JObject json = JObject.Parse( json_str );

                IList< JToken > stages = json[ "stage" ].Children().ToList();
                int i = 0;
                int loop_max = stages.Count;
                for( ; i < loop_max ; ++i )
                {
                    IList< JToken > stage_spawn_arr = stages[ i ].Children().ToList();
                    int j = 0;
                    int loop_max_ = stage_spawn_arr.Count;

                    List< MonsterSpawnData > stage_spawn_data = new List< MonsterSpawnData >();

                    for( ; j < loop_max_ ; ++j )
                    {
                        JObject obj = JObject.Parse( stage_spawn_arr[ j ].ToString() );

                        MonsterSpawnData monster_spawn_data = new MonsterSpawnData();

                        monster_spawn_data.start_time = Int32.Parse( obj[ "start_time" ].ToString() );
                        monster_spawn_data.duration = Int32.Parse( obj[ "duration" ].ToString() );
                        monster_spawn_data.monster_name = Int32.Parse( obj[ "monster_name" ].ToString() );
                        monster_spawn_data.spawn_interval = Int32.Parse( obj[ "spawn_interval" ].ToString() );
                        monster_spawn_data.random_spawn_count = Int32.Parse( obj[ "random_spawn_count" ].ToString() );
                        monster_spawn_data.pattern = obj[ "pattern" ].ToString();
                        monster_spawn_data.stage_clear = Int32.Parse( obj[ "stage_clear" ].ToString() );

                        stage_spawn_data.Add( monster_spawn_data );
                    }

                    data.Add( i, stage_spawn_data );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }


            return data;
        }

        public async Task start( string file_name )
        {
            List< string > keys = new List< string >();
            keys.Add( $"table/{file_name}" );


            int i = 0;
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                string key = keys[ i ];
                task_list.Add( fileLoad( key ) );
            }

            while( task_list.Count > 0 )
            {
                var finish = await Task.WhenAny( task_list );
                task_list.Remove( finish );
                if( task_list.Count == 0 )
                    load_complete = true;
            }
        }
        public async Task<int> fileLoad( string key )
        {
            AsyncOperationHandle handle = Addressables.LoadAssetAsync< TextAsset >( key );
            int task_id = handle.Task.Id;

            var obj = await handle.Task;

            table_org = (TextAsset)obj;

            return task_id;
        }
    }
}