using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class MonsterPatternSpawner
    {
        public MonsterSpawner spawn( MonsterSpawnData wave_data )
        {
            string pattern_type = wave_data.pattern.ToLower();

            MonsterSpawner spawner = null;
            switch( pattern_type )
            {
                case "press_vertical_rail" :
                    spawner = pressVerticalRail( wave_data );
                    break;
                case "colosseum" :
                    spawner = colosseum( wave_data );
                    break;
                default :
                    break;
            }
            return spawner;
        }

        public MonsterSpawner pressVerticalRail( MonsterSpawnData spawn_data )
        {
            MonsterSpawner spawner = new MonsterSpawner();

            int i = 0;
            //magic
            int loop_max = 120;
            for( ; i < loop_max ; ++i )
            {
                //float 계산으로 인해 좌표가 정확하지 않으면 충돌판정에서 서로 밀려나 이상하게 세팅된다
                float standard_x = Mathf.Ceil( GameManager.mainch.transform.position.x );
                float standard_y = Mathf.Ceil( GameManager.mainch.transform.position.y ) - 3000;

                if( i < 60 )
                {
                    standard_x -= 1000;
                    standard_y += ((i % 60) * 100);
                }
                else
                {
                    standard_x += 1000;
                    standard_y += ((i % 60) * 100);
                }

                Vector3 spawn_pos = new Vector3( standard_x, standard_y, 0 );

                string prefab_name = $"prefabs/object/{GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].prefab_name}";
                GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
                if( enemy_ins == null )
                    return null;

                Npc enemy = GameManager.charmgr.add( enemy_ins );
                Monster monster = new Monster();
                monster.hp = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].hp;
                monster.atk = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].atk;
                monster.move_speed = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].move_speed;
                monster.exp = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].exp;
                monster.skill_index = GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].skill_index;

                int j = 0;
                int loop_max_j = (int)monster.skill_index.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    enemy.skillmgr.learnMonsterSkill( monster.skill_index[ i ], enemy.uid );
                }

                enemy.setStat( monster );
                enemy_ins.transform.position = spawn_pos;
                //10sec
                enemy.setLifeTime( 10000 );
                enemy.move_dest = new Vector2( GameManager.mainch.transform.position.x, standard_y );
                enemy_ins.SetActive( true );

                if( spawn_data.stage_clear == 1 )
                {
                    if( GameManager.gamelogic.stage_clear_trigger == false )
                        GameManager.gamelogic.stage_clear_trigger = true;

                    GameManager.gamelogic.stage_clear_key_monsters.Add( enemy.uid );
                }
            }

            return spawner;
        }

        public MonsterSpawner colosseum( MonsterSpawnData spawn_data )
        {
            MonsterSpawner spawner = new MonsterSpawner();

            int i = 0;
            //magic
            int loop_max = 80;
            for( ; i < loop_max ; ++i )
            {
                //float 계산으로 인해 좌표가 정확하지 않으면 충돌판정에서 서로 밀려나 이상하게 세팅된다
                float standard_x = Mathf.Ceil( GameManager.mainch.transform.position.x );
                float standard_y = Mathf.Ceil( GameManager.mainch.transform.position.y );

                if( i < 20 )
                {
                    standard_x -= 1000;
                    standard_x += ((i % 20) * 100);
                    standard_y += 500;
                }
                else if( i < 40 )
                {
                    standard_x -= 1000;
                    standard_x += ((i % 20) * 100);
                    standard_y -= 500;
                }
                else if( i < 60 )
                {
                    standard_x -= 500;
                    standard_y -= 1000;
                    standard_y += ((i % 20) * 100);
                }
                else
                {
                    standard_x += 500;
                    standard_y -= 1000;
                    standard_y += ((i % 20) * 100);
                }

                Vector3 spawn_pos = new Vector3( standard_x, standard_y, 0 );

                string prefab_name = $"prefabs/object/{GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].prefab_name}";
                GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
                if( enemy_ins == null )
                    return null;

                Npc enemy = GameManager.charmgr.add( enemy_ins );
                Monster monster = new Monster();
                monster.hp = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].hp;
                monster.atk = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].atk;
                monster.move_speed = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].move_speed;
                monster.exp = (int)GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].exp;
                monster.skill_index = GameManager.tablemgr.monster.monster_data[ spawn_data.monster_name ].skill_index;

                int j = 0;
                int loop_max_j = (int)monster.skill_index.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    enemy.skillmgr.learnMonsterSkill( monster.skill_index[ i ], enemy.uid );
                }

                enemy.setStat( monster );
                enemy_ins.transform.position = spawn_pos;
                //10sec
                enemy.setLifeTime( 10000 );
                enemy.move_dest = new Vector2( GameManager.mainch.transform.position.x, standard_y );
                enemy_ins.SetActive( true );

                if( spawn_data.stage_clear == 1 )
                {
                    if( GameManager.gamelogic.stage_clear_trigger == false )
                        GameManager.gamelogic.stage_clear_trigger = true;

                    GameManager.gamelogic.stage_clear_key_monsters.Add( enemy.uid );
                }
            }

            return spawner;
        }
    }
}