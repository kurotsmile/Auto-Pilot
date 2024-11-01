using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class App_Manager : MonoBehaviour
{
    public App app;
    private List<string> list_app=new ();
    private Carrot_Box box=null;
    private string type_app_view="";

    public void Show_Select_App_Id(UnityAction<string> act_done=null){
        if(this.app.devices_manager.list_id_devices.Count==1){
            this.Show_List_App_By_ID_Device(this.app.devices_manager.list_id_devices[0],act_done);
        }else{

        }
    }

    public void Show_List_App_By_ID_Device(string id,UnityAction<string> act_done=null){
        this.app.adb.GetInstalledApps(id,datas=>{
            if(this.box!=null) this.box.close();
            this.box=this.app.cr.Create_Box();
            this.box.set_icon(this.app.cr.icon_carrot_database);
            this.box.set_title("List Application");

            Carrot_Box_Btn_Item btn_all_type=this.box.create_btn_menu_header(this.app.cr.icon_carrot_all_category);
            btn_all_type.set_act(()=>{
                this.type_app_view="";
                Show_List_App_By_ID_Device(id,act_done);
            });
            if(this.type_app_view=="") btn_all_type.set_icon_color(this.app.cr.color_highlight);

            Carrot_Box_Btn_Item btn_user_type=this.box.create_btn_menu_header(this.app.sp_icon_app_user);
            btn_user_type.set_act(()=>{
                this.type_app_view="-3";
                Show_List_App_By_ID_Device(id,act_done);
            });
            if(this.type_app_view=="-3") btn_user_type.set_icon_color(this.app.cr.color_highlight);

            Carrot_Box_Btn_Item btn_system_type=this.box.create_btn_menu_header(this.app.sp_icon_app_system);
            btn_system_type.set_act(()=>{
                this.type_app_view="-s";
                Show_List_App_By_ID_Device(id,act_done);
            });
            if(this.type_app_view=="-s") btn_system_type.set_icon_color(this.app.cr.color_highlight);

            for(int i=0;i<datas.Count;i++){
                var s_app_id=datas[i];
                Carrot_Box_Item box_item_app=this.box.create_item("item_app");
                box_item_app.set_title("App "+i);
                box_item_app.set_tip(datas[i]);
                box_item_app.set_icon(this.app.cr.icon_carrot_app);
                box_item_app.set_act(()=>{
                    act_done?.Invoke(s_app_id);
                    this.box.close();
                });
            }
        },type_app_view);
    }

    private void Load_list_for_contain(Transform tr){
        this.app.cr.clear_contain(tr);
        for(int i=0;i<this.list_app.Count;i++){
            var index=i;
            var id_app=this.list_app[i];
            Carrot_Box_Item box_item=this.app.Add_item_main();
            box_item.set_title("App "+i);
            box_item.txt_name.color=Color.white;
            box_item.set_tip(this.list_app[i]);
            box_item.set_icon_white(this.app.cr.icon_carrot_app);
            box_item.set_act(()=>{
                this.app.txt_status_app.text="Select app index:"+index;
            });

            Carrot_Box_Btn_Item btn_app_setting=box_item.create_item();
            btn_app_setting.set_icon_color(Color.white);
            btn_app_setting.set_icon(app.sp_icon_app_setting);
            btn_app_setting.set_color(app.cr.color_highlight);
            btn_app_setting.set_act(()=>{
                this.app.adb.Open_Setting_App(id_app);
            });

            Carrot_Box_Btn_Item btn_del=box_item.create_item();
            btn_del.set_icon_color(Color.white);
            btn_del.set_icon(app.cr.sp_icon_del_data);
            btn_del.set_color(app.cr.color_highlight);
            btn_del.set_act(()=>{
                this.list_app.RemoveAt(index);
                this.Load_list_for_contain(tr);
            });

            if(i%2==0) box_item.GetComponent<Image>().color=this.app.color_colum_a;
        }
    }
}
