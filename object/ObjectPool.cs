using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace game
{
    public class ObjectPool
    {
        public int cap;
        public int pool_max;
        public GameObject prefab;

        public IObjectPool< GameObject > pool;

        public ObjectPool()
        {
            cap = 10;
            pool_max = 10;
            Init();
        }
        private void Init()
        {
            pool = new ObjectPool< GameObject >(
                OnCreate,
                OnGet,
                OnReturn,
                OnDestroy,
                true, cap, pool_max );
        }

        private GameObject OnCreate()
        {
            GameObject obj = Object.Instantiate( prefab );
            return obj;
        }

        private void OnGet( GameObject obj )
        {
            obj.SetActive( true );
        }

        private void OnReturn( GameObject obj )
        {
            obj.SetActive( false );
        }

        private void OnDestroy( GameObject obj )
        {
            Object.Destroy( obj );
        }
    }
}