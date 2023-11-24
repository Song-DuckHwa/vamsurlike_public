using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace game
{
    public class MapManager
    {
        public GameObject tile;
        public CollideFuncs collide = new CollideFuncs();

        public Dictionary< (int,int), GameObject > ins_tiles = new Dictionary< (int,int), GameObject >();

        public ( int, int ) pc_position = ( 0, 0 );

        public void loadTile()
        {
            string map_prefab_address = "Assets/prefabs/map/tile.prefab";
            if( GameManager.prefabmgr.prefabs.ContainsKey( map_prefab_address ) == false )
            {
                Debug.Log( "No has map" );
                return;
            }


            tile = GameManager.prefabmgr.prefabs[ map_prefab_address ];
            destroyTileset();
            ins_tiles = createNewTileset( pc_position.Item1, pc_position.Item2 );
        }

        public void Update()
        {
            int i = 0;
            List< (int, int) > key_arr = ins_tiles.Keys.ToList();
            int loop_max = key_arr.Count;
            for( ; i < loop_max ; ++i )
            {
                (int, int) key = key_arr[ i ];
                GameObject tile = ins_tiles[ key ];
                if( tile == null )
                {
                    Debug.Log( "Not has tile" );
                    return;
                }

                Vector3 mainch_pos = GameManager.mainch.gameobj.transform.position;
                float dist = Vector2.Distance( tile.transform.position, mainch_pos );
                //magic
                //사각형 대각선 길이로 체크
                float diagonal = 512f * Mathf.Sqrt( 2f );
                if( dist > diagonal )
                    continue;

                BoxCollider2D box = tile.GetComponent< BoxCollider2D >();

                Rect rect = new Rect();
                rect.xMin = tile.transform.position.x - (box.size.x * 0.5f);
                rect.xMax = tile.transform.position.x + (box.size.x * 0.5f);
                rect.yMin = tile.transform.position.y - (box.size.y * 0.5f);
                rect.yMax = tile.transform.position.y + (box.size.y * 0.5f);

                bool result = collide.pointToRectCollide( mainch_pos, rect );
                if( result )
                {
                    (int, int) quadrant = calcQuadrant( tile.transform.position, box, mainch_pos );
                    Dictionary< (int,int), GameObject > new_ins_tiles = createNewTileset( key.Item1, key.Item2, quadrant );

                    destroyTileset();

                    ins_tiles = new_ins_tiles;
                    pc_position = key;

                    return;
                }
            }
        }

        public Dictionary< (int,int), GameObject > createNewTileset( int center_x, int center_y )
        {
            Dictionary< (int,int), GameObject > new_ins_tiles = new Dictionary< (int,int), GameObject >();

            int i = 0;
            int loop_max_i = 3;
            for( ; i < loop_max_i ; ++i )
            {
                int j = 0;
                int loop_max_j = 3;
                for( ; j < loop_max_j ; ++j )
                {
                    int pos_x = (center_x - 1) + i;
                    int pos_y = (center_y - 1) + j;
                    GameObject ins_tile = GameObject.Instantiate( tile );
                    Tile tile_compo = ins_tile.GetComponent< Tile >();
                    tile_compo.position = ( pos_x, pos_y );
                    ins_tile.transform.position = new Vector3( 1024f * pos_x, 1024f * pos_y, 0f );
                    ins_tile.SetActive( true );

                    new_ins_tiles.Add( ( pos_x, pos_y ), ins_tile );
                }
            }

            return new_ins_tiles;
        }

        public Dictionary< (int,int), GameObject > createNewTileset( int center_x, int center_y, (int, int) quadrant )
        {
            if( quadrant.Item1 == 0 && quadrant.Item2 == 0 )
                return createNewTileset( center_x, center_y );

            Dictionary< (int,int), GameObject > new_ins_tiles = new Dictionary< (int,int), GameObject >();

            int i = 0;
            int loop_max_i = 2;
            for( ; i < loop_max_i ; ++i )
            {
                int j = 0;
                int loop_max_j = 2;
                for( ; j < loop_max_j ; ++j )
                {
                    int pos_x = center_x + (i * quadrant.Item1);
                    int pos_y = center_y + (j * quadrant.Item2);
                    GameObject ins_tile = GameObject.Instantiate( tile );
                    Tile tile_compo = ins_tile.GetComponent< Tile >();
                    tile_compo.position = ( pos_x, pos_y );
                    ins_tile.transform.position = new Vector3( 1024f * pos_x, 1024f * pos_y, 0f );
                    ins_tile.SetActive( true );

                    new_ins_tiles.Add( ( pos_x, pos_y ), ins_tile );
                }
            }

            return new_ins_tiles;
        }

        public void destroyTileset()
        {
            int i = 0;
            List< (int, int) > key_arr = ins_tiles.Keys.ToList();
            int loop_max = key_arr.Count;
            for( ; i < loop_max ; ++i )
            {
                (int, int) key = key_arr[ i ];
                Object.Destroy( ins_tiles[ key ].gameObject );
                ins_tiles[ key ] = null;
            }

            ins_tiles.Clear();
        }

        public (int, int) calcQuadrant( Vector3 rect_center_pos, BoxCollider2D box, Vector3 point )
        {
            Rect rt_rect = new Rect();
            rt_rect.xMin = rect_center_pos.x;
            rt_rect.xMax = rect_center_pos.x + (box.size.x * 0.5f);
            rt_rect.yMin = rect_center_pos.y;
            rt_rect.yMax = rect_center_pos.y + (box.size.y * 0.5f);
            bool result = collide.pointToRectCollide( point, rt_rect );
            if( result )
                return ((int)RectQuadrantX.R, (int)RectQuadrantY.T );

            Rect lt_rect = new Rect();
            lt_rect.xMin = rect_center_pos.x - (box.size.x * 0.5f);
            lt_rect.xMax = rect_center_pos.x;
            lt_rect.yMin = rect_center_pos.y;
            lt_rect.yMax = rect_center_pos.y + (box.size.y * 0.5f);
            result = collide.pointToRectCollide( point, lt_rect );
            if( result )
                return ((int)RectQuadrantX.L, (int)RectQuadrantY.T );

            Rect lb_rect = new Rect();
            lb_rect.xMin = rect_center_pos.x - (box.size.x * 0.5f);
            lb_rect.xMax = rect_center_pos.x;
            lb_rect.yMin = rect_center_pos.y - (box.size.y * 0.5f);
            lb_rect.yMax = rect_center_pos.y;
            result = collide.pointToRectCollide( point, lb_rect );
            if( result )
                return ((int)RectQuadrantX.L, (int)RectQuadrantY.B );

            Rect rb_rect = new Rect();
            rb_rect.xMin = rect_center_pos.x;
            rb_rect.xMax = rect_center_pos.x + (box.size.x * 0.5f);
            rb_rect.yMin = rect_center_pos.y - (box.size.y * 0.5f);
            rb_rect.yMax = rect_center_pos.y;
            result = collide.pointToRectCollide( point, rb_rect );
            if( result )
                return ((int)RectQuadrantX.R, (int)RectQuadrantY.B );

            return ((int)RectQuadrantX.NONE, (int)RectQuadrantY.NONE );
        }
    }

    enum RectQuadrantX
    {
        NONE = 0,
        R = 1,
        L = -1,
    }
    enum RectQuadrantY
    {
        NONE = 0,
        T = 1,
        B = -1,
    }
}