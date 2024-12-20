﻿using Carrot;
using UnityEngine;
using UnityEngine.UI;
public class App : MonoBehaviour
{
    [Header("Main Object")]
    public Color32 color_nomal;
    public Color32 color_sel;
    public Color32 color_colum_a;
    public Color32 color_colum_b;
    public ADB_Control adb;
    public ADB_Editor adb_editor;
    public ADB_List_task adb_tasks;
    public Carrot.Carrot cr;
    public Carrot_File file;
    public GameObject item_box_prefab;
    public App_Python_Chrome_Driver apcd;
    public Devices_Manager devices_manager;
    public App_Manager apps;
    public Excel_Data excel;
    public Proxy_Manager proxys;

    [Header("UI")]
    public Transform tr_all_item;
    public Transform tr_all_item_right;
    public Text txt_status_app;
    public Text txt_btn_memu;
    public Text txt_btn_mode;
    public Image img_icon_btn_memu;
    public Image img_icon_mode;


    [Header("Asset")]
    public Sprite sp_icon_start;
    public Sprite sp_icon_stop;
    public Sprite sp_icon_auto_web;
    public Sprite sp_icon_auto_app;
    public Sprite sp_icon_excel_file;
    public Sprite sp_icon_text_file;
    public Sprite sp_icon_devices;
    public Sprite sp_icon_app_setting;
    public Sprite sp_icon_app_user;
    public Sprite sp_icon_app_system;
    public Sprite sp_icon_get_all_app;
    public Sprite sp_icon_scrcpy;
    public Sprite sp_icon_open;
    public Sprite sp_icon_start_app;
    public Sprite sp_icon_postion_click;
    public Sprite sp_icon_postion_click_off;
    public Sprite sp_icon_reboot;
    public Sprite sp_icon_power_off;
    public Sprite sp_icon_ip;
    public Sprite sp_icon_proxy_port;
    public Sprite sp_icon_vpn;
    public Sprite sp_icon_api;
    public Sprite sp_icon_checked;
    public Sprite sp_icon_checked_all;
    public Sprite sp_icon_export;
    public Sprite sp_icon_import;
    public Sprite sp_icon_script;
    public Sprite sp_icon_sad;
    public Sprite sp_icon_emulator;
    public Sprite sp_icon_sort;
    private bool is_play_simulador=false;
    private bool is_mode_web=true;

    private string  path_scrcpy="";
    private string path_adb="";
    private string path_memu="";
    void Start()
    {
        this.cr.Load_Carrot();
        this.adb_tasks.On_Load();
        this.adb_editor.On_Load();
        this.adb_editor.Load_Method_Menu_Right();

        if(PlayerPrefs.GetInt("is_mode_web",1)==1)
            this.is_mode_web=true;
        else
            this.is_mode_web=false;
        this.Check_status_mode();
        
        this.adb_editor.Set_Act_close(Load_menu_main);
        this.adb_tasks.Set_Act_Close(Load_menu_main);
        this.devices_manager.On_Load(Load_menu_main);
        this.proxys.On_Load(Load_menu_main);

        if(PlayerPrefs.GetString("path_scrcpy","")!="") this.path_scrcpy=PlayerPrefs.GetString("path_scrcpy");
        if(PlayerPrefs.GetString("path_adb","")!="") this.path_adb=PlayerPrefs.GetString("path_adb");
        if(PlayerPrefs.GetString("path_memu","")!="") this.path_memu=PlayerPrefs.GetString("path_memu");

        this.Load_menu_main();
    }

    public void Quit_App()
    {
        this.cr.play_sound_click();
        this.cr.delay_function(2f,()=>{
            Application.Quit();
        });
    }

    public void Btn_start_or_stop_Memu(){
        this.cr.play_sound_click();
        if(this.is_play_simulador){
            this.txt_btn_memu.text="Play";
            this.img_icon_btn_memu.sprite=this.sp_icon_start;
            this.adb.RunCommandWithMemu("stop");
            this.adb.is_memu=false;
            this.is_play_simulador=false;
        }else{
            this.adb.is_memu=true;
            this.txt_btn_memu.text="Stop";
            this.img_icon_btn_memu.sprite=this.sp_icon_stop;
            this.adb.RunCommandWithMemu("start");
            this.is_play_simulador=true;
        }
    }

