
using System;
using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ADB_Control : MonoBehaviour
{

    [Header("Main Obj")]
    public App app;
    public bool is_memu=false;

    [Header("UI")]
    public Slider slider_process_length;

    [Header("Asset")]
    public Sprite sp_icon_devices;
    private IList list_command;

    private int index_comand_cur=0;
    private float timer_step=0;
    private float timer_step_waiting=2;
    private float timer_step_length=2;
    private bool is_play=false;
    private bool is_timer_waiting=false;

    public List<string> list_id_devices;
    private UnityAction act_done;
    private Carrot_Box box=null;

    public void On_Play(IList list_cmd,UnityAction act_done=null){
        if(this.list_command==null||this.list_command.Count==0){
            this.app.cr.Show_msg("ADB Control","No commands have been created yet!",Msg_Icon.Alert);
        }
        else if(this.list_id_devices==null||this.list_id_devices.Count==0){
            this.app.cr.Show_msg("No Devices","You have not plugged in a device to run, please select a device or emulator to continue this process!",Msg_Icon.Alert);
        }
        else
        {
            this.list_command=list_cmd;
            this.slider_process_length.maxValue=list_cmd.Count;
            this.slider_process_length.value=0;
            this.index_comand_cur=0;
            this.act_done=act_done;
            this.is_play=true;    
        }
    }

    void Update()
    {
        if(this.is_play){
            this.timer_step+=1f*Time.deltaTime;
            if(this.timer_step>=this.timer_step_waiting){
                this.timer_step=0;
                if(this.is_timer_waiting){
                    this.timer_step_waiting=this.timer_step_length;
                    this.is_timer_waiting=false;
                }

                if(this.index_comand_cur>=this.list_command.Count){
                    this.On_Stop();
                    act_done?.Invoke();
                }
                IDictionary data_item=(IDictionary) this.list_command[this.index_comand_cur];

                if(data_item["type"].ToString()=="mouse_click") this.On_Mouse_Click(data_item["x"].ToString(),data_item["y"].ToString());
                if(data_item["type"].ToString()=="waiting"){
                    this.timer_step_waiting=int.Parse(data_item["timer"].ToString());
                    this.is_timer_waiting=true;
                }

                if(data_item["type"].ToString()=="send_text") this.On_Send_Text(data_item["text"].ToString());
                if(data_item["type"].ToString()=="open_app") this.On_Open_App(data_item["id_app"].ToString());
                if(data_item["type"].ToString()=="close_app") this.On_Stop_App(data_item["id_app"].ToString());
                if(data_item["type"].ToString()=="swipe") this.On_Swipe(data_item["x1"].ToString(),data_item["y1"].ToString(),data_item["x2"].ToString(),data_item["y2"].ToString(),int.Parse(data_item["timer"].ToString()));

                this.slider_process_length.value=(this.index_comand_cur+1);
                this.index_comand_cur++;
                Debug.Log("Done Step");
            }
        }
    }

    public void On_Stop(){
        this.slider_process_length.value=0;
        this.is_play=false;
    }

    public bool get_status(){
        return this.is_play;
    }

    public void On_Mouse_Click(string x,string y){
        this.app.txt_status_app.text="Tap x:"+x+" , y:"+y;
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell input tap "+x+" "+y);
        }
        else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb -s "+id_device+" shell input tap "+x+" "+y);
                }
            }
        }
    }

    public void On_Send_Text(string s_text){
        this.app.txt_status_app.text="Send Text:"+s_text;
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell input text \""+s_text+"\"");
        }else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb -s "+id_device+" shell input text \""+s_text+"\"");
                }
            }
        }
    }

    public void On_Swipe(string x1,string y1,string x2,string y2,int timer_ms){
        this.app.txt_status_app.text="Swipe "+x1+","+y1+" -> "+x2+","+y2;
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell input swipe "+x1+" "+y1+" "+x2+" "+y2+" "+timer_ms);
        }
        else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb -s "+id_device+" shell input swipe "+x1+" "+y1+" "+x2+" "+y2+" "+timer_ms);
                }
            }
        }
    }

    public void On_Open_App(string id_app){
        this.app.txt_status_app.text="Open app "+id_app;
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell monkey -p "+id_app+" -v 1");
        }
        else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb shell monkey -p "+id_app+" -v 1");
                }
            }
        }
    }

    public void On_Stop_App(string packageName)
    {
        this.app.txt_status_app.text="Close app "+packageName;
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell am force-stop "+packageName);
        }
        else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb -s "+id_device+" shell am force-stop "+packageName);
                }
            }
        }
    }

    public void On_stop_all_app(){
        this.app.txt_status_app.text="Close all app!";
        if(this.is_memu){
            this.RunCommandWithMemu("adb shell am kill-all");
        }else{
            if(this.Check_devices_alive()){
                for(int i=0;i<this.list_id_devices.Count;i++){
                    string id_device=this.list_id_devices[i];
                    this.RunADBCommand("adb -s "+id_device+" shell am kill-all");
                }
            }
        }
    }

    private bool Check_devices_alive(){
        if(this.list_id_devices==null||this.list_id_devices.Count==0){
            return false;
        }else{
            return true;
        }
    }

    public void RunCommandWithMemu(string s_command,UnityAction<string> act_done=null)
    {
        string command = "/C J:\\Microvirt\\MEmu\\MEmuc.exe -i 1 " + s_command;
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        this.app.txt_status_app.text=output;
        act_done?.Invoke(output);
    }

     public void RunADBCommand(string command,UnityAction<string> Act_done=null)
    {
        System.Diagnostics.Process process = new();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        Act_done?.Invoke(output);
    }

    private void ListConnectedDevices(UnityAction<List<string>> Act_done)
    {
        if(this.is_memu){
            this.RunCommandWithMemu("adb devices",output=>{
                this.Load_list_devices(output,Act_done);
            });
        }else{
            this.RunADBCommand("adb devices",output=>{
                this.Load_list_devices(output,Act_done);
            });
        }
    }

    private void Load_list_devices(string output,UnityAction<List<string>> Act_done){
        string[] lines = output.Split('\n');
        List<string> deviceList = new();
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (!string.IsNullOrEmpty(line) && line.Contains("device"))
            {
                string[] parts = line.Split('\t');
                if (parts.Length > 0) deviceList.Add(parts[0]);
            }
        }
        Act_done?.Invoke(deviceList);
    }

    public void Btn_show_list_devices(){
        this.ListConnectedDevices(list=>{
            Carrot_Box box_devices=this.app.cr.Create_Box();
            box_devices.set_title("List Devices");
            box_devices.set_icon(this.sp_icon_devices);

            List<string> list_device=new();
            if(list.Count>=0){
                for(int i=0;i<list.Count;i++){
                    if(list[i].Trim()!="List of devices attached"){
                        Carrot_Box_Item device_item=box_devices.create_item("item_device");
                        device_item.set_title(list[i]);
                        device_item.set_tip("Device Android");
                        device_item.set_icon(this.app.cr.icon_carrot_app);
                        list_device.Add(list[i]);
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

    public void Get_list_app(){
        this.GetInstalledApps(this.list_id_devices[0],datas=>{
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

    public void GetInstalledApps(string deviceSerial = null,UnityAction<List<string>> action_done=null)
    {
        string adbCommand = deviceSerial == null 
            ? "adb shell pm list packages"
            : $"adb -s {deviceSerial} shell pm list packages";

        this.RunADBCommand(adbCommand,s_list=>{
            string[] lines = s_list.Split('\n');
            List<string> appList = new();
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.StartsWith("package:"))
                {
                    appList.Add(trimmedLine.Replace("package:", ""));
                }
            }
            action_done?.Invoke(appList);
        });
    }

    public void Set_List_Command(IList list_cmd){
        this.list_command=list_cmd;
    }
}
