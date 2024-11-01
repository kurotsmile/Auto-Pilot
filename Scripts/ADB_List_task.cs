using System.Collections;
using System.Collections.Generic;
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
    private string s_data_task_temp=null;
    private UnityAction act_close;

    public void On_Load(){
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
                this.s_data_task_temp= PlayerPrefs.GetString("s_data_task_temp");
                this.list_task=(IList) Json.Deserialize(this.s_data_task_temp);
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

    public void Open_file_tastk_app(){
        this.app.cr.play_sound_click();
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Open_file(paths=>{
            this.index_cur_task=0;
            this.app.cr.clear_contain(this.app.tr_all_item);
            string s_path=paths[0];
            string fileContent = FileBrowserHelpers.ReadTextFromFile(s_path);
            this.list_task=(IList) Json.Deserialize(fileContent);
            PlayerPrefs.SetString("s_data_task_temp",fileContent);
            this.Update_list_task_ui();
        });
    }


    private void Update_list_task_ui(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        for(int i=0;i<this.list_task.Count;i++){
            IList data_app=(IList) Json.Deserialize(this.list_task[i].ToString());
            var index=i;
            var id_app=data_app[0].ToString();
            Carrot_Box_Item box_item=this.app.Add_item_main();
            box_item.set_title("App "+i);
            box_item.txt_name.color=Color.white;
            box_item.set_tip(id_app);
            box_item.set_icon_white(this.app.cr.icon_carrot_app);
            box_item.set_act(()=>{
                this.index_cur_task=index;
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

    public void Save_File_List_App(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Save_file(pasths=>{
            string s_path=pasths[0];
            this.SaveListToFile(s_path);
        });
    }

    private void SaveListToFile(string filePath)
    {
        FileBrowserHelpers.WriteTextToFile(filePath,Json.Serialize(this.list_task));
        this.app.cr.Show_msg("Save Excel","Save File Excel Success!\nAt:"+filePath,Msg_Icon.Success);
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
            this.app.devices_manager.Show_list_devices(false);
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
}
