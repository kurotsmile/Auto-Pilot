using System.Collections;
using System.Collections.Generic;
using System.IO;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ADB_List_task : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;
    public Image img_btn_play;
    public Text txt_btn_play;
    private IList list_task;
    private int index_cur_task=0;
    private bool is_play=false;
    private UnityAction act_close;

    public void On_Load(){
        this.list_task=(IList)Json.Deserialize("[]");
        this.panel_btn.SetActive(false);
    }

    public void Show(){
        this.On_Show();
    }

    public void On_Show(IList list_task_app=null){
        this.panel_btn.SetActive(true);
        this.Update_ui_btn_play();
        this.app.adb_editor.Update_list_ui_Method_right_menu();
        if(list_task_app==null){
            if(PlayerPrefs.GetString("s_data_task_temp","")!=""){
                this.list_task=(IList) Json.Deserialize(PlayerPrefs.GetString("s_data_task_temp"));
                this.Update_list_task_ui();
            }
        }else{
            this.list_task=list_task_app;
            this.Update_list_task_ui();
        }
    }

    public void Close_task_list(){
        this.panel_btn.SetActive(false);
        this.On_Stop();
        act_close?.Invoke();
    }

    private void Update_list_task_ui(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        for(int i=0;i<this.list_task.Count;i++){
            IList data_app=(IList) Json.Deserialize(this.list_task[i].ToString());
            var index=i;
            var id_app=data_app[0].ToString();
            string s_name_app="App "+index;
            if(data_app.Count>=2) s_name_app=data_app[1].ToString();
            Carrot_Box_Item box_item=this.app.Add_item_main();
            box_item.set_title(s_name_app);
            box_item.txt_name.color=Color.white;
            box_item.set_tip(id_app);
            box_item.set_icon_white(this.app.cr.icon_carrot_app);

            this.app.apps.Extension_btn_item_App(id_app,box_item);
            
            box_item.set_act(()=>{
                this.index_cur_task=index;
                this.app.txt_status_app.text="Select app index:"+index;
            });

            Carrot_Box_Btn_Item btn_order=box_item.create_item();
            btn_order.set_icon_color(Color.white);
            btn_order.set_color(app.cr.color_highlight);
            btn_order.set_icon(this.app.sp_icon_sort);
            btn_order.set_act(()=>{
                Carrot_Box box_sort=this.app.cr.Create_Box();
                box_sort.set_icon(this.app.sp_icon_sort);
                box_sort.set_title("Sort");

                Carrot_Box_Item item_inp_order=box_sort.create_item();
                item_inp_order.set_icon(this.app.cr.icon_carrot_write);
                item_inp_order.set_title("Order App");
                item_inp_order.set_tip("Change order app");
                item_inp_order.set_type(Box_Item_Type.box_value_slider);
                item_inp_order.slider_val.maxValue=this.list_task.Count;
                item_inp_order.slider_val.minValue=0;
                item_inp_order.slider_val.wholeNumbers=true;
                item_inp_order.set_val(index.ToString());
                item_inp_order.check_type();

                Carrot_Box_Btn_Panel btn_Panel=box_sort.create_panel_btn();
                Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
                btn_done.set_bk_color(this.app.cr.color_highlight);
                btn_done.set_label("Done");
                btn_done.set_label_color(Color.white);
                btn_done.set_icon_white(this.app.cr.icon_carrot_done);
                btn_done.set_act_click(()=>{
                    int index_sel=int.Parse(item_inp_order.get_val());
                    object temp = this.list_task[index_sel];
                    this.list_task[index_sel] = this.list_task[index];
                    this.list_task[index] = temp;
                    this.Update_list_task_ui();
                    box_sort.close();
                    this.app.cr.play_sound_click();
                });

                Carrot_Button_Item btn_cancel=btn_Panel.create_btn("btn_cancel");
                btn_cancel.set_bk_color(this.app.cr.color_highlight);
                btn_cancel.set_label("Cancel");
                btn_cancel.set_label_color(Color.white);
                btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
                btn_cancel.set_act_click(()=>{
                    box_sort.close();
                    this.app.cr.play_sound_click();
                });
            });

            Carrot_Box_Btn_Item btn_edit=box_item.create_item();
            btn_edit.set_icon_color(Color.white);
            btn_edit.set_icon(app.cr.user.icon_user_edit);
            btn_edit.set_color(app.cr.color_highlight);
            btn_edit.set_act(()=>{
                Carrot_Box box_edit_info=this.app.cr.Create_Box();
                box_edit_info.set_icon(this.app.cr.user.icon_user_edit);
                box_edit_info.set_title("Edit Info");

                Carrot_Box_Item item_inp_id_app=box_edit_info.create_item();
                item_inp_id_app.set_icon(this.app.cr.icon_carrot_write);
                item_inp_id_app.set_title("Id App");
                item_inp_id_app.set_tip("Change application package name");
                item_inp_id_app.set_type(Box_Item_Type.box_value_input);
                if(data_app.Count>=1) item_inp_id_app.set_val(data_app[0].ToString());

                Carrot_Box_Item item_inp_name=box_edit_info.create_item();
                item_inp_name.set_icon(this.app.cr.icon_carrot_write);
                item_inp_name.set_title("Name App");
                item_inp_name.set_tip("Enter application name");
                item_inp_name.set_type(Box_Item_Type.box_value_input);
                if(data_app.Count>=2) item_inp_name.set_val(data_app[1].ToString());

                Carrot_Box_Item item_inp_note=box_edit_info.create_item();
                item_inp_note.set_icon(this.app.cr.icon_carrot_write);
                item_inp_note.set_title("Note");
                item_inp_note.set_tip("Short description for this app");
                item_inp_note.set_type(Box_Item_Type.box_value_input);
                if(data_app.Count>=3) item_inp_note.set_val(data_app[2].ToString());

                Carrot_Box_Btn_Panel btn_Panel=box_edit_info.create_panel_btn();
                Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
                btn_done.set_bk_color(this.app.cr.color_highlight);
                btn_done.set_label("Done");
                btn_done.set_label_color(Color.white);
                btn_done.set_icon_white(this.app.cr.icon_carrot_done);
                btn_done.set_act_click(()=>{
                    IList data=(IList) Json.Deserialize("[]");
                    data.Add(item_inp_id_app.get_val());
                    data.Add(item_inp_name.get_val());
                    data.Add(item_inp_note.get_val());
                    data_app=data;
                    this.list_task[index]=Json.Serialize(data);
                    this.Update_data();
                    this.Update_list_task_ui();
                    box_edit_info.close();
                    this.app.cr.play_sound_click();
                });

                Carrot_Button_Item btn_cancel=btn_Panel.create_btn("btn_cancel");
                btn_cancel.set_bk_color(this.app.cr.color_highlight);
                btn_cancel.set_label("Cancel");
                btn_cancel.set_label_color(Color.white);
                btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
                btn_cancel.set_act_click(()=>{
                    box_edit_info.close();
                    this.app.cr.play_sound_click();
                });
            });

            Carrot_Box_Btn_Item btn_del=box_item.create_item();
            btn_del.set_icon_color(Color.white);
            btn_del.set_icon(app.cr.sp_icon_del_data);
            btn_del.set_color(app.cr.color_highlight);
            btn_del.set_act(()=>{
                this.list_task.RemoveAt(index);
                this.Update_list_task_ui();
            });

            if(i%2==0) box_item.GetComponent<Image>().color=this.app.color_colum_a;
        }
    }

    public void On_Play(){
        if(this.is_play){
            this.is_play=false;
            this.app.adb.On_Stop();
        }else{
            this.is_play=true;
            this.Play_task_by_index(this.index_cur_task);
        }
        this.Update_ui_btn_play();
    }

    public void On_Stop(){
        this.is_play=false;
        this.app.adb.On_Stop();
        this.Update_ui_btn_play();
    }

    private void On_Next_task(){
        this.index_cur_task++;
        if(this.index_cur_task<this.list_task.Count){
            this.Play_task_by_index(index_cur_task);
        }else{
            this.index_cur_task=0;
            this.app.cr.Show_msg("Done all Task!","List Task",Msg_Icon.Success);
            this.On_Stop();
        }
    }

    private void Play_task_by_index(int index){
        IList data_arg=(IList)Json.Deserialize(this.list_task[index].ToString());
        Debug.Log("Play task : "+data_arg[0].ToString());
        this.Update_list_ui();
        this.app.txt_status_app.text="Play task:"+index+" "+data_arg[0].ToString();
        this.app.adb.On_Play(this.app.adb_editor.Get_list_command_method_cur(),()=>{
            this.On_Next_task();
        },data_arg);
    }

    private void Update_list_ui(){
        if(this.list_task.Count>0){
            Carrot_Box_Item item_box_cur=this.app.tr_all_item.GetChild(this.index_cur_task).GetComponent<Carrot_Box_Item>();
            item_box_cur.img_icon.color=Color.yellow;
            item_box_cur.txt_name.color=Color.yellow;
        }
    }

    private void Update_ui_btn_play(){
        if(this.is_play){
            this.img_btn_play.sprite=this.app.sp_icon_stop;
            this.txt_btn_play.text="Stop";
        }else{
            this.img_btn_play.sprite=this.app.sp_icon_start;
            this.txt_btn_play.text="Start";
        }
    }

    public void Btn_export(){
        this.app.excel.Show_export(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Save_file(paths=>{
                    string filePath=paths[0];
                    FileBrowserHelpers.WriteTextToFile(filePath,Json.Serialize(this.list_task));
                    this.app.excel.Show_export_success(filePath);
                });
            }
        });
    }

    public void Btn_import(){
        this.app.excel.Show_import(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Open_file(paths=>{
                    this.index_cur_task=0;
                    this.app.cr.clear_contain(this.app.tr_all_item);
                    string fileContent = FileBrowserHelpers.ReadTextFromFile(paths[0]);
                    this.list_task=(IList) Json.Deserialize(fileContent);
                    PlayerPrefs.SetString("s_data_task_temp",Json.Serialize(this.list_task));
                    this.Update_list_task_ui();
                    this.app.excel.Show_import_success(paths[0]);
                });
            }
        });
    }

    public void Set_Act_Close(UnityAction act){
        this.act_close=act;
    }

    public void Show_List_App(){
        if(this.app.devices_manager.list_id_devices.Count==0){
            this.app.cr.Show_msg("List Devices","No devices found!",Msg_Icon.Alert);
            return;
        }

        if(this.app.devices_manager.list_id_devices.Count==1){
            this.app.adb.GetInstalledApps(this.app.devices_manager.list_id_devices[0].ToString(),apps=>{
                this.list_task=this.Fomat_col_item_list_app(apps);
                this.Update_list_task_ui();
            });
        }else{
            this.app.devices_manager.Show_list_devices(Type_Show_Devices.get_apps);
        }
    }

    public IList Fomat_col_item_list_app(List<string> apps){
        IList list_app=(IList) Json.Deserialize("[]");
        for(int i=0;i<apps.Count;i++){
            IList list_col=(IList) Json.Deserialize("[]");
            list_col.Add(apps[i]);
            list_app.Add(Json.Serialize(list_col));
        }
        return list_app;
    }

    public int get_count_list_task(){
        return list_task.Count;
    }

    private void Update_data(){
        PlayerPrefs.SetString("s_data_task_temp",Json.Serialize(this.list_task));
    }
}
