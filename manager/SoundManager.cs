using game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace game
{
	/*
     * SoundManager
     * 프리팹 매니저와 비슷하게 사운드 소스를 로드하여 들고 있는 매니저
     */
    public class SoundManager
    {
        public Dictionary< SFX, AudioSource > sfxs = new Dictionary< SFX, AudioSource >();
        public AudioSource bgm;

        public void init()
        {
            if( sfxs.Count != 0 )
                return;

            GameObject bgm_org = GameManager.prefabmgr.prefabs[ "prefabs/sound/bgm/alright" ];
            GameObject bgm_ins = Object.Instantiate( bgm_org );

            bgm = bgm_ins.GetComponent< AudioSource >();
            bgm_ins.SetActive( true );


            int i = 1;
            int loop_max = GameManager.tablemgr.sound.Count;
            for( ; i < loop_max ; ++i )
            {
                string asset_address = GameManager.tablemgr.sound[ i ];
                GameObject sfx_org = GameManager.prefabmgr.prefabs[ asset_address ];
                GameObject sfx_ins = Object.Instantiate( sfx_org );

                AudioSource sfx_audio_source = sfx_ins.GetComponent< AudioSource >();
                sfx_ins.SetActive( true );

                sfxs.Add( (SFX)i, sfx_audio_source );
            }
        }

        public void clear()
        {
            int i = 0;
            List< SFX > key_list = sfxs.Keys.ToList();
            int loop_max = key_list.Count;
            for( ; i < loop_max ; ++i )
            {
                SFX key = key_list[ i ];
                sfxs.Remove( key );
            }
        }

        public void playBgm( bool is_on )
        {
            if( is_on )
                bgm.Play();
            else
                bgm.Stop();
        }

        public void playSfx( SFX sfx )
        {
            sfxs[ sfx ].Play();
        }
    }

    public enum SFX
    {
        NONE = 0,
        DAMAGE,
        SWING,
        BULLET,
        LASER,
        LEVELUP,
        BUTTONCLICK,
        GEM,
        CLEAR,
        BOSSKILL,
        FAIL,
        PCDAMAGE,
        MAX,
    }
}