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
    /**
    * SkillLevelData
    * 스킬의 레벨 당 데이터
    **/
    public class SkillLevelData
    {
        public int damage;
        public int radius;
        //공격 빈도
        public int attack_tick;
        //공격 오브젝트 발생 수
        public int objcount;
    }
    /**
    * SkillDetailData
    * 스킬 하나의 기본 데이터
    **/
    public class SkillDetailData
    {
        //레벨업 시 보상목록에 보여지는가
        public bool show_levelup_reward;
        //용법 - 스킬인지, 아이템인지
        public int usage;
        //프리팹 address
        public string asset_address;
        //설명
        public List< string > description;
        //스킬의 레벨 당 데이터
        public List< SkillLevelData > level_data = new List< SkillLevelData >();
    }
    /**
    * SkillTableData
    * 스킬 테이블의 가장 상단에 위치한 클래스.
    * uid로 스킬 데이터를 찾고 해당 스킬을 pc가 배울 수 있는지, 아이템을 사용하는 스킬인지 판단.
    **/
    public class SkillTableData
    {
        public Dictionary< int, SkillDetailData > data = new Dictionary<int, SkillDetailData>();
        public SortedSet< int > pc_can_learn_uid = new SortedSet< int >();
        public SortedSet< int > item_uid = new SortedSet< int >();
    }

    /**
    * SkillTable
    * 스킬 테이블을 파싱하여 데이터로 들고 있는 클래스
    **/
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

        /**
        * string 형 데이터를 int형으로 변환
        **/
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

        /**
        * 파일 로딩 시작
        **/
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
                //thread
                var finish = await Task.WhenAny( task_list );
                task_list.Remove( finish );
                if( task_list.Count == 0 )
                    load_complete = true;
            }
        }

        /**
        * 파일 로딩 멀티쓰레드
        **/
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