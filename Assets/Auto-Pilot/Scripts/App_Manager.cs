using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.Events;

public enum TYPE_LIST_APP{
    get_list,
    get_one
}
public class App_Manager : MonoBehaviour
{
    public App app;
    private Carrot_Box box=null;
    private string type_app_view="";

    public void Show_Select_App_Id(UnityAction<string> act_done=null){
        if(this.app.devices_manager.list_id_devices.Count==1){
            this.Show_List_App_By_ID_Device(this.app.devices_manager.list_id_devices[0].ToString(),TYPE_LIST_APP.get_one,act_done);
        }else{
            this.app.devices_manager.Show_list_devices(Type_Show_Devices.get_id_device,id_device=>{
                this.Show_List_App_By_ID_Device(id_device,TYPE_LIST_APP.get_one,act_done);
            });
        }
    }

    public void Show_List_App_By_ID_Device(string id_device,TYPE_LIST_APP type=TYPE_LIST_APP.get_list,UnityAction<string> act_done=null){
        this.app.adb.GetInstalledApps(id_device,datas=>{
            if(this.box!=null) this.box.close();
            this.box=this.app.cr.Create_Box();
            this.box.set_icon(this.app.cr.icon_carrot_database);
            this.box.set_title("List Application");

            Carrot_Box_Btn_Item btn_all_type=this.box.create_btn_menu_header(this.app.cr.icon_carrot_all_category);
            btn_all_type.set_act(()=>{
                this.type_app_view="";
                Show_List_App_By_ID_Device(id_device,type,act_done);
            });
            if(this.type_app_view=="") btn_all_type.set_icon_color(this.app.cr.color_highlight);

            Carrot_Box_Btn_Item btn_user_type=this.box.create_btn_menu_header(this.app.sp_icon_app_user);
            btn_user_type.set_act(()=>{
                this.type_app_view="-3";
                Show_List_App_By_ID_Device(id_device,type,act_done);
            });
            if(this.type_app_view=="-3") btn_user_type.set_icon_color(this.app.cr.color_highlight);

            Carrot_Box_Btn_Item btn_system_type=this.box.create_btn_menu_header(this.app.sp_icon_app_system);
            btn_system_type.set_act(()=>{
                this.type_app_view="-s";
                Show_List_App_By_ID_Device(id_device,type,act_done);
            });
            if(this.type_app_view=="-s") btn_system_type.set_icon_color(this.app.cr.color_highlight);

            if(type==TYPE_LIST_APP.get_list){
                Carrot_Box_Btn_Item btn_sel_all=this.box.create_btn_menu_header(this.app.cr.icon_carrot_add,false);
                btn_sel_all.set_act(()=>{
                    this.app.adb_tasks.On_Show(this.app.adb_tasks.Fomat_col_item_list_app(datas));
                    if(this.box!=null) this.box.close();
                });
            }

            for(int i=0;i<datas.Count;i++){
                IList list_data_arg=(IList) Json.Deserialize("[]");
                var s_app_id=datas[i];
                list_data_arg.Add(s_app_id);
                Carrot_Box_Item box_item_app=this.box.create_item("item_app_"+i);
                box_item_app.set_title("App "+i);
                box_item_app.set_tip(s_app_id);
                this.Extension_btn_item_App(s_app_id,box_item_app);
                box_item_app.set_act(()=>{
                    act_done?.Invoke(s_app_id);
                    this.box.close();
                });
            }
        },type_app_view);
    }

    public void Extension_btn_item_App(string id_app,Carrot_Box_Item box_Item){
        Carrot_Box_Btn_Item btn_app_setting=box_Item.create_item();
        btn_app_setting.set_icon_color(Color.white);
        btn_app_setting.set_icon(app.sp_icon_app_setting);
        btn_app_setting.set_color(app.cr.color_highlight);
        btn_app_setting.set_act(()=>{
                this.app.adb.Open_Setting_App(id_app);
        });

        Carrot_Box_Btn_Item btn_menu=box_Item.create_item();
        btn_menu.set_icon_color(Color.white);
        btn_menu.set_icon(app.cr.icon_carrot_all_category);
        btn_menu.set_color(app.cr.color_highlight);
        btn_menu.set_act(()=>{
            this.Show_Menu_App(id_app);
        });
    }

    public void Show_Menu_App(string id_app){
        if(this.box!=null) this.box.close();
        this.box=this.app.cr.Create_Box();
        this.box.set_title("Menu App");
        this.box.set_icon(this.app.cr.icon_carrot_all_category);

        Carrot_Box_Item item_start_app=this.box.create_item();
        item_start_app.set_icon(this.app.sp_icon_start_app);
        item_start_app.set_title("Open App");
        item_start_app.set_tip("Launch the application on the device");
        item_start_app.set_act(()=>{
            this.app.adb.On_Open_App(id_app);
        });

        Carrot_Box_Item item_setting=this.box.create_item();
        item_setting.set_icon(this.app.cr.sp_icon_setting);
        item_setting.set_title("Open setting");
        item_setting.set_tip("Open setting app");
        item_setting.set_act(()=>{
            this.app.adb.Open_Setting_App(id_app); 
        });

        Carrot_Box_Item item_clear_data=this.box.create_item();
        item_clear_data.set_icon(this.app.adb_editor.sp_icon_clear_data);
        item_clear_data.set_title("Clear Data");
        item_clear_data.set_tip("Clear data and settings of this app");
        item_clear_data.set_act(()=>{
            this.app.adb.Clear_Data_App(id_app);
        });

        Carrot_Box_Item item_remove_app=this.box.create_item();
        item_remove_app.set_icon(this.app.cr.sp_icon_del_data);
        item_remove_app.set_title("Remove App");
        item_remove_app.set_tip("Remove the application from the device");
        item_remove_app.set_act(()=>{
            this.app.adb.RunADBCommand_All_Device("uninstall "+id_app);
        });
    }
}
