using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
	/*
     * PrefabManager
     * 프리팹들을 로딩해서 메모리에 올려놓고 관리하는 매니저
     */
    public class PrefabManager
    {
        public Dictionary< string, GameObject > prefabs = new Dictionary< string, GameObject >();
        public bool load_complete = false;

        private List< string > task_keys = new List< string>();
        public List< Task< int > > task_list = new List< Task< int > >();

        public async Task start()
        {
            await loadScriptableObject( "object" );
            await loadScriptableObject( "skill" );
            await loadScriptableObject( "map" );
            await loadScriptableObject( "sound" );

            if( load_complete == true )
                return;

            int i = 0;
            int loop_max = task_keys.Count;
            for( ; i < loop_max ; ++i )
            {
                string key = task_keys[ i ];
                task_list.Add( Load( key ) );
            }

            while( task_list.Count > 0 )
            {
                var finish = await Task.WhenAny( task_list );
                task_list.Remove( finish );
                if( task_list.Count == 0 )
                    load_complete = true;
            }
        }

        public async Task<int> Load( string key )
        {
            //오브젝트 자체가 인스턴스화 되서 가져와지기 때문에 씬이 언로드 되면 오브젝트가 사라진다.
            //그래서 메모리에 쭉 들고 있게 하기 위해서 loadAsset으로 변경
            //AsyncOperationHandle handle = Addressables.InstantiateAsync( key );
            AsyncOperationHandle handle = Addressables.LoadAssetAsync< GameObject >( key );
            int task_id = handle.Task.Id;

            var obj = await handle.Task;

            ((GameObject)obj).SetActive( false );
            prefabs.Add( key, (GameObject)obj );

            return task_id;
        }

        public async Task loadScriptableObject( string key )
        {
            AsyncOperationHandle handle = Addressables.LoadAssetAsync< ScriptableObject >( $"address/{key}" );

            var obj = await handle.Task;

            PrefabTable table = (PrefabTable)obj;

            int i = 0;
            int loop_max = table.address.Count;
            for( ; i < loop_max ; ++i )
            {
                task_keys.Add( table.address[ i ] );
            }
        }
    }
}