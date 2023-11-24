using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace game
{
    public class PrefabManager
    {
        public Dictionary< string, GameObject > prefabs = new Dictionary< string, GameObject >();
        public bool load_complete = false;

        public async Task start()
        {
            if( load_complete == true )
                return;

            List< string > keys = new List< string >();
            /*------object------ */
            keys.Add( "Assets/prefabs/object/pcmain.prefab" );
            keys.Add( "Assets/prefabs/object/enemy.prefab" );
            keys.Add( "Assets/prefabs/object/exp.prefab" );
            keys.Add( "Assets/prefabs/object/yellow.prefab" );
            keys.Add( "Assets/prefabs/object/boss.prefab" );
            keys.Add( "Assets/prefabs/object/heal.prefab" );

            /*------skill------ */
            keys.Add( "Assets/prefabs/skillaoe.prefab" );
            keys.Add( "Assets/prefabs/skillbullet.prefab" );
            keys.Add( "Assets/prefabs/skillswing.prefab" );
            keys.Add( "Assets/prefabs/skillorbit.prefab" );

            keys.Add( "Assets/prefabs/skill/skilllaser.prefab" );
            keys.Add( "Assets/prefabs/skill/rectindicator.prefab" );

            /*------ui------ */
            keys.Add( "Assets/prefabs/ui/exp.prefab" );
            keys.Add( "Assets/prefabs/ui/hp.prefab" );

            /*------map------ */
            keys.Add( "Assets/prefabs/map/tile.prefab" );

            List< Task< int > > task_list = new List< Task< int > >();

            int i = 0;
            int loop_max = keys.Count;
            for( ; i < loop_max ; ++i )
            {
                string key = keys[ i ];
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
    }
}