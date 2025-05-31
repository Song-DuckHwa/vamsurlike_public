using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public GameObject effect;
    public int duration = 3;
    public float start_time;
    public float end_time;

    public GameObject hitbox;
    public int hitbix_duration = 1;
    public float hitbox_start_time;
    public float hitbox_end_time;

    public BoxCollider2D hitbox_collider;

    private void Awake()
    {
        hitbox_collider = hitbox.GetComponent< BoxCollider2D >();
    }

    void OnEnable()
    {
        start_time = Time.time;
        end_time = start_time + (duration * 0.1f);

        //start to 2f
        hitbox_start_time = Time.time + 0.1f;
        hitbox_end_time = hitbox_start_time + (hitbix_duration * 0.1f);
    }

    void Update()
    {
        if( Time.time > end_time )
        {
            effect.SetActive( false );
            return;
        }

        if( Time.time > hitbox_end_time )
        {
            hitbox.SetActive( false );
            return;
        }

        if( hitbox.activeSelf == true )
        {
            List< int > hitted_targets = GameManager.charmgr.attackCollideCheck( hitbox_collider );
            if( hitted_targets.Count > 0 )
            {
                int i = 0;
                int loop_max = hitted_targets.Count;
                for( ; i < loop_max ; ++i )
                {
                    Npc ch = GameManager.charmgr.find( hitted_targets[ i ] );
                    if( ch != null )
                    {
                        //ch.takeDamage( actor.gameObject, knockback_dist );
                    }
                }
            }
        }

        if( Time.time > hitbox_start_time )
        {
            hitbox.SetActive( true );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position, this.transform.localRotation * Vector3.up * 50.0f);
    }
}
