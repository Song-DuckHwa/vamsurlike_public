using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
    public class SoundTable
    {
        public List< Task< int > > task_list = new List< Task< int > >();
        public TextAsset table_org;
        public bool load_complete = false;

        public List< string > _parse()
        {
            string json_str = table_org.ToString();

            List< string > data = new List< string >();

            try
            {
                JArray json = JArray.Parse( json_str );

                int i = 0;
                int loop_max = json.Count;
                for( ; i < loop_max ; ++i )
                {
                    data.Add( json[ i ].ToString() );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }


            return data;
        }

        public Dictionary< int, string > parse()
        {
            string json_str = table_org.ToString();

            Dictionary< int, string > data = new Dictionary< int, string >();

            try
            {
                JArray json = JArray.Parse( json_str );

                int i = 0;
                int loop_max = json.Count;
                for( ; i < loop_max ; ++i )
                {
                    data.Add( i, json[ i ].ToString() );
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