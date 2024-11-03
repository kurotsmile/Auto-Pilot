using System.Collections;
using System.Collections.Generic;
using Carrot;
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

    public void On_Load(){
        this.list_id_devices=(IList)Json.Deserialize("[]");
        if(PlayerPrefs.GetString("list_id_devices","")!=""){
            this.list_id_devices=(IList)Json.Deserialize(PlayerPrefs.GetString("list_id_devices"));
        }
        this.Update_Ui();
    }

    public void Load_List_by_main(){

    }

    public void Show(){
        this.Show_list_devices(Type_Show_Devices.select_devices);
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
                        this.app.adb.RunPowershellCMD("scrcpy -s "+id_device);
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
                    this.list_id_devices=new List<string>();
                    for(int i=0;i<list_device.Count;i++){
                        if(list_select[i]) this.list_id_devices.Add(list_device[i]);
                    }
                    PlayerPrefs.SetString("list_id_devices",Json.Serialize(this.list_id_devices));
                    this.Update_Ui();
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
}
