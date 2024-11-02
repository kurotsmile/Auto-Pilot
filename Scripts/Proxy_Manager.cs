using Carrot;
using UnityEngine;

public class Proxy_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    public GameObject panel_btn;

    public void On_Load(){
        this.panel_btn.SetActive(false);
    }

    public void Show(){
        this.panel_btn.SetActive(true);
        this.app.cr.Show_msg("Porxy","Show List Proxy",Msg_Icon.Alert);
    }

    public void Close(){
        this.panel_btn.SetActive(false);
    }
}
