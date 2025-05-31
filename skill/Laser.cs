using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace game
{
    public class Laser : Skill
    {
        public int delayed_time = 0;
        public float start_time = 0;
        public int effect_duration = 0;
        public int knockback_dist = 0;

        public GameObject effect_ref;
        public GameObject effect_ins;
        public SpriteRenderer effect_spr_renderer;

        public float x_offset;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Npc actor = GameManager.charmgr.find( actor_uid );
            if( actor == null )
            {
                die();
                return;
            }


            if( gameObject.activeSelf == true && GameManager.getCurrentGameTime() >= (start_time + delayed_time + effect_duration) )
            {
                die();
                return;
            }

            if( effect_spr_renderer.color == Color.white )
            {
                List< int > hitted_targets = GameManager.charmgr.attackCollideCheck( (BoxCollider2D)collider_compo );
                if( hitted_targets.Count > 0 )
                {
                    int i = 0;
                    int loop_max = hitted_targets.Count;
                    for( ; i < loop_max ; ++i )
                    {
                        //collide mask로 변경해야 함
                        if( hitted_targets[ i ] == actor_uid )
                            continue;

                        Npc ch = GameManager.charmgr.find( hitted_targets[ i ] );
                        if( ch != null )
                        {
                            if( hitted_targets_set.TryGetValue( hitted_targets[ i ], out int target_uid ) == false )
                            {
                                ch.takeDamage( actor.gameObject, knockback_dist );
                                hitted_targets_set.Add( hitted_targets[ i ] );
                            }
                        }
                    }
                }
            }

            if( effect_spr_renderer.color != Color.white && GameManager.getCurrentGameTime() >= start_time + delayed_time )
            {

                float deg = (actor.direction.x < 0) ? 180 : 0;
                transform.eulerAngles = new Vector3( 0f, 0f, deg );

                effect_spr_renderer.color = Color.white;
                transform.position = new Vector3(
                    actor.transform.position.x,
                    actor.transform.position.y,
                    actor.transform.position.z
                );

                GameManager.soundmgr.sfxs[ SFX.LASER ].Play();
            }
        }

        public override bool active()
        {
            SkillDetailData table = getTableData();
            if( table == null )
                return true;

            Npc actor = GameManager.charmgr.find( actor_uid );
            if( actor == null )
                return true;

            /*-----indicator-----*/
            GameObject indicator_obj = GameManager.prefabmgr.prefabs[ "prefabs/skill/rectindicator" ];
            GameObject indicator_ins = Instantiate( indicator_obj );

            RectIndicator indicator_ins_script = indicator_ins.GetComponent< RectIndicator >();
            //magic
            indicator_ins_script.duration = 5000;
            indicator_ins_script.setRectSize( 2000, 300 );

            SpriteRenderer actor_spr_renderer = actor.spr.GetComponent< SpriteRenderer >();
            indicator_ins.transform.position = actor.transform.position;
            indicator_ins.transform.SetParent( actor.transform );

            indicator_ins_script.setPosition( new Vector3(
                actor.transform.position.x + (actor_spr_renderer.sprite.bounds.size.x * 0.5f),
                actor.transform.position.y,
                actor.transform.position.z
            ));
            indicator_ins_script.attachCharacter( actor.uid );
            indicator_ins.SetActive( true );

            /*-----laser-----*/
            //딜레이 만큼 뒤에 발사하고 repeat time에 다시 생성
            repeat_time_msec = GameManager.getCurrentGameTime() + 7000;

            GameObject ins = GameManager.poolmgr.instanceGet( table.asset_address );
            if( ins == null )
                return true;

            Laser script = ins.GetComponent< Laser >();
            script.skill_index = skill_index;
            script.actor_uid = actor_uid;
            script.collider_compo = ins.GetComponent< BoxCollider2D >();
            //magic
            script.start_time = GameManager.getCurrentGameTime();
            script.delayed_time = 5000;
            script.effect_duration = 1000;

            script.effect_spr_renderer = ins.GetComponent< SpriteRenderer >();
            script.effect_spr_renderer.color = Color.clear;
            script.x_offset = actor_spr_renderer.sprite.bounds.size.x * 0.5f;

            ins.SetActive( true );

            return false;
        }
    }
}