using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHP : MonoBehaviour
{
    public int max_hp;
    public int current_hp;

    public Slider hpbar;
    public TextMeshProUGUI hp_text;

    public void decreaseHp( int decrease_hp )
    {
        current_hp -= decrease_hp;
        hp_text.text = current_hp + "/" + max_hp;

        hpbar.value = ((float)current_hp) / ((float)max_hp);
    }

    public void increaseHp( int increase_hp )
    {
        current_hp += increase_hp;
        hp_text.text = current_hp + "/" + max_hp;

        hpbar.value = ((float)current_hp) / ((float)max_hp);
    }

    public void calcHP( int value )
    {
        current_hp += value;
        if( current_hp > max_hp )
            current_hp = max_hp;

        if( current_hp < 0 )
            current_hp = 0;

        hp_text.text = current_hp + "/" + max_hp;

        hpbar.value = ((float)current_hp) / ((float)max_hp);
    }
}
