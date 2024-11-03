using System.Collections;
using Carrot;
using UnityEngine;

public class Proxy_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    public GameObject panel_btn;
    private IList list_proxy;

    public void On_Load(){
        this.panel_btn.SetActive(false);
        if(PlayerPrefs.GetString("list_proxy","")!="")
            this.list_proxy=(IList) Json.Deserialize(PlayerPrefs.GetString("list_proxy"));
        else
            this.list_proxy=(IList) Json.Deserialize("[]");
    }

    public void Show_Add(){
        Carrot_Box box_add=this.app.cr.Create_Box();        
        box_add.set_icon(this.app.cr.icon_carrot_add);
        box_add.set_title("Add Proxy");

        Carrot_Box_Item item_ip=box_add.create_item("item_ip");
        item_ip.set_icon(this.app.sp_icon_ip);
        item_ip.set_title("Ip");
        item_ip.set_tip("Ip address");
        item_ip.set_type(Box_Item_Type.box_value_input);

        Carrot_Box_Item item_port=box_add.create_item("item_port");
        item_port.set_icon(this.app.sp_icon_proxy_port);
        item_port.set_title("Port");
        item_port.set_tip("Connection port");
        item_port.set_type(Box_Item_Type.box_value_input);

        Carrot_Box_Btn_Panel panel=box_add.create_panel_btn();
        Carrot_Button_Item btn_done=panel.create_btn("btn_done");
        btn_done.set_icon_white(this.app.cr.icon_carrot_done);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_bk_color(this.app.cr.color_highlight);
        btn_done.set_act_click(()=>{
            IDictionary data_proxy=(IDictionary)Json.Deserialize("{}");
            data_proxy["ip"]=item_ip.get_val();
            data_proxy["port"]=item_port.get_val();
            this.list_proxy.Add(data_proxy);
            PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
            this.app.cr.Show_msg("Add Proxy","Add Proxy success!",Msg_Icon.Success);
            box_add.close();
            this.Update_list_UI();
        });

        Carrot_Button_Item btn_cancel=panel.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_bk_color(this.app.cr.color_highlight);
        btn_cancel.set_act_click(()=>{
            box_add.close();
        });
    }

    private void Update_list_UI(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        for(int i=0;i<this.list_proxy.Count;i++){
            IDictionary data_vpn=(IDictionary) this.list_proxy[i];
            Carrot_Box_Item item_vpn=this.app.Add_item_main();
            item_vpn.set_icon(this.app.sp_icon_vpn);
            item_vpn.set_title(data_vpn["ip"].ToString());
            item_vpn.set_tip(data_vpn["port"].ToString());
        }
    }

    public void Show(){
        this.panel_btn.SetActive(true);
        this.Update_list_UI();
    }

    public void Close(){
        this.panel_btn.SetActive(false);
    }

    public void Btn_import_proxy(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Open_file(paths=>{

        });
    }

    public void Btn_export_proxy(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Save_file(paths=>{

        });
    }
}