    public void Btn_save_data(){
        this.file.Set_filter(Carrot_File_Data.JsonData);
        this.adb_editor.Save_data_json_control();
        this.cr.play_sound_click();
    }

    public Carrot_Box_Item Add_item_main(){
        GameObject obj_item=Instantiate(this.item_box_prefab);
        obj_item.transform.SetParent(this.tr_all_item);
        obj_item.transform.localScale=new Vector3(1f,1f,1f);
        obj_item.transform.localPosition=new Vector3(1f,1f,1f);
        Carrot_Box_Item box_item=obj_item.GetComponent<Carrot_Box_Item>();
        obj_item.GetComponent<Image>().color=this.color_colum_b;
        box_item.img_icon.color=Color.white;
        box_item.txt_name.color=Color.white;
        box_item.check_type();
        return box_item;
    }

    public Carrot_Box_Item Add_Item_Right(string s_title,string s_tip,Sprite s_icon,Transform tr_father=null){
        if(tr_father==null) tr_father=this.tr_all_item_right;
        GameObject obj_item=Instantiate(this.item_box_prefab);
        obj_item.transform.SetParent(tr_father);
        obj_item.transform.localPosition=new Vector3(1f,1f,1f);
        obj_item.transform.localScale=new Vector3(1f,1f,1f);

        Carrot_Box_Item item_box=obj_item.GetComponent<Carrot_Box_Item>();
        item_box.set_icon_white(s_icon);
        item_box.set_title(s_title);
        item_box.set_tip(s_tip);
        item_box.txt_name.color=Color.white;
        item_box.GetComponent<Image>().color=this.color_colum_a;
        item_box.check_type();
        return item_box;
    }

    public void Btn_show_setting(){
        Carrot_Box box_setting=this.cr.Create_Setting();

        Carrot_Box_Item item_path_memu=box_setting.create_item_of_top("item_path_memu");
        item_path_memu.set_icon(this.sp_icon_emulator);
        item_path_memu.set_type(Box_Item_Type.box_value_input);
        item_path_memu.set_title("Memu Path");
        item_path_memu.set_tip("Change the path to Memu emulator");
        item_path_memu.check_type();
        item_path_memu.set_val(this.path_memu);
        Create_btn_Open(item_path_memu);

        Carrot_Box_Item item_path_scrcpy=box_setting.create_item_of_top("item_path_scrcpy");
        item_path_scrcpy.set_icon(this.sp_icon_scrcpy);
        item_path_scrcpy.set_type(Box_Item_Type.box_value_input);
        item_path_scrcpy.set_title("Scrcpy Path");
        item_path_scrcpy.set_tip("Change Scrcpy path support android platform control by UI");
        item_path_scrcpy.check_type();
        item_path_scrcpy.set_val(this.path_scrcpy);
        Create_btn_Open(item_path_scrcpy);

        Carrot_Box_Btn_Item btn_download_scrcpy=item_path_scrcpy.create_item();
        btn_download_scrcpy.set_icon(this.cr.icon_carrot_download);
        btn_download_scrcpy.set_icon_color(Color.white);
        btn_download_scrcpy.set_color(this.cr.color_highlight);
        btn_download_scrcpy.set_act(()=>{
            Application.OpenURL("https://github.com/Genymobile/scrcpy/releases");
            cr.play_sound_click();
        });

        Carrot_Box_Item item_path_adb=box_setting.create_item_of_top("item_path_adb");
        item_path_adb.set_icon(this.adb_editor.sp_icon_adb_cmd);
        item_path_adb.set_type(Box_Item_Type.box_value_input);
        item_path_adb.set_title("ADB Path");
        item_path_adb.set_tip("Change adb android path support android platform control by command");
        item_path_adb.check_type();
        item_path_adb.set_val(this.path_adb);
        Create_btn_Open(item_path_adb);

        box_setting.set_act_before_closing(()=>{
            this.path_scrcpy=item_path_scrcpy.get_val();
            this.path_adb=item_path_adb.get_val();
            this.path_memu=item_path_memu.get_val();
            PlayerPrefs.SetString("path_scrcpy",item_path_scrcpy.get_val());
            PlayerPrefs.SetString("path_adb",item_path_adb.get_val());
            PlayerPrefs.SetString("path_menu",item_path_memu.get_val());
        });
    }

