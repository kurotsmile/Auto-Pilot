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

    private IList list_command;
    private int index_comand_cur=0;
    private float timer_step=0;
    private float timer_step_waiting=2;
    private float timer_step_length=2;
    private bool is_play=false;
    private bool is_timer_waiting=false;

    private UnityAction act_done;
    private IList data_arg_temp=null;

    public void On_Play(IList list_cmd,UnityAction act_done=null,IList data_arg=null){
        if(this.list_command==null||this.list_command.Count==0){
            this.app.cr.Show_msg("ADB Control","No commands have been created yet!",Msg_Icon.Alert);
        }
        else if(this.app.devices_manager.list_id_devices==null||this.app.devices_manager.list_id_devices.Count==0){
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
            this.data_arg_temp=data_arg;
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

                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.mouse_click.ToString()) this.On_Mouse_Click(this.Arg(data_item["x"].ToString()),this.Arg(data_item["y"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.waiting.ToString()){
                    this.timer_step_waiting=int.Parse(this.Arg(data_item["timer"].ToString()));
                    this.is_timer_waiting=true;
                }

                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.send_text.ToString()) this.On_Send_Text(this.Arg(data_item["text"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.open_app.ToString()) this.On_Open_App(this.Arg(data_item["id_app"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.close_app.ToString()) this.On_Stop_App(this.Arg(data_item["id_app"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.swipe.ToString()) this.On_Swipe(this.Arg(data_item["x1"].ToString()),this.Arg(data_item["y1"].ToString()),this.Arg(data_item["x2"].ToString()),this.Arg(data_item["y2"].ToString()),this.Arg(data_item["timer"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.open_app_setting.ToString()) this.Open_Setting_App(this.Arg(data_item["id_app"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.adb_cmd.ToString()) this.RunADBCommand_All_Device(this.Arg(data_item["cmd"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.forced_stop.ToString()) this.Force_Stop_App(this.Arg(data_item["id_app"].ToString()));
                if(data_item["type"].ToString()==CONTROL_ADB_TYPE.clear_data.ToString()) this.Clear_Data_App(this.Arg(data_item["id_app"].ToString()));

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
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell input tap "+x+" "+y);
        else
            this.RunADBCommand_All_Device("shell input tap "+x+" "+y);
    }

    public void On_Send_Text(string s_text){
        this.app.txt_status_app.text="Send Text:"+s_text;
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell input text \""+s_text+"\"");
        else
            this.RunADBCommand_All_Device("shell input text \""+s_text+"\"");
    }

    public void On_Swipe(string x1,string y1,string x2,string y2,string timer_ms){
        this.app.txt_status_app.text="Swipe "+x1+","+y1+" -> "+x2+","+y2;
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell input swipe "+x1+" "+y1+" "+x2+" "+y2+" "+timer_ms);
        else
            this.RunADBCommand_All_Device("shell input swipe "+x1+" "+y1+" "+x2+" "+y2+" "+timer_ms);
    }

    public void On_Open_App(string id_app){
        this.app.txt_status_app.text="Open app "+id_app;
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell monkey -p "+id_app+" -v 1");
        else
            this.RunADBCommand_All_Device("shell monkey -p "+id_app+" -v 1");
    }

    public void On_Stop_App(string packageName)
    {
        this.app.txt_status_app.text="Close app "+packageName;
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell am force-stop "+packageName);
        else
            this.RunADBCommand_All_Device("shell am force-stop "+packageName);
    }

    public void On_stop_all_app(){
        this.app.txt_status_app.text="Close all app!";
        if(this.is_memu)
            this.RunCommandWithMemu("adb shell am kill-all");
        else
            this.RunADBCommand_All_Device("shell am kill-all");
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

    public void RunPowershellCMD(string command,UnityAction<string> Act_done=null)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = command;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        //process.WaitForExit();
        if (string.IsNullOrEmpty(error))
            Debug.Log("Output: " + output);
        else
            Debug.LogError("Error: " + error);
        Act_done?.Invoke(output);
    }

    public void RunScrcpyCMD(string command, UnityAction<string> Act_done = null)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = this.app.get_path_scrcpy()+"\\scrcpy.exe";
        process.StartInfo.Arguments =command; 

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
    
        if (string.IsNullOrEmpty(error))
            Debug.Log("Output: " + output);
        else
            Debug.LogError("Error: " + error);
        
        Act_done?.Invoke(output);
    }

    public void ListConnectedDevices(UnityAction<List<string>> Act_done)
    {
        if(this.is_memu){
            this.RunCommandWithMemu("adb devices",output=>{
                this.Load_list_devices(output,Act_done);
            });
        }else{
            this.RunPowershellCMD("adb devices",output=>{
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

    public void GetInstalledApps(string deviceSerial = null,UnityAction<List<string>> action_done=null,string arg_app_type="")
    {
        string adbCommand = deviceSerial == null 
            ? "adb shell pm list packages "+arg_app_type
            : $"adb -s {deviceSerial} shell pm list packages "+arg_app_type;

        this.RunPowershellCMD(adbCommand,s_list=>{
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

    public void Open_Setting_App(string id_app){
        this.RunADBCommand_All_Device("shell am start -a android.settings.APPLICATION_DETAILS_SETTINGS -d package:"+id_app);
    }

    public void Clear_Data_App(string id_app){
        this.RunADBCommand_All_Device("shell pm clear "+id_app);
    }

    public void Force_Stop_App(string id_app){
        this.RunADBCommand_All_Device("shell am force-stop "+id_app);
    }

    public void RunADBCommand_One_Device(string id_device,string s_command,UnityAction<string> act_done=null){
        this.RunPowershellCMD("adb -s "+id_device+" "+s_command,act_done);
    }

    public void RunADBCommand_All_Device(string s_command){
        for(int i=0;i<this.app.devices_manager.list_id_devices.Count;i++) this.RunPowershellCMD("adb -s "+this.app.devices_manager.list_id_devices[i].ToString()+" "+s_command);
    }

    private string Arg(string s_command){
        for(int i=0;i<this.data_arg_temp.Count;i++){
            s_command = s_command.Replace("[" + i + "]", this.data_arg_temp[i].ToString());
        }
        return s_command;
    }
}
