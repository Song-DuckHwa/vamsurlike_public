using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace game
{
    /**
    * PCMain
    * 플레이어가 조종하는 pc 클래스
    **/
    public class PCMain : PC
    {
        public int current_exp = 0;
        public int max_exp = 0;
        public int collect_dist = 0;

        /**
        * 초기화
        **/
        public override void init()
        {
            level = 1;
            hp = 50;
            move_speed = 200;
            direction = Vector3.zero;

            collide_mask |= (int)CollideMask.NPC;
            collide_mask |= (int)CollideMask.MOB;
            collide_mask |= (int)CollideMask.BULLET;
            collide_mask |= (int)CollideMask.OBSTACLE;

            collide_type = (int)CollideMask.PC;

            attack_dir_effect.transform.localPosition = new Vector3( 60f, 0, 0 );

            skillmgr.clearSkill();

            invincible_duration_msec = 400;
            invincible_time_msec = 0;

            current_exp = 0;
        }

        /**
        * 스탯 세팅
        **/
        public void setStat( Level stat )
        {
            hp = (int)stat.hp;
            atk = (int)stat.atk;
            move_speed = (int)stat.speed;
            max_exp = (int)stat.exp;
        }

        /**
        * 레벨업 시 hp 동기화
        **/
        public void equalizingHP()
        {
            current_hp = hp;
        }
    }
}