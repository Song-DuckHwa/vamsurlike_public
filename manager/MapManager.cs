using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace game
{
    public class MapManager
    {
        //나중에 stage table로 빼서 받아오도록 변경해야 한다
        public string map_prefab_address = "prefabs/map/tilemap";

        public List< Tile > tile_list = new List< Tile >();

        private const int CONST_TILE_WIDTH = 2;
        private const int CONST_TILE_HEIGHT = 2;
        private const int CONST_TILE_SIZE = 2000;

        public void loadTile()
        {
            destroyTileSet();

            GameManager.prefabmgr.prefabs.TryGetValue( map_prefab_address, out GameObject obj );
            if( obj == null )
            {
                Debug.Log( "No has map" );
                return;
            }

            init();
        }

        public void init()
        {
            int i = 0;
            int loop_max_i = CONST_TILE_HEIGHT;
            for( ; i < loop_max_i ; ++i )
            {
                int j = 0;
                int loop_max_j = CONST_TILE_WIDTH;
                for( ; j < loop_max_j ; ++j )
                {
                    GameObject ins_tile = GameManager.poolmgr.instanceGet( map_prefab_address );
                    ins_tile.transform.SetParent( GameManager.gamelogic.grid.transform );
                    Tile tile_script = ins_tile.GetComponent< Tile >();
                    ins_tile.transform.position = new Vector3( -1000f + (CONST_TILE_SIZE * i), -1000f + (CONST_TILE_SIZE * j), 0f );
                    ins_tile.SetActive( true );

                    tile_list.Add( tile_script );

                    tile_script.OnCollideStart += OnTileCollideStart;
                    tile_script.OnCollideIng += OnTileCollideStart;
                }
            }
        }

        public void moveTileset( int chpos_quadrant, Vector2 main_tile_pos )
        {
            int index = 0;

            int offset_x = 0;
            int offset_y = 0;
            switch( chpos_quadrant )
            {
                case 1 :
                    offset_x = 1;
                    offset_y = 1;
                    break;
                case 2:
                    offset_x = -1;
                    offset_y = 1;
                    break;
                case 3 :
                    offset_x = -1;
                    offset_y = -1;
                    break;
                case 4 :
                    offset_x = 1;
                    offset_y = -1;
                    break;
                default :
                    break;
            }

            int i = 0;
            int loop_max_i = CONST_TILE_HEIGHT;
            for( ; i < loop_max_i ; ++i )
            {
                int j = 0;
                int loop_max_j = CONST_TILE_WIDTH;
                for( ; j < loop_max_j ; ++j )
                {
                    if( Vector2.Equals( main_tile_pos, (Vector2)tile_list[ index ].transform.position ) )
                    {
                        index++;
                    }

                    float pos_x = main_tile_pos.x + ((offset_x * CONST_TILE_SIZE) * j);
                    float pos_y = main_tile_pos.y + ((offset_y * CONST_TILE_SIZE) * i);

                    if( Vector2.Equals( main_tile_pos, new Vector2( pos_x, pos_y ) ) )
                    {
                        continue;
                    }

                    tile_list[ index ].transform.position = new Vector3( pos_x, pos_y, 0f );
                    index++;
                }
            }
        }

        public void OnTileCollideStart( OnActionCollideArg eventArgs )
        {
            //타일의 사분면 중 캐릭터는 어디에 있는가 찾아서 그에 따라 타일을 이동
            Vector2 event_tile_pos = new Vector2( eventArgs.tile.transform.position.x, eventArgs.tile.transform.position.y );
            moveTileset( eventArgs.quadrant, event_tile_pos );
        }

        public void destroyTileSet()
        {
            int i = tile_list.Count - 1;
            for( ; i > -1 ; --i )
            {
                tile_list[ i ].release();
                tile_list.RemoveAt( i );
            }
        }
    }
}