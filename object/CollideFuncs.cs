using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace game
{
    public class CollideFuncs
    {
        //원 - 원 충돌체크
        public void check( Dictionary< int, CharacterData > char_dic )
        {
            //현재 방식으로는 앞에서 진행했던 오브젝트들 끼리의 중복체크가 일어나게 됨
            //list의 뒤에서부터 검사해서 end와 end - 1 부터 출발하여 비교하는 식으로 하면 중복이 없어짐
            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                CharacterData main = new CharacterData();
                char_dic.TryGetValue( key_i, out main );
                if( main == null )
                    continue;

                int j = 0;
                int loop_max_j = keys.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    int key_j = keys[ j ];
                    CharacterData target = new CharacterData();
                    char_dic.TryGetValue( key_j, out target );
                    if( target == null )
                        continue;

                    if( main == target )
                        continue;

                    //충돌 마스크 체크 해서 충돌하지 않는 개체들이면 스킵
                    if( (((NPC)main.script).collide_mask & ((NPC)target.script).collide_type) == 0 )
                        continue;

                    float dist = Vector3.Distance( main.gameobj.transform.position, target.gameobj.transform.position );
                    //magic
                    if( dist > 50 )
                        continue;

                    Vector3 ray_dir = target.gameobj.transform.position - main.gameobj.transform.position;
                    ray_dir.Normalize();

                    Vector3 ray_start_pos = ray_dir;
                    ray_start_pos += main.gameobj.transform.position;

                    RaycastHit2D result = Physics2D.Raycast( ray_start_pos, ray_dir, 25f );
                    if( result.collider )
                    {
                        ((NPC)main.script).enemyCollide();
                        ((NPC)target.script).enemyCollide();

                        rollbackPosition( main.gameobj, target.gameobj );
                        rollbackPosition( target.gameobj, main.gameobj );
                    }
                }

            }
        }

        public void check_( Dictionary< int, CharacterData > char_dic )
        {
            int i = 0;
            int loop_max = char_dic.Count;
            for( ; i < loop_max ; ++i )
            {
                CharacterData main = char_dic[ i ];
                int j = 0;
                int loop_max_j = char_dic.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    CharacterData target = char_dic[ j ];
                    if( main == target )
                        continue;

                    bool result = rectToRectOBB( main.gameobj.GetComponent<BoxCollider2D>(), target.gameobj.GetComponent<BoxCollider2D>() );
                    if( result )
                    {
                        //현재 제대로 작동하지 않음. 원형의 롤백포지션이라서 그럼
                        rollbackPositionOBB( main.gameobj, target.gameobj );
                        rollbackPositionOBB( target.gameobj, main.gameobj );
                    }
                }
            }
        }

        //원형에서의 롤백
        public void rollbackPosition( GameObject main, GameObject col )
        {
            //magic
            float max_dist = 50f;
            float rollback_rad = Mathf.Atan2( main.transform.position.y - col.transform.position.y, main.transform.position.x - col.transform.position.x );
            Vector3 rollback_pos = new Vector3( Mathf.Cos( rollback_rad ), Mathf.Sin( rollback_rad ), 0 );
            rollback_pos.Normalize();
            rollback_pos *= max_dist;

            rollback_pos.x += col.transform.position.x;
            rollback_pos.y += col.transform.position.y;

            rollback_pos.x = rollback_pos.x - main.transform.position.x;
            rollback_pos.y = rollback_pos.y - main.transform.position.y;

            main.transform.Translate( rollback_pos );
        }

        //obb에서의 롤백
        public void rollbackPositionOBB( GameObject main, GameObject col )
        {
            //magic
            float max_dist = 50f;
            float rollback_rad = Mathf.Atan2( main.transform.position.y - col.transform.position.y, main.transform.position.x - col.transform.position.x );
            Vector3 rollback_pos = new Vector3( Mathf.Cos( rollback_rad ), Mathf.Sin( rollback_rad ), 0 );
            rollback_pos.Normalize();
            rollback_pos *= max_dist;

            rollback_pos.x += col.transform.position.x;
            rollback_pos.y += col.transform.position.y;

            rollback_pos.x = rollback_pos.x - main.transform.position.x;
            rollback_pos.y = rollback_pos.y - main.transform.position.y;

            main.transform.Translate( rollback_pos );
        }


        public bool rectToRectOBB( BoxCollider2D main, BoxCollider2D col )
        {
            Vector2 t = col.transform.position - main.transform.position;

            float rarb = 0f;
            Vector2[] l = {
                main.transform.up,
                col.transform.up,
                main.transform.right,
                col.transform.right,
            };

            int i = 0;
            int loop_max = l.Length;
            for( ; i < loop_max ; ++i )
            {
                float tl = Mathf.Abs( Vector2.Dot( t, l[ i ] ) );

                //magic
                rarb = Mathf.Abs( Vector2.Dot( l[ i ], main.transform.up ) * ((main.size.y * main.transform.localScale.y) * 0.5f) )
                + Mathf.Abs( Vector2.Dot( l[ i ], main.transform.right ) * ((main.size.x * main.transform.localScale.x) * 0.5f) )
                + Mathf.Abs( Vector2.Dot( l[ i ], col.transform.up ) * ((col.size.y * col.transform.localScale.y) * 0.5f) )
                + Mathf.Abs( Vector2.Dot( l[ i ], col.transform.right ) * ((col.size.x * col.transform.localScale.x) * 0.5f) );

                //모든 축 투영 검사가 통과해야 충돌한 것으로 인정
                if( tl > rarb )
                    return false;
            }

            return true;
        }

        //원-사각형 충돌 - 사각형이 회전하면 못씀
        public bool circleToRectCollide( CircleCollider2D circle, Rect rect )
        {

            float circle_radius = circle.radius * circle.transform.localScale.x;
            //사각형 범위 안에서 충돌한 경우
            if( (rect.xMin <= circle.transform.position.x && circle.transform.position.x <= rect.xMax) ||
                (rect.yMin <= circle.transform.position.y && circle.transform.position.y <= rect.yMax)
            )
            {
                Rect new_rect = new Rect();
                new_rect.xMin = rect.xMin - circle_radius;
                new_rect.xMax = rect.xMax + circle_radius;
                new_rect.yMin = rect.yMin - circle_radius;
                new_rect.yMax = rect.yMax + circle_radius;

                if( (new_rect.xMin <= circle.transform.position.x && circle.transform.position.x <= new_rect.xMax) &&
                    (new_rect.yMin <= circle.transform.position.y && circle.transform.position.y <= new_rect.yMax)
                )
                {
                    return true;
                }
            }
            //사각형 네 귀퉁이에서 충돌한 경우
            else
            {
                Vector2 lt = new Vector2( rect.xMin, rect.yMax );
                Vector2 lb = new Vector2( rect.xMax, rect.yMax );
                Vector2 rt = new Vector2( rect.xMin, rect.yMin );
                Vector2 rb = new Vector2( rect.xMax, rect.yMin );

                float lt_dist = Vector2.Distance( lt, circle.transform.position );
                if( lt_dist <= circle_radius )
                    return true;

                float lb_dist = Vector2.Distance( lb,circle.transform.position );
                if( lb_dist <= circle_radius )
                    return true;

                float rt_dist = Vector2.Distance( rt, circle.transform.position );
                if( rt_dist <= circle_radius )
                    return true;

                float rb_dist = Vector2.Distance( rb, circle.transform.position );
                if( rb_dist <= circle_radius )
                    return true;
            }

            return false;
        }

        //점-사각형 충돌 - 사각형이 회전하면 못씀
        public bool pointToRectCollide( Vector2 point, Rect rect )
        {
            if( (rect.xMin <= point.x && point.x <= rect.xMax) &&
                (rect.yMin <= point.y && point.y <= rect.yMax)
            )
            {
                return true;
            }

            return false;
        }

        /**
         *  직사각형의 공격판정과 캐릭터들이 hit 하는가 obb로 계산
         *  @hitbox - 공격 판정
         *  @char_dic - uid가 키값인 캐릭터 데이터
         */
        public List< int > attackCollideCheck( BoxCollider2D hitbox, Dictionary< int, CharacterData > char_dic )
        {
            List< int > hit_obj_uids = new List< int >();

            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key = keys[ i ];
                CharacterData ch = char_dic[ key ];
                //메인 캐릭터는 검사하지 않음
                if( ch.uid == GameManager.mainch.uid )
                    continue;

                bool result = rectToRectOBB( hitbox, ch.gameobj.GetComponent<BoxCollider2D>() );
                if( result )
                    hit_obj_uids.Add( ch.uid );
            }

            return hit_obj_uids;
        }

        /**
         *  원의 공격판정과 캐릭터들이 hit 하는가 계산
         *  @hitbox - 공격 판정
         *  @char_dic - uid가 키값인 캐릭터 데이터
         */
        public List< int > attackCollideCheck( CircleCollider2D hitbox, Dictionary< int, CharacterData > char_dic )
        {
            List< int > hit_obj_uids = new List< int >();

            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key = keys[ i ];
                CharacterData ch = char_dic[ key ];
                //메인 캐릭터는 검사하지 않음
                if( ch.uid == GameManager.mainch.uid )
                    continue;

                BoxCollider2D compo = ch.gameobj.GetComponent<BoxCollider2D>();

                Rect rect = new Rect();
                rect.xMin = compo.transform.position.x - (compo.size.x * 0.5f);
                rect.xMax = compo.transform.position.x + (compo.size.x * 0.5f);
                rect.yMin = compo.transform.position.y - (compo.size.y * 0.5f);
                rect.yMax = compo.transform.position.y + (compo.size.y * 0.5f);

                bool result = circleToRectCollide( hitbox, rect );
                if( result )
                    hit_obj_uids.Add( ch.uid );
            }

            return hit_obj_uids;
        }
    }
}