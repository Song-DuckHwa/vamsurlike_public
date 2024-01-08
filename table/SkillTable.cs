using game;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
    public class SkillLevelData
    {
        public int damage;
        public int radius;
        public int attack_tick;
        public int objcount;
    }
    public class SkillDetailData
    {
        public bool show_levelup_reward;
        public int usage;
        public string asset_address;
        public List< string > description;
        public List< SkillLevelData > level_data = new List< SkillLevelData >();
    }

    public class SkillTableData
    {
        public Dictionary< int, SkillDetailData > data = new Dictionary<int, SkillDetailData>();
        public SortedSet< int > pc_can_learn_uid = new SortedSet< int >();
        public SortedSet< int > item_uid = new SortedSet< int >();
    }

    public class SkillTable
    {
        public List< Task< int > > task_list = new List< Task< int > >();
        public TextAsset table_org;
        public SkillTableData table;
        public bool load_complete = false;

        public SkillTableData parse()
        {
            string json_str = table_org.ToString();

            SkillTableData table = new SkillTableData();

            try
            {
                JObject json = JObject.Parse( json_str );

                IList< JProperty > prop = json.Properties().ToList();
                int i = 0 ;
                int loop_max = prop.Count;
                for( ; i < loop_max ; ++i )
                {
                    string key = prop[ i ].Name.ToString();

                    SkillDetailData data = new SkillDetailData();
                    data.show_levelup_reward = json[ key ][ "show_levelup_reward" ].ToObject< bool >();
                    string usage = json[ key ][ "usage" ].ToString();
                    data.usage = convertSkillUsageType( usage );
                    data.asset_address = json[ key ][ "asset_address" ].ToString();
                    JArray description = (JArray)json[ key ][ "description" ];
                    data.description = description.Select(c => (string)c).ToList();

                    JArray level_data = (JArray)json[ key ][ "level" ];
                    int j = 0 ;
                    int loop_max_j = level_data.Count;
                    for( ; j < loop_max_j ; ++j )
                    {
                        SkillLevelData level = new SkillLevelData();
                        level.damage = Int32.Parse( level_data[ j ][ "damage" ].ToString() );
                        level.radius = Int32.Parse( level_data[ j ][ "radius" ].ToString() );
                        level.attack_tick = Int32.Parse( level_data[ j ][ "attack_tick" ].ToString() );
                        level.objcount = Int32.Parse( level_data[ j ][ "objcount" ].ToString() );

                        data.level_data.Add( level );
                    }

                    table.data.Add( Int32.Parse( key ), data );

                    if( data.show_levelup_reward == true )
                        table.pc_can_learn_uid.Add( Int32.Parse( key ) );

                    if( data.usage == (int)SkillUsageType.ITEM )
                        table.item_uid.Add( Int32.Parse( key ) );
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }

            return table;
        }

        public int convertSkillUsageType( string usage_org )
        {
            if( usage_org.Contains( "repeat" ) == true )
                return (int)SkillUsageType.REPEAT;
            if( usage_org.Contains( "once" ) == true )
                return (int)SkillUsageType.ONCE;
            if( usage_org.Contains( "item" ) == true )
                return (int)SkillUsageType.ITEM;

            return (int)SkillUsageType.NONE;
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

    public enum SkillUsageType
    {
        NONE = 0,
        REPEAT,
        ONCE,
        ITEM,
        MAX,
    }
}