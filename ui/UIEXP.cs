using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    public class UIEXP : MonoBehaviour
    {
        public int max_exp;
        public int current_exp;

        public int level;

        public Slider expbar;
        public TextMeshProUGUI level_text;

        // Start is called before the first frame update
        void Start()
        {
            init();
        }

        public void init()
        {
            max_exp = 10;
            current_exp = 0;
            level = 1;

            addExp( 0 );
        }

        public void addExp( int exp )
        {
            current_exp += exp;
            expbar.value = ((float)current_exp) / ((float)max_exp);
        }

        public void levelup()
        {
            current_exp = current_exp - max_exp;
            level += 1;
            level_text.text = "LV. " + level;

            max_exp = GameManager.mainch.max_exp;
            expbar.value = ((float)current_exp) / ((float)max_exp);
        }
    }
}