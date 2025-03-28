using UnityEngine;

namespace game
{
    /**
    * MonsterPatternSpawner
    * 몬스터 스폰을 랜덤 스폰이 아닌 패턴식으로 할 경우 사용하는 클래스
    **/
    public class MonsterPatternSpawner
    {
        /**
        * 데이터에서 스트링으로 된 데이터를 통해 어떤 패턴을 사용할 것인지 판단
        * @wave_data - 스폰 테이블의 웨이브 데이터
        **/
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

        /**
        * 세로 방향으로 늘어져 플레이어가 있는 곳을 향해 x축으로 전진
        * @spawn_data - 스폰 테이블의 웨이브 데이터
        **/
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

                Monster table_data = GameManager.tablemgr.Get< Monster >( spawn_data.monster_name );
                string prefab_name = $"prefabs/object/{table_data.prefab_name}";
                GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
                if( enemy_ins == null )
                    return null;

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
                //10sec
                enemy.setLifeTime( 10000 );
                enemy.move_dest = new Vector2( GameManager.mainch.transform.position.x, standard_y );
                enemy_ins.SetActive( true );

                //해당 웨이브가 스테이지 클리어에 필요한가
                if( spawn_data.stage_clear == 1 )
                {
                    if( GameManager.gamelogic.stage_clear_trigger == false )
                        GameManager.gamelogic.stage_clear_trigger = true;

                    GameManager.gamelogic.stage_clear_key_monsters.Add( enemy.uid );
                }
            }

            return spawner;
        }

        /**
        * 우물 정(#)자로 플레이어를 가둠
        * @spawn_data - 스폰 테이블의 웨이브 데이터
        **/
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

                Monster table_data = GameManager.tablemgr.Get< Monster >( spawn_data.monster_name );
                string prefab_name = $"prefabs/object/{table_data.prefab_name}";
                GameObject enemy_ins = GameManager.poolmgr.instanceGet( prefab_name );
                if( enemy_ins == null )
                    return null;

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
                //10sec
                enemy.setLifeTime( 10000 );
                enemy.move_dest = new Vector2( GameManager.mainch.transform.position.x, standard_y );
                enemy_ins.SetActive( true );

                //해당 웨이브가 스테이지 클리어에 필요한가
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