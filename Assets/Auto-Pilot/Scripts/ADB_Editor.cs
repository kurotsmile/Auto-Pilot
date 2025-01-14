using System;
using System.Collections;
using Carrot;
using SimpleFileBrowser;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CONTROL_ADB_TYPE{
    mouse_click,
    open_app,
    close_app,
    close_all_app,
    send_text,
    waiting,
    swipe,
    open_app_setting,
    adb_cmd,
    forced_stop,
    clear_data
}

public enum CONTROL_WEB_TYPE{
    open_url,
    click_emp,
    find_emp,
    close
}

public class ADB_Editor : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;
    public Image img_icon_play;
    public Text txt_play;
    public GameObject obj_btn_update;

    [Header("Asset")]
    public Sprite sp_icon_mouse;
    public Sprite sp_icon_open_app;
    public Sprite sp_icon_close_app;
    public Sprite sp_icon_waiting;
    public Sprite sp_icon_send_text;
    public Sprite sp_icon_swipe;
    public Sprite sp_icon_stop_all;
    public Sprite sp_icon_inster_after;
    public Sprite sp_icon_adb_cmd;
    public Sprite sp_icon_forced_stop;
    public Sprite sp_icon_clear_data;

    private IList list_command;

    private Carrot_Box box=null;
    private Carrot_Window_Input box_inp=null;
    
    private int length_method=0;
    private int index_sel_method=0;
    private UnityAction act_close=null;
    private int index_edit=-1;
    
    public void On_Load(){
        this.list_command=(IList) Carrot.Json.Deserialize("[]");
        this.panel_btn.SetActive(false);
        this.length_method=PlayerPrefs.GetInt("length_method",0);
        if(PlayerPrefs.GetString("m_"+index_sel_method+"_data","")!=""){
            IList list_cmd= (IList) Carrot.Json.Deserialize(PlayerPrefs.GetString("m_"+index_sel_method+"_data"));
            this.app.adb.Set_List_Command(list_cmd);
        }else{
            TextAsset m_Click_Pi_Data = Resources.Load<TextAsset>("Pi_Click");
            TextAsset m_Focus_Stop_Data = Resources.Load<TextAsset>("Focus_Stop");
            this.Add_method("Focus Stop",m_Focus_Stop_Data.text);
            this.Add_method("PiNetWork Click",m_Click_Pi_Data.text);
        }
    }

    public void Show(){
        this.Show_Editor();
    }

    public void Show_Editor(int index_update=-1){
        this.index_edit=index_update;
        this.list_command=(IList) Carrot.Json.Deserialize("[]");
        this.app.cr.clear_contain(this.app.tr_all_item_right);
        this.app.cr.play_sound_click();
        if(this.app.Get_Mode())
            this.Load_Menu_Right_Web();
        else
            this.Load_Menu_Right_App();
        this.panel_btn.SetActive(true);
        this.Update_btn_ui();
        this.Update_list_ui();
    }

    private void Load_Menu_Right_Web(Transform tr_father=null,int index_insert=-1){

        this.app.Add_Item_Right("Open The Web","Open the web with the url",this.sp_icon_open_app,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.open_app,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.open_app);
        });

        this.app.Add_Item_Right("Add Mouse click","Add position x,y click",this.sp_icon_mouse,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.mouse_click,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.mouse_click);
        });

        this.app.Add_Item_Right("Close The Web","Close the browser",this.sp_icon_close_app,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.close_app,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.close_app);
        });
    }

    private void Load_Menu_Right_App(Transform tr_father=null,int index_insert=-1){

        this.app.Add_Item_Right("Add Mouse click","Add position x,y click",this.sp_icon_mouse,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.mouse_click,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.mouse_click);
        });

        this.app.Add_Item_Right("Open The App","Open the application with the package name",this.sp_icon_open_app,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.open_app,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.open_app);
        });

        this.app.Add_Item_Right("Close The App","Close the application with the package name",this.sp_icon_close_app,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.close_app,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.close_app);
        });

        this.app.Add_Item_Right("Waiting","waiting to continue other tasks",this.sp_icon_waiting,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.waiting,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.waiting);
        });

        this.app.Add_Item_Right("Send Text","Send Text to Device as Clipboard",this.sp_icon_send_text,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.send_text,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.send_text);
        });

        this.app.Add_Item_Right("Swipe","Slide the screen from one position to another",this.sp_icon_swipe,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.swipe,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.swipe);
        });

        this.app.Add_Item_Right("Open app setting","Open application settings by application package id",this.app.sp_icon_app_setting,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.open_app_setting,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.open_app_setting);
        });

        this.app.Add_Item_Right("ADB Command Line","Add custom ADB command line",this.sp_icon_adb_cmd,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.adb_cmd,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.adb_cmd);
        });

        this.app.Add_Item_Right("Forced stop","Force stop background apps",this.sp_icon_forced_stop,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.forced_stop,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.forced_stop);
        });

        this.app.Add_Item_Right("Clear Data App","Delete application cache data with package name",this.sp_icon_clear_data,tr_father).set_act(()=>{
            if(index_insert!=-1)
                this.Show_edit_control(index_insert,CONTROL_ADB_TYPE.clear_data,true);
            else
                this.Show_edit_control(-1,CONTROL_ADB_TYPE.clear_data);
        });

        this.app.Add_Item_Right("Stop all applications","Stop all user applications excluding system applications",this.sp_icon_stop_all,tr_father).set_act(()=>{
            this.Add_item_for_list(CONTROL_ADB_TYPE.close_all_app,"Stop all applications");
            this.Update_list_ui();
        });
    }

    public void Load_Method_Menu_Right(){
        this.Update_list_ui_Method_right_menu();
    }

    public void Update_list_ui_Method_right_menu(){
        this.app.cr.clear_contain(this.app.tr_all_item_right);
        for(int i=0;i<=this.length_method;i++){
            var index=i;
            if(PlayerPrefs.GetString("m_"+i+"_name","")=="") continue;

            string s_name=PlayerPrefs.GetString("m_"+i+"_name");
            Carrot_Box_Item item_m=this.app.Add_Item_Right(s_name,"Method",this.app.cr.icon_carrot_advanced);
            item_m.set_act(()=>{
                this.index_sel_method=index;
                IList list_cmd= (IList) Carrot.Json.Deserialize(PlayerPrefs.GetString("m_"+index+"_data"));
                this.app.adb.Set_List_Command(list_cmd);
                this.Update_list_ui_Method_right_menu();
            });

            Carrot_Box_Btn_Item btn_edit=item_m.create_item();
            btn_edit.set_icon(this.app.cr.user.icon_user_edit);
            btn_edit.set_icon_color(Color.white);
            btn_edit.set_color(this.app.cr.color_highlight);
            btn_edit.set_act(()=>{
                this.Show_Editor(index);
                this.list_command= (IList) Carrot.Json.Deserialize(PlayerPrefs.GetString("m_"+index+"_data"));
                this.Update_list_ui();
            });

            Carrot_Box_Btn_Item btn_del=item_m.create_item();
            btn_del.set_icon(this.app.cr.sp_icon_del_data);
            btn_del.set_icon_color(Color.white);
            btn_del.set_color(this.app.cr.color_highlight);
            btn_del.set_act(()=>{
                this.Delete_method(index);
            });

            if(index==this.index_sel_method){
                item_m.GetComponent<Image>().color=this.app.color_colum_a;
                item_m.img_icon.color=Color.yellow;
            }
        }
    }

    private void Delete_method(int index){
        PlayerPrefs.DeleteKey("m_"+index+"_data");
        PlayerPrefs.DeleteKey("m_"+index+"_name");
        this.Update_list_ui_Method_right_menu();
    }

    public void Save_data_json_control(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Save_file(paths=>{
            string s_path=paths[0];
            FileBrowserHelpers.WriteTextToFile(s_path,Carrot.Json.Serialize(this.list_command));
        });
    }

    public void Save_function(){
        if(this.list_command.Count==0){
            this.app.cr.Show_msg("Save method","Please create the command lines before saving the method!",Msg_Icon.Alert);
            return;
        }

        this.box_inp=this.app.cr.Show_input("Save method","Enter the method name you want to save for future use.");
        box_inp.set_act_done(val=>{
            this.Add_method(val,Json.Serialize(this.list_command));
            this.app.cr.Show_msg("Save method","Save success!",Msg_Icon.Success);
            this.box_inp.close();
        });
    }

    private void Add_method(string s_name,string s_data){
        PlayerPrefs.SetString("m_"+this.length_method+"_name",s_name);
        PlayerPrefs.SetString("m_"+this.length_method+"_data",s_data);
        this.length_method++;
        PlayerPrefs.SetInt("length_method",this.length_method);
    }

    public void Update_list_ui(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        if(this.list_command.Count==0){
            this.app.Add_none_item();
            return;
        }

        for(int i=0;i<this.list_command.Count;i++){
                var index=i;
                IDictionary control_data=(IDictionary) list_command[i];

                GameObject obj_control=Instantiate(this.app.item_box_prefab);
                obj_control.transform.SetParent(this.app.tr_all_item);
                obj_control.transform.localScale=new Vector3(1f,1f,1f);
                obj_control.transform.localPosition=new Vector3(1f,1f,1f);
                if(i%2==0)
                    obj_control.GetComponent<Image>().color=this.app.color_colum_a;
                else
                    obj_control.GetComponent<Image>().color=this.app.color_colum_b;
                Carrot_Box_Item cr_item=obj_control.GetComponent<Carrot_Box_Item>();

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.mouse_click.ToString()){
                    cr_item.set_icon_white(this.sp_icon_mouse);
                    cr_item.set_title("Mouse Click");
                    cr_item.set_tip("X:"+control_data["x"].ToString()+" , Y:"+control_data["y"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app.ToString()){
                    cr_item.set_icon_white(this.sp_icon_open_app);
                    cr_item.set_title("Open App");
                    cr_item.set_tip("App id:"+control_data["id_app"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.close_app.ToString()){
                    cr_item.set_icon_white(this.sp_icon_close_app);
                    cr_item.set_title("Close App");
                    cr_item.set_tip("App id:"+control_data["id_app"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.waiting.ToString()){
                    cr_item.set_icon_white(this.sp_icon_waiting);
                    cr_item.set_title("Waiting");
                    cr_item.set_tip("Timer:"+control_data["timer"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.send_text.ToString()){
                    cr_item.set_icon_white(this.sp_icon_send_text);
                    cr_item.set_title("Send Text");
                    cr_item.set_tip("Text:"+control_data["text"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.swipe.ToString()){
                    cr_item.set_icon_white(this.sp_icon_swipe);
                    cr_item.set_title("Swipe");
                    cr_item.set_tip("Move To :"+control_data["x1"].ToString()+","+control_data["y1"].ToString()+" -> "+control_data["x2"].ToString()+","+control_data["y2"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.close_all_app.ToString()){
                    cr_item.set_icon_white(this.sp_icon_stop_all);
                    cr_item.set_title("Stop All");
                    cr_item.set_tip(control_data["tip"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app_setting.ToString()){
                    cr_item.set_icon_white(this.app.sp_icon_app_setting);
                    cr_item.set_title("Open App setting");
                    cr_item.set_tip("App id:"+control_data["id_app"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.adb_cmd.ToString()){
                    cr_item.set_icon_white(this.sp_icon_adb_cmd);
                    cr_item.set_title("OADB Command");
                    cr_item.set_tip(control_data["cmd"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.forced_stop.ToString()){
                    cr_item.set_icon_white(this.sp_icon_forced_stop);
                    cr_item.set_title("Forced stop");
                    cr_item.set_tip(control_data["id_app"].ToString());
                }

                if(control_data["type"].ToString()==CONTROL_ADB_TYPE.clear_data.ToString()){
                    cr_item.set_icon_white(this.sp_icon_clear_data);
                    cr_item.set_title("Clear Data");
                    cr_item.set_tip(control_data["id_app"].ToString());
                }

                cr_item.check_type();
                cr_item.txt_name.color=Color.white;
                cr_item.set_act(()=>{
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.mouse_click.ToString()) this.app.adb.On_Mouse_Click(control_data["x"].ToString(),control_data["y"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.send_text.ToString()) this.app.adb.On_Send_Text(control_data["text"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app.ToString()) this.app.adb.On_Open_App(control_data["id_app"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.close_app.ToString()) this.app.adb.On_Stop_App(control_data["id_app"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.close_all_app.ToString()) this.app.adb.On_stop_all_app();
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.swipe.ToString())  this.app.adb.On_Swipe(control_data["x1"].ToString(),control_data["y1"].ToString(),control_data["x2"].ToString(),control_data["y2"].ToString(),control_data["timer"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app_setting.ToString()) this.app.adb.Open_Setting_App(control_data["id_app"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.adb_cmd.ToString()) this.app.adb.RunADBCommand_All_Device(control_data["cmd"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.forced_stop.ToString()) this.app.adb.Force_Stop_App(control_data["id_app"].ToString());
                    if(control_data["type"].ToString()==CONTROL_ADB_TYPE.clear_data.ToString()) this.app.adb.Clear_Data_App(control_data["id_app"].ToString());
                });

                Carrot_Box_Btn_Item btn_inster=cr_item.create_item();
                btn_inster.set_icon_color(Color.white);
                btn_inster.set_icon(this.sp_icon_inster_after);
                btn_inster.set_color(app.cr.color_highlight);
                btn_inster.set_act(()=>{
                    this.Show_sub_menu(index);
                });

                if(control_data["type"].ToString()!="close_all_app"){
                    Carrot.Carrot_Box_Btn_Item btn_edit=cr_item.create_item();
                    btn_edit.set_icon_color(Color.white);
                    btn_edit.set_icon(app.cr.icon_carrot_write);
                    btn_edit.set_color(app.cr.color_highlight);
                    btn_edit.set_act(()=>{
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.mouse_click.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.mouse_click);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.open_app);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.close_app.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.close_app);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.waiting.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.waiting);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.send_text.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.send_text);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.swipe.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.swipe);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.open_app_setting.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.open_app_setting);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.adb_cmd.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.adb_cmd);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.forced_stop.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.forced_stop);
                        if(control_data["type"].ToString()==CONTROL_ADB_TYPE.clear_data.ToString()) this.Show_edit_control(index,CONTROL_ADB_TYPE.clear_data);
                    });
                }

                Carrot_Box_Btn_Item btn_del=cr_item.create_item();
                btn_del.set_icon_color(Color.white);
                btn_del.set_icon(app.cr.sp_icon_del_data);
                btn_del.set_color(app.cr.color_highlight);
                btn_del.set_act(()=>{
                    this.list_command.RemoveAt(index);
                    this.Update_list_ui();
                });
        }
    }

    private Carrot_Box_Item Add_field_position(string s_title,string s_tip){
        Carrot.Carrot_Box_Item inp_number=this.box.create_item("inp_pos");
        inp_number.set_title(s_title);
        inp_number.set_tip(s_tip);
        inp_number.set_icon(this.app.cr.icon_carrot_write);
        inp_number.set_type(Carrot.Box_Item_Type.box_number_input);
        return inp_number;
    }

    private Carrot_Box_Item Add_field_id_app(){
        Carrot_Box_Item inp_app_id=this.box.create_item("inp_app_id");
        inp_app_id.set_title("Application Package Name (Application ID)");
        inp_app_id.set_tip("Enter the app bundle id name or select from the list");
        inp_app_id.set_icon(this.app.cr.icon_carrot_write);
        inp_app_id.set_type(Carrot.Box_Item_Type.box_value_input);

        Carrot_Box_Btn_Item btn_sel_app=inp_app_id.create_item();
        btn_sel_app.set_icon(app.cr.icon_carrot_app);
        btn_sel_app.set_act(()=>{
            this.app.apps.Show_Select_App_Id(id_app=>{
                inp_app_id.set_val(id_app);
            });
        });
        return inp_app_id;
    }

    private Carrot_Box_Item Add_field_tip(){
        Carrot.Carrot_Box_Item inp_note=this.box.create_item("inp_note");
        inp_note.set_title("Note");
        inp_note.set_tip("Write a short description of this action to help you remember it.");
        inp_note.set_icon(this.app.cr.icon_carrot_write);
        inp_note.set_type(Carrot.Box_Item_Type.box_value_input);
        return inp_note;
    }

    private void Show_edit_control(int index,CONTROL_ADB_TYPE type,bool is_insert=false){
        if(this.box!=null) this.box.close();

        IDictionary data_control=null;
        if(is_insert){
            data_control=(IDictionary) Json.Deserialize("{}");
        }else{
            if(index!=-1)
                data_control=(IDictionary)this.list_command[index];
            else
                data_control=(IDictionary) Json.Deserialize("{}");
        }

        data_control["type"]=type.ToString();

        this.box=this.app.cr.Create_Box("box_control_edit");
        Carrot_Box_Btn_Panel btn_Panel=null;
        if(type==CONTROL_ADB_TYPE.mouse_click){
            this.box.set_icon(this.sp_icon_mouse);
            if(index==-1)
                this.box.set_title("Add Mouse Click");
            else
                this.box.set_title("Update Mouse Click");

            this.Add_btn_dev_for_box();
            Carrot_Box_Item inp_x=this.Add_field_position("Position x","Position x mouse and tap");
            if(data_control["x"]!=null)
                inp_x.set_val(data_control["x"].ToString());
            else
                inp_x.set_val("0");

            Carrot_Box_Item inp_y=this.Add_field_position("Position y","Position y mouse and tap");
            if(data_control["y"]!=null)
                inp_y.set_val(data_control["y"].ToString());
            else
                inp_y.set_val("0");

            Carrot_Box_Item inp_tip=this.Add_field_tip();
            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["x"]=inp_x.get_val();
                data_control["y"]=inp_y.get_val();
                data_control["tip"]=inp_tip.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.open_app){
            this.box.set_icon(this.sp_icon_open_app);
            if(index==-1)
                this.box.set_title("Add Open App");
            else
                this.box.set_title("Update Open App");

            Carrot_Box_Item inp_id_app=Add_field_id_app();
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.close_app){
            this.box.set_icon(this.sp_icon_close_app);
            if(index==-1)
                this.box.set_title("Add Close App");
            else
                this.box.set_title("Update Close App");
            Carrot_Box_Item inp_id_app=Add_field_id_app();
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.waiting){
            this.box.set_icon(this.sp_icon_waiting);
            if(index==-1)
                this.box.set_title("Add Waiting");
            else
                this.box.set_title("Update Waiting");
            Carrot.Carrot_Box_Item inp_timer=this.box.create_item("timer");
            inp_timer.set_title("Waiting time");
            inp_timer.set_tip("Enter the number of seconds to wait");
            inp_timer.set_icon(this.app.cr.icon_carrot_write);
            inp_timer.set_type(Carrot.Box_Item_Type.box_number_input);
            if(data_control["timer"]!=null) inp_timer.set_val(data_control["timer"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["timer"]=inp_timer.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.send_text){
            this.box.set_icon(this.sp_icon_send_text);
            if(index==-1)
                this.box.set_title("Add Send Text");
            else
                this.box.set_title("Update Send Text");
            Carrot.Carrot_Box_Item inp_text=this.box.create_item("inp_text");
            inp_text.set_title("Text");
            inp_text.set_tip("Enter the text you want to insert into the device");
            inp_text.set_icon(this.app.cr.icon_carrot_write);
            inp_text.set_type(Carrot.Box_Item_Type.box_value_input);
            if(data_control["text"]!=null) inp_text.set_val(data_control["text"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["text"]=inp_text.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.swipe){
            this.box.set_icon(this.sp_icon_swipe);
            if(index==-1)
                this.box.set_title("Add Swipe");
            else
                this.box.set_title("Update Swipe");
            this.Add_btn_dev_for_box();
            Carrot_Box_Item inp_x1=this.Add_field_position("Position x1","Position x1 mouse and tap");
            if(data_control["x1"]!=null)
                inp_x1.set_val(data_control["x1"].ToString());
            else
                inp_x1.set_val("0");

            Carrot_Box_Item inp_y1=this.Add_field_position("Position y1","Position y1 mouse and tap");
            if(data_control["y1"]!=null)
                inp_y1.set_val(data_control["y1"].ToString());
            else
                inp_y1.set_val("0");

            Carrot_Box_Item inp_x2=this.Add_field_position("Position x2","Position x2 mouse and tap");
            if(data_control["x2"]!=null)
                inp_x2.set_val(data_control["x2"].ToString());
            else
                inp_x2.set_val("0");

            Carrot_Box_Item inp_y2=this.Add_field_position("Position y2","Position y2 mouse and tap");
            if(data_control["y2"]!=null) inp_y2.set_val(data_control["y2"].ToString()); else inp_y2.set_val("0");

            Carrot_Box_Item inp_timer_ms=this.Add_field_position("Timer ms","Time to perform the operation");
            if(data_control["timer"]!=null)
                inp_timer_ms.set_val(data_control["timer"].ToString());
            else
                inp_timer_ms.set_val("100");

            Carrot_Box_Item inp_tip=this.Add_field_tip();
            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["x1"]=inp_x1.get_val();
                data_control["y1"]=inp_y1.get_val();
                data_control["x2"]=inp_x2.get_val();
                data_control["y2"]=inp_y2.get_val();
                data_control["tip"]=inp_tip.get_val();
                data_control["timer"]=inp_timer_ms.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.open_app_setting){
            this.box.set_icon(this.app.sp_icon_app_setting);
            if(index==-1)
                this.box.set_title("Add open app setting");
            else
                this.box.set_title("Update open app setting");
            Carrot_Box_Item inp_id_app=Add_field_id_app();
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.adb_cmd){
            this.box.set_icon(this.sp_icon_adb_cmd);
            if(index==-1)
                this.box.set_title("Add ADB Command");
            else
                this.box.set_title("Update ADB Command");
            Carrot_Box_Item inp_cmd=this.box.create_item("inp_cmd");
            inp_cmd.set_title("ADB Command Line");
            inp_cmd.set_tip("Add custom ADB command line, Exclude adb keyword");
            inp_cmd.set_icon(this.app.cr.icon_carrot_write);
            inp_cmd.set_type(Box_Item_Type.box_value_input);
            if(data_control["cmd"]!=null) inp_cmd.set_val(data_control["cmd"].ToString());

            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["cmd"]=inp_cmd.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.forced_stop){
            this.box.set_icon(this.sp_icon_forced_stop);
            if(index==-1)
                this.box.set_title("Add Forced Stop App");
            else
                this.box.set_title("Update Forced Stop App");
            Carrot_Box_Item inp_id_app=this.Add_field_id_app();
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());
            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        if(type==CONTROL_ADB_TYPE.clear_data){
            this.box.set_icon(this.sp_icon_clear_data);
            if(index==-1)
                this.box.set_title("Add Clear Data App");
            else
                this.box.set_title("Update Clear Data App");
            Carrot_Box_Item inp_id_app=this.Add_field_id_app();
            if(data_control["id_app"]!=null) inp_id_app.set_val(data_control["id_app"].ToString());
            btn_Panel=Frm_editor_btn_done(()=>{
                data_control["id_app"]=inp_id_app.get_val();
                if(is_insert){
                    this.list_command.Insert(index+1,data_control);
                }else{
                    if(index!=-1)
                        this.list_command[index]=data_control;
                    else
                        this.list_command.Add(data_control);
                }
            });
        }

        Carrot_Button_Item btn_cancel=btn_Panel.create_btn("btn_cancel");
        btn_cancel.set_bk_color(this.app.cr.color_highlight);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
        btn_cancel.set_act_click(()=>{
            this.box.close();
            this.app.cr.play_sound_click();
        });
    }

    private void Add_btn_dev_for_box(){
        Carrot_Box_Btn_Item btn_dev=this.box.create_btn_menu_header(this.app.cr.sp_icon_dev);
        btn_dev.set_act(()=>{
            this.app.devices_manager.Show_list_devices(Type_Show_Devices.dev_mode);
        });
    }

    private Carrot_Box_Btn_Panel Frm_editor_btn_done(UnityAction act_click=null){
        Carrot_Box_Btn_Panel btn_Panel=this.box.create_panel_btn();
        Carrot_Button_Item btn_done=btn_Panel.create_btn("btn_done");
        btn_done.set_bk_color(this.app.cr.color_highlight);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_icon_white(this.app.cr.icon_carrot_done);
        btn_done.set_act_click(()=>{
            act_click?.Invoke();
            this.box.close();
            this.app.cr.play_sound_click();
            this.Update_list_ui();
        });
        return btn_Panel;
    }

    public void Close_Editor(){
        this.index_edit=-1;
        this.obj_btn_update.SetActive(false);
        this.panel_btn.SetActive(false);
        this.app.cr.play_sound_click();
        this.Load_Method_Menu_Right();
        act_close?.Invoke();
    }

    public void On_Open(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Open_file(paths=>{
            string s_path=paths[0];
            this.list_command= (IList) Carrot.Json.Deserialize(FileBrowserHelpers.ReadTextFromFile(s_path));
            this.Update_list_ui();
        });
    }

    public void Show_sub_menu(int index_insert){
        this.box=this.app.cr.Create_Box();
        this.box.set_icon(this.sp_icon_inster_after);
        this.box.set_title("Insert next element after this element");
        this.Load_Menu_Right_App(this.box.area_list_contain,index_insert);
    }

    public void Add_item_for_list(CONTROL_ADB_TYPE type,string s_tip="Control ADB"){
        IDictionary data_control=(IDictionary) Json.Deserialize("{}");
        data_control["type"]=type.ToString();
        data_control["tip"]=s_tip;
        this.list_command.Add(data_control);
    }

    public void Play_all_comand(){
        if(this.app.adb.get_status()){
            this.txt_play.text="Stop";
            this.img_icon_play.sprite=this.app.sp_icon_stop;
            this.app.adb.On_Stop();
        }else{
            this.txt_play.text="Play";
            this.img_icon_play.sprite=this.app.sp_icon_start;
            if(this.app.Get_Mode())
                this.app.apcd.Run(Json.Serialize(this.list_command));
            else
                this.app.adb.On_Play(this.list_command);
        }
    }

    public IList Get_list_Command(){
        return this.list_command;
    }

    public IList Get_list_command_method_cur(){
        IList list_cmd=(IList) Json.Deserialize(PlayerPrefs.GetString("m_"+this.index_sel_method+"_data"));
        return list_cmd;
    }

    public void Set_Act_close(UnityAction act){
        this.act_close=act;
    }

    private void Update_btn_ui(){
        if(this.index_edit==-1)
            this.obj_btn_update.SetActive(false);
        else
            this.obj_btn_update.SetActive(true);
    }

    public void On_update_method(){
        PlayerPrefs.SetString("m_"+this.index_edit+"_data",Json.Serialize(this.list_command));
        this.app.cr.Show_msg("Update method","Update method successful!",Msg_Icon.Success);
    }

    public int get_length_method(){
        return this.length_method;
    }
}