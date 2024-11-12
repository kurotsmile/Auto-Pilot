using System.Collections;
using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum Type_Show_Devices{
    select_devices,
    dev_mode,
    get_apps,
    get_id_device
}
public class Devices_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    public IList list_id_devices;
    [Header("UI")]
    public Text txt_devices;
    public GameObject obj_panel_btn;
    private UnityAction act_close=null;
    public void On_Load(UnityAction act_close){
        this.act_close=act_close;
        this.obj_panel_btn.SetActive(false);
        this.list_id_devices=(IList)Json.Deserialize("[]");
        if(PlayerPrefs.GetString("list_id_devices","")!=""){
            this.list_id_devices=(IList)Json.Deserialize(PlayerPrefs.GetString("list_id_devices"));
        }
        this.Update_Ui();
    }

    public void Show(){
        if(this.list_id_devices.Count==0) this.Show_list_devices(Type_Show_Devices.select_devices,devices=>{
            this.Load_list_for_main();
        });
        this.obj_panel_btn.SetActive(true);
        this.Load_list_for_main();
        this.Load_list_menu_right();
    }

    private void Load_list_menu_right(){
        this.app.cr.clear_contain(this.app.tr_all_item_right);
        this.app.Add_Item_Right("Control via computer all","Enable scrcpy for all devices",this.app.sp_icon_scrcpy).set_act(()=>{
            for(int i=0;i<this.list_id_devices.Count;i++){
                string id_device=this.list_id_devices[i].ToString();
                this.app.adb.RunScrcpyCMD("-s "+id_device);
            }
        });

        this.app.Add_Item_Right("Restart all","Restart all devices",this.app.sp_icon_reboot).set_act(()=>{
            this.app.adb.RunADBCommand_All_Device("reboot");
            this.app.cr.Show_msg("Restart all","Restarted all devices!",Msg_Icon.Success);
        });

        this.app.Add_Item_Right("Power off all","Turn off all devices",this.app.sp_icon_power_off).set_act(()=>{
            this.app.adb.RunADBCommand_All_Device("reboot -p");
            this.app.cr.Show_msg("Power off all","All devices are powered off!",Msg_Icon.Success);
        });
    }

    public void Load_list_for_main(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        if(this.list_id_devices.Count>0){
            for(int i=0;i<list_id_devices.Count;i++){
                var index=i;
                var id_device=this.list_id_devices[i].ToString();
                Carrot_Box_Item box_device_item=this.app.Add_item_main();
                box_device_item.set_icon(this.app.cr.icon_carrot_app);
                box_device_item.set_title(this.list_id_devices[i].ToString());
                box_device_item.set_tip("Device Android");

                Carrot_Box_Btn_Item btn_menu=box_device_item.create_item();
                btn_menu.set_icon(this.app.cr.icon_carrot_all_category);
                btn_menu.set_icon_color(Color.white);
                btn_menu.set_color(this.app.cr.color_highlight);
                btn_menu.set_act(()=>{
                    this.Show_menu(id_device);
                });

                Carrot_Box_Btn_Item btn_del=box_device_item.create_item();
                btn_del.set_icon(this.app.cr.sp_icon_del_data);
                btn_del.set_icon_color(Color.white);
                btn_del.set_color(this.app.cr.color_highlight);
                btn_del.set_act(()=>{
                    list_id_devices.RemoveAt(index);
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Load_list_for_main();
                });
            }

        }else{
            this.app.Add_none_item();
        }
    }

    public void Show_list_devices(Type_Show_Devices type=Type_Show_Devices.select_devices,UnityAction<string> act_done=null){
        this.app.adb.ListConnectedDevices(list=>{

            List<string> list_device=new();
            bool[] list_select=new bool[list.Count];
            if(list.Count>=0){
                for(int i=0;i<list.Count;i++){
                    if(list[i].Trim()!="List of devices attached"){
                        list_device.Add(list[i]);
                    }
                }
                list_select=new bool[list_device.Count];
            }
            
            if(list_device.Count==0){
                this.app.cr.Show_msg("List Devices","No devices found!",Msg_Icon.Alert);
                return;
            }else{
                for(int i=0;i<list_device.Count;i++) list_select[i]=true;
            }

            Carrot_Box box_devices=this.app.cr.Create_Box();
            box_devices.set_title("List Devices");
            box_devices.set_icon(this.app.sp_icon_devices);

            bool[] is_show_pos=new bool[list_device.Count];
            for(int i=0;i<list_device.Count;i++){
                is_show_pos[i]=false;
                if(list_device[i].Trim()!="List of devices attached"){
                    var index=i;
                    var id_device=list_device[i];
                    Carrot_Box_Item device_item=box_devices.create_item("item_device");
                    device_item.set_title(list_device[i]);
                    device_item.set_tip("Device Android");
                    device_item.set_icon(this.app.cr.icon_carrot_app);
                    if(type==Type_Show_Devices.get_apps){
                        device_item.set_act(()=>{
                            this.app.apps.Show_List_App_By_ID_Device(id_device);
                            act_done?.Invoke(id_device);
                        });
                    }

                    if(type==Type_Show_Devices.dev_mode){
                        device_item.set_act(()=>{
                            this.app.adb.RunADBCommand_One_Device(id_device,"shell settings put system pointer_location 1",logs=>{
                                this.app.adb.RunPowershellCMD("scrcpy -s "+id_device,log=>{
                                    this.app.adb.RunADBCommand_One_Device(id_device,"shell settings put system pointer_location 0");
                                });
                            });
                            act_done?.Invoke(id_device);
                        });
                    }

                    if(type==Type_Show_Devices.get_id_device){
                        device_item.set_act(()=>{
                            act_done?.Invoke(id_device);
                        });
                    }

                    Carrot_Box_Btn_Item btn_pos=device_item.create_item();
                    btn_pos.set_icon(this.app.sp_icon_postion_click);
                    btn_pos.set_icon_color(Color.white);
                    btn_pos.set_color(this.app.cr.color_highlight);
                    btn_pos.set_act(()=>{
                        if(is_show_pos[index]==false){
                            this.app.adb.RunADBCommand_One_Device(id_device,"shell settings put system pointer_location 1");
                            is_show_pos[index]=true;
                            btn_pos.set_color(Color.red);
                        }else{
                            this.app.adb.RunADBCommand_One_Device(id_device,"shell settings put system pointer_location 0");
                            is_show_pos[index]=false;
                            btn_pos.set_color(this.app.cr.color_highlight);
                        }
                    });

                    Carrot_Box_Btn_Item btn_scrcpy=device_item.create_item();
                    btn_scrcpy.set_icon(this.app.sp_icon_scrcpy);
                    btn_scrcpy.set_icon_color(Color.white);
                    btn_scrcpy.set_color(this.app.cr.color_highlight);
                    btn_scrcpy.set_act(()=>{
                        this.app.adb.RunScrcpyCMD("-s "+id_device);
                    });

                    Carrot_Box_Btn_Item btn_reboot=device_item.create_item();
                    btn_reboot.set_icon(this.app.sp_icon_reboot);
                    btn_reboot.set_icon_color(Color.white);
                    btn_reboot.set_color(this.app.cr.color_highlight);
                    btn_reboot.set_act(()=>{
                        this.app.adb.RunADBCommand_One_Device(id_device,"reboot");
                    });

                    Carrot_Box_Btn_Item btn_power_off=device_item.create_item();
                    btn_power_off.set_icon(this.app.sp_icon_power_off);
                    btn_power_off.set_icon_color(Color.white);
                    btn_power_off.set_color(this.app.cr.color_highlight);
                    btn_power_off.set_act(()=>{
                        this.app.adb.RunADBCommand_One_Device(id_device,"reboot -p");
                    });

                    if(type!=Type_Show_Devices.dev_mode){
                        Carrot_Box_Btn_Item btn_get_all_app=device_item.create_item();
                        btn_get_all_app.set_icon(this.app.sp_icon_get_all_app);
                        btn_get_all_app.set_icon_color(Color.white);
                        btn_get_all_app.set_color(this.app.cr.color_highlight);
                        btn_get_all_app.set_act(()=>{
                            this.app.adb.GetInstalledApps(id_device,datas=>{
                                this.app.adb_tasks.On_Show(this.app.adb_tasks.Fomat_col_item_list_app(datas));
                                box_devices.close();
                                this.Set_One_Device(id_device);
                            });
                        });
                    }

                    if(type==Type_Show_Devices.select_devices){
                        Carrot_Box_Btn_Item btn_sel=device_item.create_item();
                        btn_sel.set_icon(this.app.cr.icon_carrot_done);
                        btn_sel.set_icon_color(Color.white);
                        btn_sel.set_color(this.app.cr.color_highlight);
                        btn_sel.set_act(()=>{
                            if(list_select[index]){
                                list_select[index]=false;
                                btn_sel.set_icon(this.app.cr.icon_carrot_cancel);
                            }else{
                                list_select[index]=true;
                                btn_sel.set_icon(this.app.cr.icon_carrot_done);
                            }
                        });
                    }

                    this.app.adb.RunADBCommand_One_Device(id_device,"shell getprop ro.product.model",name_device=>{
                        device_item.set_tip(name_device);
                    });
                }
            }

            if(type==Type_Show_Devices.select_devices){
                Carrot_Box_Btn_Panel btn_Panel=box_devices.create_panel_btn();
                Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
                btn_done.set_bk_color(this.app.cr.color_highlight);
                btn_done.set_label("Done");
                btn_done.set_label_color(Color.white);
                btn_done.set_icon_white(this.app.cr.icon_carrot_done);
                btn_done.set_act_click(()=>{
                    for(int i=0;i<list_device.Count;i++){
                        if (!list_id_devices.Contains(list_device[i])) list_id_devices.Add(list_device[i]);
                    }
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Update_Ui();
                    box_devices.close();
                    this.app.cr.play_sound_click();
                    act_done?.Invoke(Json.Serialize(this.list_id_devices));
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
            }
        });
    }

    public bool Check_devices_alive(){
        if(this.list_id_devices==null||this.list_id_devices.Count==0){
            return false;
        }else{
            return true;
        }
    }

    private void Set_One_Device(string id_main_device){
        this.list_id_devices=(IList)Json.Deserialize("[]");
        this.list_id_devices.Add(id_main_device);
        this.Update_Ui();
    }

    private void Update_Ui(){
        this.txt_devices.text="List Devices("+this.list_id_devices.Count+")";
    }

    public void Btn_Close(){
        this.obj_panel_btn.SetActive(false);
        act_close?.Invoke();
    }

    public void Btn_show_add_devices(){
        this.Show_list_devices(Type_Show_Devices.select_devices);
    }

    public void Btn_Show_Import(){
        this.app.excel.Show_import(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Open_file(paths=>{
                    string s_path=paths[0];
                    string s_data=FileBrowserHelpers.ReadTextFromFile(s_path);
                    this.list_id_devices=(IList) Json.Deserialize(s_data);
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Load_list_for_main();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }

            if(type==TYPE_DATA_IE.data_txt){
                this.app.file.Set_filter(Carrot_File_Data.TextDocument);
                this.app.file.Open_file(paths=>{
                    string response=FileBrowserHelpers.ReadTextFromFile(paths[0]);
                    string[] devices = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                    this.list_id_devices=(IList) Json.Deserialize("[]");
                    foreach (string id_device in devices) this.list_id_devices.Add(id_device);
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Load_list_for_main();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }

            if(type==TYPE_DATA_IE.data_excel){
                this.app.file.Set_filter(Carrot_File_Data.ExelData);
                this.app.file.Open_file(paths=>{
                    string response=FileBrowserHelpers.ReadTextFromFile(paths[0]);
                    string[] devices = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                    this.list_id_devices=(IList) Json.Deserialize("[]");
                    foreach (string id_device in devices) this.list_id_devices.Add(id_device);
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Load_list_for_main();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }
        });
    }

    public void Btn_Show_export(){
        this.app.excel.Show_export(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Save_file(paths=>{
                    string s_path=paths[0];
                    FileBrowserHelpers.WriteTextToFile(s_path,Json.Serialize(this.list_id_devices));
                    this.app.excel.Show_export_success(paths[0]);
                });
            }

            if(type==TYPE_DATA_IE.data_txt){
                this.app.file.Set_filter(Carrot_File_Data.TextDocument);
                this.app.file.Save_file(paths=>{
                    string s_data="";
                    for(int i=0;i<this.list_id_devices.Count;i++){
                        s_data+=this.list_id_devices[i]+"\n";
                    }
                    FileBrowserHelpers.WriteTextToFile(paths[0],s_data);
                    this.app.excel.Show_export_success(paths[0]);
                });
            }

            if(type==TYPE_DATA_IE.data_excel){
                this.app.file.Set_filter(Carrot_File_Data.ExelData);
                this.app.file.Save_file(paths=>{
                    string s_data="";
                    for(int i=0;i<this.list_id_devices.Count;i++){
                        s_data+=this.list_id_devices[i]+"\n";
                    }
                    FileBrowserHelpers.WriteTextToFile(paths[0],s_data);
                    this.app.excel.Show_export_success(paths[0]);
                });
            }
        });
    }

    private void Show_menu(string id_device){
         Carrot_Box box_menu=this.app.cr.Create_Box();
         box_menu.set_icon(this.app.cr.icon_carrot_all_category);
    }
}
