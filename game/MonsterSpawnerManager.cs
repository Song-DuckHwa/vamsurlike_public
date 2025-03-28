using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    /**
    * MonsterSpawnerManager
    * 몬스터 스폰을 관리하는 매니저
    **/
    public class MonsterSpawnerManager
    {
        public int current_stage_number;
        public int current_wave_number;

        public MonsterWaveData spawn_data;
        public MonsterSpawnData next_wave_data;

        public List< MonsterSpawner > spawner_list = new List< MonsterSpawner >();

        public MonsterPatternSpawner pattern_spawner = new MonsterPatternSpawner();

        /**
        * 스폰 데이터 초기화
        * @stage_number - 이 값의 스테이지 데이터를 찾아 데이터 세팅
        **/
        public void init( int stage_number )
        {
            clear();
            current_stage_number = stage_number;
            current_wave_number = 0;

            List< MonsterSpawnData > wave_list = GameManager.tablemgr.Get< List< MonsterSpawnData > >( stage_number );
            next_wave_data = wave_list[ current_wave_number ];
        }

        /**
        * 스폰 데이터 세팅
        * @current_game_time_msec - 현재 게임의 진행 시간. 스폰 타이밍인지 확인하기 위해 필요
        **/
        public void spawn( int current_game_time_msec )
        {
            if( next_wave_data != null )
            {
                //spawner add
                if( current_game_time_msec >= (next_wave_data.start_time * 1000) )
                {
                    MonsterSpawner spawner = new MonsterSpawner();

                    if( next_wave_data.pattern != "none" )
                    {
                        spawner = pattern_spawner.spawn( next_wave_data );
                        spawner_list.Add( spawner );
                    }

                    spawner.spawn_data.start_time = (int)(next_wave_data.start_time * 1000);
                    spawner.spawn_data.duration = (int)(next_wave_data.duration * 1000);
                    spawner.spawn_data.monster_name = next_wave_data.monster_name;
                    spawner.spawn_data.spawn_interval = (int)(next_wave_data.spawn_interval * 1000);
                    spawner.spawn_data.random_spawn_count = (int)next_wave_data.random_spawn_count;
                    spawner.spawn_data.stage_clear = (int)next_wave_data.stage_clear;
                    spawner.init();

                    spawner_list.Add( spawner );

                    current_wave_number += 1;

                    List< MonsterSpawnData > wave_list = GameManager.tablemgr.Get< List< MonsterSpawnData > >( current_stage_number );
                    if( current_wave_number < wave_list.Count )
                    {
                        next_wave_data = wave_list[ current_wave_number ];
                    }
                    else
                    {
                        next_wave_data = null;
                    }
                }
            }

            //spawn
            int i = spawner_list.Count - 1;
            for( ; i > -1 ; --i )
            {
                int end_time_msec = spawner_list[ i ].spawn_data.start_time + spawner_list[ i ].spawn_data.duration;
                //게임 시간이 스폰 종료 시간이라면 스포닝 리스트에서 스폰데이터 삭제
                if( current_game_time_msec >= end_time_msec )
                {
                    spawner_list.RemoveAt( i );
                    continue;
                }

                spawner_list[ i ].spawn( current_game_time_msec );
            }
        }

        /**
        * 스포닝 리스트 초기화
        **/
        public void clear()
        {
            int i = spawner_list.Count - 1;
            for( ; i > -1 ; --i )
            {
                spawner_list[ i ] = null;
                spawner_list.RemoveAt( i );
            }
        }
    }
}