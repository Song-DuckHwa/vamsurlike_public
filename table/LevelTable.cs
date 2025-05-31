using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
    public class LevelTableData
    {
        public Dictionary< int, Level > stat = new Dictionary< int, Level >();
    }

    public class Level
    {
        public int hp;
        public int atk;
        public int speed;
        public int exp;
    }

    public class LevelTable
    {
        public List< Task< int > > task_list = new List< Task< int > >();
        public TextAsset table_org;
        public SkillTableData table;
        public bool load_complete = false;

        public LevelTableData _parse()
        {
            string json_str = table_org.ToString();

            LevelTableData data = new LevelTableData();

            try
            {
                data.stat = JsonConvert.DeserializeObject< Dictionary< int, Level > >( json_str );
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }

            /*
            JObject json = JObject.Parse( json_str );
            IList<string> keys = json.Properties().Select(p => p.Name).ToList();
            */

            return data;
        }

        public Dictionary< int, Level > parse()
        {
            string json_str = table_org.ToString();

            Dictionary< int, Level > data = new Dictionary< int, Level >();

            try
            {
                data = JsonConvert.DeserializeObject< Dictionary< int, Level > >( json_str );
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