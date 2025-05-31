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
    public class MonsterData
    {
        public Dictionary< string, Monster > monster_data = new Dictionary< string, Monster >();
    }

    public class Monster
    {
        public string prefab_name;
        public int hp;
        public int atk;
        public int move_speed;
        public int exp;
        public List< int > skill_index = new List< int >();
    }

    public class MonsterTable
    {
        public List< Task< int > > task_list = new List< Task< int > >();
        public TextAsset table_org;
        public SkillTableData table;
        public bool load_complete = false;

        public MonsterData _parse()
        {
            string json_str = table_org.ToString();

            MonsterData table = new MonsterData();
            try
            {
                //data.monster_data = JsonConvert.DeserializeObject< Dictionary< string, Monster > >( json_str );

                JObject json = JObject.Parse( json_str );

                IList< JProperty > prop = json.Properties().ToList();
                int i = 0 ;
                int loop_max = prop.Count;
                for( ; i < loop_max ; ++i )
                {
                    string key = prop[ i ].Name.ToString();

                    Monster data = new Monster();
                    data.prefab_name = json[ key ][ "prefab_name" ].ToString();
                    data.hp = Int32.Parse( json[ key ][ "hp" ].ToString() );
                    data.atk = Int32.Parse( json[ key ][ "atk" ].ToString() );
                    data.move_speed = Int32.Parse( json[ key ][ "move_speed" ].ToString() );
                    data.exp = Int32.Parse( json[ key ][ "exp" ].ToString() );

                    JArray skill_index = (JArray)json[ key ][ "skill" ];
                    data.skill_index = skill_index.Select(c => (int)c).ToList();

                    table.monster_data.Add( key, data );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }


            /*
            JObject json = JObject.Parse( json_str );
            IList<string> keys = json.Properties().Select(p => p.Name).ToList();
            */

            return table;
        }

        public Dictionary< int, Monster > parse()
        {
            string json_str = table_org.ToString();

            Dictionary< int, Monster > table = new Dictionary< int, Monster >();
            try
            {
                JObject json = JObject.Parse( json_str );

                IList< JProperty > prop = json.Properties().ToList();
                int i = 0 ;
                int loop_max = prop.Count;
                for( ; i < loop_max ; ++i )
                {
                    string key = prop[ i ].Name.ToString();

                    Monster data = new Monster();
                    data.prefab_name = json[ key ][ "prefab_name" ].ToString();
                    data.hp = Int32.Parse( json[ key ][ "hp" ].ToString() );
                    data.atk = Int32.Parse( json[ key ][ "atk" ].ToString() );
                    data.move_speed = Int32.Parse( json[ key ][ "move_speed" ].ToString() );
                    data.exp = Int32.Parse( json[ key ][ "exp" ].ToString() );

                    JArray skill_index = (JArray)json[ key ][ "skill" ];
                    data.skill_index = skill_index.Select(c => (int)c).ToList();

                    table.Add( Int32.Parse( key ), data );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }

            return table;
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
            //오브젝트 자체가 인스턴스화 되서 가져와지기 때문에 씬이 언로드 되면 오브젝트가 사라진다.
            //그래서 메모리에 쭉 들고 있게 하기 위해서 loadAsset으로 변경
            //AsyncOperationHandle handle = Addressables.InstantiateAsync( key );
            AsyncOperationHandle handle = Addressables.LoadAssetAsync< TextAsset >( key );
            int task_id = handle.Task.Id;

            var obj = await handle.Task;

            table_org = (TextAsset)obj;

            return task_id;
        }
    }
}