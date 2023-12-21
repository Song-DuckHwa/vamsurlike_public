using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "prefab_address", menuName = "Scriptable Object/prefab_address", order = int.MaxValue )]
public class PrefabTable : ScriptableObject
{    
    public List< string > address;
}
