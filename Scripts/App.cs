using Carrot;
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
    public Sprite  sp_icon_devices;
    public Sprite  sp_icon_app_setting;
    public Sprite  sp_icon_app_user;
    public Sprite  sp_icon_app_system;

    private bool is_play_simulador=false;
    private bool is_mode_web=true;

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
        this.Load_menu_main();
        this.adb_editor.Set_Act_close(Load_menu_main);
        this.adb_tasks.Set_Act_Close(Load_menu_main);
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
        this.cr.Create_Setting();
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
        Carrot_Box_Item item_file_excel=this.Add_item_main();
        item_file_excel.set_title("Import Excel csv");
        item_file_excel.set_tip("Import data to run automatically from excel file");
        item_file_excel.set_icon_white(this.sp_icon_excel_file);

        Carrot_Box_Item item_file_text=this.Add_item_main();
        item_file_text.set_icon_white(this.sp_icon_text_file);
        item_file_text.set_title("Import Text file");
        item_file_text.set_tip("Import data to run automatically from text file");
    }

    public bool Get_Mode(){
        return this.is_mode_web;
    }
}