    private void Create_btn_Open(Carrot_Box_Item item_m){
        Carrot_Box_Btn_Item btn_open=item_m.create_item();
        btn_open.set_icon_color(Color.white);
        btn_open.set_icon(this.sp_icon_open);
        btn_open.set_color(this.cr.color_highlight);
        btn_open.set_act(()=>{
            this.file.Open_folders(parths=>{
                string parth=parths[0];
                item_m.set_val(parth);
            });
        });
    }

    public void Btn_change_mode(){
        if(this.is_mode_web){
            this.is_mode_web=false;
            PlayerPrefs.SetInt("is_mode_web",0);
        }else{
            this.is_mode_web=true;
            PlayerPrefs.SetInt("is_mode_web",1);
        }
        this.Check_status_mode();
    }

    private void Check_status_mode(){
        if(this.is_mode_web){
            this.img_icon_mode.sprite=this.sp_icon_auto_web;
            this.txt_btn_mode.text="Web";
        }
        else{
            this.img_icon_mode.sprite=this.sp_icon_auto_app;
            this.txt_btn_mode.text="App";
        }
    }

    private void Load_menu_main(){
        this.cr.clear_contain(this.tr_all_item);
        Carrot_Box_Item item_devices=this.Add_item_main();
        item_devices.set_title("Devices");
        item_devices.set_tip("Devices connected and selected for operation: "+this.devices_manager.list_id_devices.Count);
        item_devices.set_icon_white(this.sp_icon_devices);
        item_devices.set_act(()=>{
            this.devices_manager.Show();
        });

        Carrot_Box_Item item_apps=this.Add_item_main();
        item_apps.set_title("Apps");
        item_apps.set_tip("Total number of applications under management: "+this.adb_tasks.get_count_list_task());
        item_apps.set_icon_white(this.sp_icon_get_all_app);
        item_apps.set_act(()=>{
            this.adb_tasks.Show();
        });

        Carrot_Box_Item item_script=this.Add_item_main();
        item_script.set_title("Automated script");
        item_script.set_tip("Total number of automated scripts: "+this.adb_editor.get_length_method());
        item_script.set_icon_white(this.sp_icon_script);
        item_apps.set_act(()=>{
            this.adb_editor.Show();
        });

        Carrot_Box_Item item_proxys=this.Add_item_main();
        item_proxys.set_title("Proxy");
        item_proxys.set_tip("Total number of Proxy under management: "+this.proxys.get_length_proxy());
        item_proxys.set_icon_white(this.sp_icon_proxy_port);
        item_proxys.set_act(()=>{
            this.proxys.Show();
        });

        this.cr.clear_contain(this.tr_all_item_right);
        Carrot_Box_Item item_mode_app=this.Add_Item_Right("Mode App","App functions",this.sp_icon_auto_app);
        item_mode_app.set_act(this.Btn_model_app);
        Carrot_Box_Item item_mode_web=this.Add_Item_Right("Mode Web","Web functions",this.sp_icon_auto_web);
        item_mode_web.set_act(this.Btn_model_web);
    }

    public bool Get_Mode(){
        return this.is_mode_web;
    }

    public string get_path_scrcpy(){
        return this.path_scrcpy;
    }

    public string get_path_memu(){
        return this.path_memu;
    }

    public void Add_none_item(){
        Carrot_Box_Item none_item=this.Add_item_main();
        none_item.set_icon(this.sp_icon_sad);
        none_item.set_title("Empty list");
        none_item.set_tip("There are no items saved in this list yet.");
    }

    public void Btn_model_app(){
        this.is_mode_web=false;
        this.cr.play_sound_click();
        this.Check_status_mode();
    }

    public void Btn_model_web(){
        this.is_mode_web=true;
        this.cr.play_sound_click();
        this.Check_status_mode();
    }

}
