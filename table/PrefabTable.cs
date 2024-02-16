using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* PrefabTable
* 프리팹의 address를 관리
**/
[CreateAssetMenu( fileName = "prefab_address", menuName = "Scriptable Object/prefab_address", order = int.MaxValue )]
public class PrefabTable : ScriptableObject
{
    public List< string > address;
}
