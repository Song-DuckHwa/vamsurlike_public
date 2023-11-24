using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace game
{
    public class CharacterData
    {
        public int uid;
        public GameObject gameobj;
        public Component script;
    }

    public class CharacterManager
    {
        public static CharacterManager instance = null;

        private Dictionary< int, CharacterData > char_dic = new Dictionary< int, CharacterData >();
        private Dictionary< int, ExpGem > expgem_dic = new Dictionary< int, ExpGem >();
        private int current_uid;
        private CollideFuncs collidemgr = new CollideFuncs();

        public ObjectPool expgem_pool;

        public void init()
        {
            reset();
            resetExpGem();
            current_uid = 0;
        }

        public void createExpGemPool()
        {
            expgem_pool = new ObjectPool();
            string skill_prefab_address = "Assets/prefabs/object/exp.prefab";

            GameManager.prefabmgr.prefabs.TryGetValue( skill_prefab_address, out GameObject obj );
            if( obj == null )
                return;

            expgem_pool.prefab = obj;
        }

        public void deleteExpGem()
        {

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
                    obj.pool.Release( obj.gameObject );

                expgem_dic.Remove( key_i );
            }
        }

        public CharacterData find( int uid )
        {
            CharacterData value = new CharacterData();
            char_dic.TryGetValue( uid, out value );

            return value;
        }

        public CharacterData add( GameObject obj )
        {
            CharacterData set = new CharacterData();
            set.uid = current_uid;
            set.gameobj = obj;
            set.script = obj.GetComponent<Entity>();
            ((NPC)set.script).uid = current_uid;
            char_dic.Add( current_uid, set );
            current_uid++;
            return set;
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
                CharacterData obj = new CharacterData();
                char_dic.TryGetValue( key_i, out obj );
                if( obj != null )
                    Object.Destroy( obj.gameobj );

                delete( key_i );
            }
        }

        //공격판정과 캐릭터들이 충돌하였는가
        public List< int > attackCollideCheck( BoxCollider2D hitbox )
        {
            return collidemgr.attackCollideCheck( hitbox, char_dic );
        }

        public List< int > attackCollideCheck( CircleCollider2D hitbox )
        {
            return collidemgr.attackCollideCheck( hitbox, char_dic );
        }

        public GameObject addExpGem( int exp )
        {
            GameObject expgem = expgem_pool.pool.Get();
            ExpGem expgem_compo = expgem.GetComponent< ExpGem >();
            expgem_compo.pool = expgem_pool.pool;
            expgem_compo.exp = exp;

            expgem_dic.Add( current_uid, expgem_compo );
            current_uid++;

            return expgem;
        }

        public void characterCollideProcess()
        {
            if( char_dic.Count > 1 )
            {
                //나중에 게임로직에서 체크하도록 수정해야 함
                collidemgr.check( char_dic );
            }
        }
    }
}
