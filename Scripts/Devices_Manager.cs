using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;

public class Devices_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    public List<string> list_id_devices;
    private Carrot_Box box=null;

    public void On_Load(){

    }

    public void Show_list_devices(){
        this.app.adb.ListConnectedDevices(list=>{
            Carrot_Box box_devices=this.app.cr.Create_Box();
            box_devices.set_title("List Devices");
            box_devices.set_icon(this.app.sp_icon_devices);

            List<string> list_device=new();
            if(list.Count>=0){
                for(int i=0;i<list.Count;i++){
                    if(list[i].Trim()!="List of devices attached"){
                        list_device.Add(list[i]);

                        Carrot_Box_Item device_item=box_devices.create_item("item_device");
                        device_item.set_title(list[i]);
                        device_item.set_tip("Device Android");
                        device_item.set_icon(this.app.cr.icon_carrot_app);
                    }
                }
            }
            Carrot_Box_Btn_Panel btn_Panel=box_devices.create_panel_btn();
            Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
            btn_done.set_bk_color(this.app.cr.color_highlight);
            btn_done.set_label("Done");
            btn_done.set_label_color(Color.white);
            btn_done.set_icon_white(this.app.cr.icon_carrot_done);
            btn_done.set_act_click(()=>{
                this.list_id_devices=list_device;
                box_devices.close();
                this.app.cr.play_sound_click();
            });

            Carrot_Button_Item btn_cancel=btn_Panel.create_btn("btn_cancel");
            btn_cancel.set_bk_color(this.app.cr.color_highlight);
            btn_cancel.set_label("Cancel");
            btn_cancel.set_label_color(Color.white);
            btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
            btn_cancel.set_act_click(()=>{
                box_devices.close();
                this.app.cr.play_sound_click();
            });
        });
    }

    public bool Check_devices_alive(){
        if(this.list_id_devices==null||this.list_id_devices.Count==0){
            return false;
        }else{
            return true;
        }
    }

    public void Get_list_app(){
        this.app.adb.GetInstalledApps(this.list_id_devices[0],datas=>{
            if(this.box!=null) this.box.close();
            this.box=this.app.cr.Create_Box();
            this.box.set_icon(this.app.cr.icon_carrot_database);
            this.box.set_title("List Application");
            for(int i=0;i<datas.Count;i++){
                Carrot_Box_Item box_item_app=this.box.create_item("item_app");
                box_item_app.set_title("App "+i);
                box_item_app.set_tip(datas[i]);
                box.set_icon(this.app.cr.icon_carrot_app);
            }
            this.app.cr.Show_msg(datas.ToString());
        });
    }
}
