using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace game
{
    public class PoolManager
    {
        public Dictionary< string, ObjectPool > pools = new Dictionary< string, ObjectPool >();
        public bool add_complete = false;


        public void addPool( string prefab_address )
        {
            GameManager.prefabmgr.prefabs.TryGetValue( prefab_address, out GameObject org );
            if( org == null )
                return;

            ObjectPool pool = new ObjectPool();
            pool.prefab = org;
            pools.Add( prefab_address, pool );
        }

        public GameObject instanceGet( string prefab_address )
        {
            pools.TryGetValue( prefab_address, out ObjectPool obj_pool );
            if( obj_pool == null )
                return null;

            GameObject obj = obj_pool.pool.Get();
            Entity obj_script = obj.GetComponent< Entity >();
            if( obj_script )
                obj_script.pool = obj_pool;

            return obj;
        }

        public void clear()
        {
            int i = 0;
            List< string > key_arr = pools.Keys.ToList();
            int loop_max = key_arr.Count;
            for( ; i < loop_max ; ++i )
            {
                string key = key_arr[ i ];
                pools[ key ].clear();
            }
        }
    }
}