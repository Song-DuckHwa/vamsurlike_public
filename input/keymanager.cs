using UnityEngine;
using System;

public class KeyManager : SingletonBase< KeyManager >
{
    #region singleton class region
        KeyManager() { }
        public static KeyManager Instance
        {
            get
            {
                if( !Initialized )
                    Init( new KeyManager() );

                return BasedInstance;
            }
        }
        #endregion

    public Action keyaction = null;

    public void Update()
    {
        if( Input.anyKey == false )
            return;

        if( keyaction != null )
        {
            keyaction.Invoke();
        }        
    }
}
