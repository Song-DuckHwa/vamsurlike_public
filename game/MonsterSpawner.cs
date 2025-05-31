using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
    public class MonsterSpawner
    {
        public MonsterSpawnData spawn_data = new MonsterSpawnData();
        private int next_spawn_time_msec;

        public void init()
        {
            next_spawn_time_msec = 0;
        }

        public void spawn( int current_game_time_msec )
        {
            if( current_game_time_msec >= next_spawn_time_msec )
            {
                int i = 0;
                int max_spawn_count = spawn_data.random_spawn_count + 1;
                int loop_max = Random.Range( 1, max_spawn_count );
                for( ; i < loop_max ; ++i )
                {
                    //0~359
                    int random_spawn_degree = Random.Range( 0, 360 );
                    float random_spawn_radian = (float)random_spawn_degree * mathlib.DEG_TO_RAD;
                    float random_x = (GameManager.instance.res_v_half * Mathf.Cos( random_spawn_radian )) + GameManager.mainch.transform.position.x;
                    float random_y = (GameManager.instance.res_h_half * Mathf.Sin( random_spawn_radian )) + GameManager.mainch.transform.position.y;

                    Vector3 spawn_pos = new Vector3( random_x, random_y, 0 );
                    Monster table_data = TableManager.Instance.Get< Monster >( spawn_data.monster_name );

                    string prefab_name = $"prefabs/object/{table_data.prefab_name}";
                    GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
                    if( enemy_ins == null )
                        return;

                    Npc enemy = GameManager.charmgr.add( enemy_ins );

                    Monster monster = new Monster();
                    monster.hp = table_data.hp;
                    monster.atk = table_data.atk;
                    monster.move_speed = table_data.move_speed;
                    monster.exp = table_data.exp;
                    monster.skill_index = table_data.skill_index;

                    int j = 0;
                    int loop_max_j = (int)monster.skill_index.Count;
                    for( ; j < loop_max_j ; ++j )
                    {
                        enemy.skillmgr.learnSkill( monster.skill_index[ i ], enemy.uid );
                    }

                    enemy.setStat( monster );
                    enemy_ins.transform.position = spawn_pos;
                    enemy_ins.SetActive( true );

                    if( spawn_data.stage_clear == 1 )
                    {
                        if( GameManager.gamelogic.stage_clear_trigger == false )
                            GameManager.gamelogic.stage_clear_trigger = true;

                        GameManager.gamelogic.stage_clear_key_monsters.Add( enemy.uid );
                    }
                }

                next_spawn_time_msec = (int)current_game_time_msec + spawn_data.spawn_interval;
            }
        }
    }
}