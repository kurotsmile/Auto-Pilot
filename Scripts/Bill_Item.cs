using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Bill_Item : MonoBehaviour
{
    public Text txt_name;
    public Text txt_date;
    public Text txt_tip;
    public UnityAction act_click;

    public void On_Click()
    {
        if (act_click != null) act_click();
    }

    public void Set_Act_Click(UnityAction act_click)
    {
        this.act_click = act_click;
    }
}
