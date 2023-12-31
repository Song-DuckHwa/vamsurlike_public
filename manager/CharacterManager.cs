using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace game
{
    public class CharacterManager
    {
        public static CharacterManager instance = null;

        private Dictionary< int, Npc > char_dic = new Dictionary< int, Npc >();
        private Dictionary< int, ExpGem > expgem_dic = new Dictionary< int, ExpGem >();
        private int current_uid;

        public void init()
        {
            reset();
            resetExpGem();
            current_uid = 0;
        }

        public bool deleteExpGem( int expgem_uid )
        {
            bool result = expgem_dic.Remove( expgem_uid );
            return result;
        }

        public void resetExpGem()
        {
            int i = 0;
            List< int > keys = expgem_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                expgem_dic.TryGetValue( key_i, out ExpGem obj );
                if( obj != null )
                    obj.release();

                expgem_dic.Remove( key_i );
            }
        }

        public Npc find( int uid )
        {
            char_dic.TryGetValue( uid, out Npc value );

            return value;
        }

        public Npc add( GameObject obj )
        {
            Npc script = obj.GetComponent< Npc >();
            script.uid = current_uid;
            char_dic.Add( current_uid, script );
            current_uid++;
            return script;
        }

        public bool delete( int uid )
        {
            bool result = char_dic.Remove( uid );
            return result;
        }

        public void clear()
        {
            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                delete( key_i );
            }
        }

        public void reset()
        {
            int i = 0;
            List< int > keys = char_dic.Keys.ToList();
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                int key_i = keys[ i ];
                char_dic.TryGetValue( key_i, out Npc ch );
                if( ch != null )
                    ch.release();

                delete( key_i );
            }
        }

        //공격판정과 캐릭터들이 충돌하였는가
        public List< int > attackCollideCheck( BoxCollider2D hitbox )
        {
            return CollideFuncs.attackCollideCheck( hitbox, char_dic );
        }

        public List< int > attackCollideCheck( CircleCollider2D hitbox )
        {
            return CollideFuncs.attackCollideCheck( hitbox, char_dic );
        }

        public GameObject addExpGem( int exp )
        {
            GameObject expgem = GameManager.poolmgr.instanceGet( "prefabs/object/exp" );
            if( expgem == null )
                return null;

            ExpGem expgem_script = expgem.GetComponent< ExpGem >();
            expgem_script.pool = GameManager.poolmgr.pools[ "prefabs/object/exp" ];
            expgem_script.exp = exp;
            expgem_script.uid = current_uid;

            expgem_dic.Add( current_uid, expgem_script );
            current_uid++;

            return expgem;
        }

        public void characterCollideProcess()
        {
            if( char_dic.Count > 1 )
            {
                //나중에 게임로직에서 체크하도록 수정해야 함
                CollideFuncs.check( char_dic );
            }
        }
    }
}
