/*
 * SingletonBase< T >
 * 제네릭 싱글톤 베이스 클래스
 */

using System;

public abstract class SingletonBase< T > where T : class
{
    protected static T _instance = null;    
    public static T BasedInstance
    {
        get
        {
            return _instance;
        }
    }

    protected static bool Initialized => _instance != null;
    
    protected static void Init( T newInstance )
    {
        if( newInstance == null )
            throw new ArgumentNullException();
 
        _instance = newInstance;
    }

    protected SingletonBase() { }
}