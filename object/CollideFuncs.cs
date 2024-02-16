using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace game
{
	/**
	* CollideFuncs
	* 충돌 계산 함수들 관장 클래스
	**/
    public static class CollideFuncs
    {
        /**
		* 원 - 원 충돌
		* @char_dic - 생성된 캐릭터 uid 목록
		**/
        public static void check( Dictionary< int, Npc > char_dic )
        {
            //현재 방식으로는 앞에서 진행했던 오브젝트들 끼리의 중복체크가 일어나게 됨
            //list의 뒤에서부터 검사해서 end와 end - 1 부터 출발하여 비교하는 식으로 하면 중복이 없어짐
            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                char_dic.TryGetValue( key_i, out Npc main );
                if( main == null )
                    continue;

                int j = 0;
                int loop_max_j = keys.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    int key_j = keys[ j ];
                    char_dic.TryGetValue( key_j, out Npc target );
                    if( target == null )
                        continue;

                    if( main == target )
                        continue;

                    //충돌 마스크 체크 해서 충돌하지 않는 개체들이면 스킵
                    if( (main.collide_mask & target.collide_type) == 0 )
                        continue;

                    if( main.state == (int)STATE.DEAD || target.state == (int)STATE.DEAD )
                        continue;

                    float dist = Vector3.Distance( main.transform.position, target.transform.position );
                    float max_dist = (main.ccol.radius * main.transform.localScale.x) + (target.ccol.radius * target.transform.localScale.x);
                    if( dist > max_dist )
                        continue;

                    main.enemyCollide();
                    target.enemyCollide();

                    rollbackPosition( main, target );
                    rollbackPosition( target, main );
                }

            }
        }

        /**
		* 원 - 원 충돌 레이캐스트로
		* @char_dic - 생성된 캐릭터 uid 목록
		**/
        public static void check_raycast( Dictionary< int, Npc > char_dic )
        {
            //현재 방식으로는 앞에서 진행했던 오브젝트들 끼리의 중복체크가 일어나게 됨
            //list의 뒤에서부터 검사해서 end와 end - 1 부터 출발하여 비교하는 식으로 하면 중복이 없어짐
            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                char_dic.TryGetValue( key_i, out Npc main );
                if( main == null )
                    continue;

                int j = 0;
                int loop_max_j = keys.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    int key_j = keys[ j ];
                    char_dic.TryGetValue( key_j, out Npc target );
                    if( target == null )
                        continue;

                    if( main == target )
                        continue;

                    //충돌 마스크 체크 해서 충돌하지 않는 개체들이면 스킵
                    if( (main.collide_mask & target.collide_type) == 0 )
                        continue;

                    float dist = Vector3.Distance( main.transform.position, target.transform.position );
                    //magic
                    if( dist > 50 )
                        continue;

                    Vector3 ray_dir = target.transform.position - main.transform.position;
                    ray_dir.Normalize();

                    Vector3 ray_start_pos = ray_dir;
                    ray_start_pos += main.transform.position;

                    RaycastHit2D result = Physics2D.Raycast( ray_start_pos, ray_dir, 25f );
                    if( result.collider )
                    {
                        main.enemyCollide();
                        target.enemyCollide();

                        rollbackPosition( main, target );
                        rollbackPosition( target, main );
                    }
                }

            }
        }

		/**
		* 사각 - 사각 충돌 - 현재 작동하지 않음
		* @char_dic - 생성된 캐릭터 uid 목록
		**/
        public static void check_( Dictionary< int, Npc > char_dic )
        {
            int i = 0;
            int loop_max = char_dic.Count;
            for( ; i < loop_max ; ++i )
            {
                Npc main = char_dic[ i ];
                int j = 0;
                int loop_max_j = char_dic.Count;
                for( ; j < loop_max_j ; ++j )
                {
                    Npc target = char_dic[ j ];
                    if( main == target )
                        continue;

                    bool result = rectToRectOBB( main.gameObject.GetComponent<BoxCollider2D>(), target.gameObject.GetComponent<BoxCollider2D>() );
                    if( result )
                    {
                        //현재 제대로 작동하지 않음. 원형의 롤백포지션이라서 그럼
                        rollbackPositionOBB( main.gameObject, target.gameObject );
                        rollbackPositionOBB( target.gameObject, main.gameObject );
                    }
                }
            }
        }

        /**
		* 원형 충돌에서의 위치 롤백
		* @main - 충돌 주체
		* @col - 충돌 피격체
		**/
        public static void rollbackPosition( Npc main, Npc col )
        {
            float main_radius = main.ccol.radius * main.transform.localScale.x;
            float col_radius = col.ccol.radius * col.transform.localScale.x;
            float max_dist = main_radius + col_radius;

            //밀려질 방향
            float rollback_rad = Mathf.Atan2( main.transform.position.y - col.transform.position.y, main.transform.position.x - col.transform.position.x );
            Vector3 rollback_pos = new Vector3( Mathf.Cos( rollback_rad ), Mathf.Sin( rollback_rad ), 0 );
            rollback_pos.Normalize();
            rollback_pos *= max_dist;

            //충돌한 외곽 지역의 위치를 파악
            rollback_pos.x += col.transform.position.x;
            rollback_pos.y += col.transform.position.y;

            //충돌 외곽 지역의 차이 만큼 탐지 대상을 밀어낸다
            rollback_pos.x = rollback_pos.x - main.transform.position.x;
            rollback_pos.y = rollback_pos.y - main.transform.position.y;

            main.transform.Translate( rollback_pos );
        }

        /**
		* 사각형 obb 충돌에서의 롤백 - 현재 작동하지 않음
		* @main - 충돌 주체
		* @col - 충돌 피격체
		**/
        public static void rollbackPositionOBB( GameObject main, GameObject col )
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

		/**
		* 원형 충돌에서 넉백
		* @main - 충돌 주체
		* @col - 충돌 피격체
		* @knockback_dist - 넉백 거리
		**/
        public static void knockbackPosition( GameObject main, GameObject col, float knockback_dist )
        {
            knockback_dist = knockback_dist < 0 ? 1 : knockback_dist;

            float rollback_rad = Mathf.Atan2( col.transform.position.y - main.transform.position.y, col.transform.position.x - main.transform.position.x );
            Vector3 rollback_pos = new Vector3( Mathf.Cos( rollback_rad ), Mathf.Sin( rollback_rad ), 0 );
            rollback_pos.Normalize();
            rollback_pos *= knockback_dist;

            rollback_pos.x += col.transform.position.x;
            rollback_pos.y += col.transform.position.y;

            rollback_pos.x = rollback_pos.x - col.transform.position.x;
            rollback_pos.y = rollback_pos.y - col.transform.position.y;

            col.transform.Translate( rollback_pos );
        }

		/**
		* 사각형 obb 충돌 계산
		* @main - 충돌 주체
		* @col - 충돌 피격체
		**/
        public static bool rectToRectOBB( BoxCollider2D main, BoxCollider2D col )
        {
            float rad = main.transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 new_main_pos = new Vector2(
                main.transform.position.x + (main.offset.x * Mathf.Cos( rad )),
                main.transform.position.y + (main.offset.x * Mathf.Sin( rad ))
            );

            rad = col.transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 new_col_pos = new Vector2(
                col.transform.position.x + (col.offset.x * Mathf.Cos( rad )),
                col.transform.position.y + (col.offset.x * Mathf.Sin( rad ))
            );

            Vector2 t = new_col_pos - new_main_pos;

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

		/**
		* 원-사각형 충돌 - 사각형이 회전하면 못씀
		* @circle - 충돌 객체가 가진 CircleCollider2D
		* @rect - 충돌 피객체가 가진 BoxCollider2D의 rect만
		**/
        public static bool circleToRectCollide( CircleCollider2D circle, Rect rect )
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

		/**
		* 점-사각형 충돌 - 사각형이 회전하면 못씀
		* @point - 충돌 객체 위치
		* @rect - 충돌 피객체가 가진 BoxCollider2D의 rect만
		**/
        public static bool pointToRectCollide( Vector2 point, Rect rect )
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
        public static List< int > attackCollideCheck( BoxCollider2D hitbox, Dictionary< int, Npc > char_dic )
        {
            List< int > hit_obj_uids = new List< int >();

            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key = keys[ i ];
                Npc ch = char_dic[ key ];

                if( ch.state == (int)STATE.DEAD )
                    continue;

                bool result = rectToRectOBB( hitbox, ch.gameObject.GetComponent<BoxCollider2D>() );
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
        public static List< int > attackCollideCheck( CircleCollider2D hitbox, Dictionary< int, Npc > char_dic )
        {
            List< int > hit_obj_uids = new List< int >();

            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key = keys[ i ];
                Npc ch = char_dic[ key ];

                if( ch.state != (int)STATE.RUN )
                    continue;

                BoxCollider2D compo = ch.gameObject.GetComponent<BoxCollider2D>();

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