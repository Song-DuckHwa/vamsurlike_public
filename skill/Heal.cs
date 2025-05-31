using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    public class Heal : Skill
    {
        public override bool active()
        {
            SkillDetailData table = getTableData();
            if( table == null )
                return true;

            Npc actor = GameManager.charmgr.find( actor_uid );
            if( actor == null )
                return true;

            GameObject ins = GameManager.poolmgr.instanceGet( table.asset_address );
            if( ins == null )
                return true;

            Heal script = ins.GetComponent< Heal >();
            script.collider_compo = ins.GetComponent< CircleCollider2D >();

            ins.transform.position = actor.transform.position;
            ins.SetActive( true );

            //magic
            repeat_time_msec += 99999999;

            return true;
        }
    }
}