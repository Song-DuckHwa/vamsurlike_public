using game;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Unity.VisualScripting;
using UnityEngine;

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
        public string asset_address;
        public List< string > description;
        public List< SkillLevelData > level_data = new List< SkillLevelData >();
    }

    public class SkillTableData
    {
        public Dictionary< int, SkillDetailData > data = new Dictionary<int, SkillDetailData>();
    }

    public class SkillTable
    {
        public SkillTableData load( string file_name )
        {
            string path = Path.Combine( Application.dataPath + "/data/table/", $"{file_name}.json" );
            string json_str = File.ReadAllText( path );

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
                }
            }
            catch( Exception e )
            {
                Debug.Log( e );
            }

            return table;
        }
    }
}