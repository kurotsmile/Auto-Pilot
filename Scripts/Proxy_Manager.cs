using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Proxy_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("UI")]
    public GameObject panel_btn;
    public Color32 color_connection_succes;
    public Color32 color_connection_fail;
    private IList list_proxy;
    private UnityAction act_close;

    public void On_Load(UnityAction act_close=null){
        this.panel_btn.SetActive(false);
        if(PlayerPrefs.GetString("list_proxy","")!="")
            this.list_proxy=(IList) Json.Deserialize(PlayerPrefs.GetString("list_proxy"));
        else
            this.list_proxy=(IList) Json.Deserialize("[]");
        this.act_close=act_close;
    }

    private void Load_Menu_Right(){
        this.app.cr.clear_contain(this.app.tr_all_item_right);
        Carrot_Box_Item item_check_all=this.app.Add_Item_Right("Check All","Check All Proxies",this.app.sp_icon_checked_all);
        item_check_all.set_act(async ()=>{
                List<Task<bool>> tasks = new();
                foreach (IDictionary proxy in this.list_proxy)
                {
                    tasks.Add(CheckProxyAsync(proxy));
                }
                bool[] results = await Task.WhenAll(tasks);
                
                for (int i = 0; i < list_proxy.Count; i++)
                {
                    IDictionary data_p=(IDictionary) list_proxy[i];
                    if(results[i]){
                        this.app.tr_all_item.GetChild(i).GetComponent<Image>().color=this.color_connection_succes;
                        data_p["status"]="good";
                    }
                    else{
                        this.app.tr_all_item.GetChild(i).GetComponent<Image>().color=this.color_connection_fail;
                        data_p["status"]="failed";
                    }
                }
        });

        Carrot_Box_Item item_clear_all=this.app.Add_Item_Right("Clear All","Clear All Proxies",this.app.adb_editor.sp_icon_clear_data);
        item_clear_all.set_act(()=>{
            this.app.cr.Show_msg("Clear All","Are you sure you want to delete all proxies?",()=>{
                PlayerPrefs.DeleteKey("list_proxy");
                this.list_proxy=(IList)Json.Deserialize("[]");
                this.Update_list_UI();
            });
        });

        Carrot_Box_Item item_clear_fail=this.app.Add_Item_Right("Remove proxy not connecting","Remove proxy not connecting in list",this.app.cr.sp_icon_del_data);
        item_clear_fail.set_act(()=>{
            IList list_p=(IList) Json.Deserialize("[]");
            for(int i=0;i<this.list_proxy.Count;i++){
                IDictionary data_p=(IDictionary) this.list_proxy[i];
                if(data_p["status"]!=null){
                    if(data_p["status"].ToString()=="good"){
                        list_p.Add(data_p);
                    }
                }
            }

            if(list_p.Count>0){
                this.list_proxy=list_p;
                PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
                this.Update_list_UI();
            }

            app.cr.Show_msg("Remove proxy not connectin","Remove proxy not connectin success!",Msg_Icon.Success);
        });
    }

    public void Show_Add(){
        Carrot_Box box_add=this.app.cr.Create_Box();        
        box_add.set_icon(this.app.cr.icon_carrot_add);
        box_add.set_title("Add Proxy");

        Carrot_Box_Item item_ip=box_add.create_item("item_ip");
        item_ip.set_icon(this.app.sp_icon_ip);
        item_ip.set_title("Ip");
        item_ip.set_tip("Ip address");
        item_ip.set_type(Box_Item_Type.box_value_input);

        Carrot_Box_Item item_port=box_add.create_item("item_port");
        item_port.set_icon(this.app.sp_icon_proxy_port);
        item_port.set_title("Port");
        item_port.set_tip("Connection port");
        item_port.set_type(Box_Item_Type.box_value_input);

        Carrot_Box_Btn_Panel panel=box_add.create_panel_btn();
        Carrot_Button_Item btn_done=panel.create_btn("btn_done");
        btn_done.set_icon_white(this.app.cr.icon_carrot_done);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_bk_color(this.app.cr.color_highlight);
        btn_done.set_act_click(()=>{
            IDictionary data_proxy=(IDictionary)Json.Deserialize("{}");
            data_proxy["ip"]=item_ip.get_val();
            data_proxy["port"]=item_port.get_val();
            this.list_proxy.Add(data_proxy);
            PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
            this.app.cr.Show_msg("Add Proxy","Add Proxy success!",Msg_Icon.Success);
            box_add.close();
            this.Update_list_UI();
        });

        Carrot_Button_Item btn_cancel=panel.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_bk_color(this.app.cr.color_highlight);
        btn_cancel.set_act_click(()=>{
            box_add.close();
        });
    }

    private void Update_list_UI(){
        this.app.cr.clear_contain(this.app.tr_all_item);
        for(int i=0;i<this.list_proxy.Count;i++){
            var index=i;
            IDictionary data_vpn=(IDictionary) this.list_proxy[i];
            Carrot_Box_Item item_vpn=this.app.Add_item_main();
            item_vpn.set_icon(this.app.sp_icon_vpn);
            item_vpn.set_title(data_vpn["ip"].ToString());
            item_vpn.set_tip(data_vpn["port"].ToString());

            Carrot_Box_Btn_Item btn_check=item_vpn.create_item();
            btn_check.set_icon_color(Color.white);
            btn_check.set_icon(app.sp_icon_checked);
            btn_check.set_color(app.cr.color_highlight);
            btn_check.set_act(async ()=>{
                bool isWorking = await CheckProxyAsync(data_vpn);
                if(isWorking){
                    app.cr.Show_msg("Check Proxy","Proxy connection good!",Msg_Icon.Success);
                    btn_check.set_color(Color.green);
                    data_vpn["status"]="good";
                }                    
                else{
                    app.cr.Show_msg("Check Proxy","Proxy connection failed!",Msg_Icon.Error);
                    btn_check.set_color(Color.red);
                    data_vpn["status"]="failed";
                }
            });

            Carrot_Box_Btn_Item btn_del=item_vpn.create_item();
            btn_del.set_icon_color(Color.white);
            btn_del.set_icon(app.cr.sp_icon_del_data);
            btn_del.set_color(app.cr.color_highlight);
            btn_del.set_act(()=>{
                this.list_proxy.RemoveAt(index);
                this.Update_list_UI();
            });
        }
    }

    public void Show(){
        this.panel_btn.SetActive(true);
        this.Update_list_UI();
        this.Load_Menu_Right();
    }

    public void Close(){
        this.panel_btn.SetActive(false);
        this.act_close?.Invoke();
    }

    public void Btn_import_proxy(){
        this.app.excel.Show_import(type=>{

            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Open_file(paths=>{
                    this.list_proxy=(IList) Json.Deserialize(FileBrowserHelpers.ReadTextFromFile(paths[0]));
                    PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
                    this.Update_list_UI();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }

            if(type==TYPE_DATA_IE.data_txt){
                this.app.file.Set_filter(Carrot_File_Data.TextDocument);
                this.app.file.Open_file(paths=>{
                    string response=FileBrowserHelpers.ReadTextFromFile(paths[0]);
                    string[] proxies = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                    this.list_proxy=(IList) Json.Deserialize("[]");
                    foreach (string proxy in proxies)
                    {
                        string[] parts = proxy.Split(':');
                        IDictionary data_p = (IDictionary)Json.Deserialize("{}");
                        data_p["ip"] = parts[0].ToString();
                        data_p["port"] = parts[1].ToString();
                        this.list_proxy.Add(data_p);
                    }
                    PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
                    this.Update_list_UI();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }

            if(type==TYPE_DATA_IE.data_excel){
                this.app.file.Set_filter(Carrot_File_Data.ExelData);
                this.app.file.Open_file(paths=>{
                    string response=FileBrowserHelpers.ReadTextFromFile(paths[0]);
                    string[] proxies = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                    this.list_proxy=(IList) Json.Deserialize("[]");
                    foreach (string proxy in proxies)
                    {
                        string[] parts = proxy.Split(',');
                        IDictionary data_p = (IDictionary)Json.Deserialize("{}");
                        data_p["ip"] = parts[0].ToString();
                        data_p["port"] = parts[1].ToString();
                        this.list_proxy.Add(data_p);
                    }
                    PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
                    this.Update_list_UI();
                    this.app.cr.Show_msg("Import","Data import successful!",Msg_Icon.Success);
                });
            }
        });
    }

    public void Btn_export_proxy(){
        this.app.excel.Show_export(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Save_file(paths=>{
                    FileBrowserHelpers.WriteTextToFile(paths[0],Json.Serialize(this.list_proxy));
                    this.Show_export_success(paths[0]);
                });
            }

            if(type==TYPE_DATA_IE.data_txt){
                this.app.file.Set_filter(Carrot_File_Data.TextDocument);
                this.app.file.Save_file(paths=>{
                    string s_data="";
                    for(int i=0;i<this.list_proxy.Count;i++){
                        IDictionary data_p=(IDictionary)this.list_proxy[i]; 
                        s_data+=""+data_p["ip"].ToString()+":"+data_p["port"].ToString()+"\n";
                    }
                    FileBrowserHelpers.WriteTextToFile(paths[0],s_data);
                    this.Show_export_success(paths[0]);
                });
            }

            if(type==TYPE_DATA_IE.data_excel){
                this.app.file.Set_filter(Carrot_File_Data.ExelData);
                this.app.file.Save_file(paths=>{
                    string s_data="";
                    for(int i=0;i<this.list_proxy.Count;i++){
                        IDictionary data_p=(IDictionary)this.list_proxy[i]; 
                        s_data+=""+data_p["ip"].ToString()+","+data_p["port"].ToString()+"\n";
                    }
                    FileBrowserHelpers.WriteTextToFile(paths[0],s_data);
                    this.Show_export_success(paths[0]);
                });
            }
        });
    }

    private void Show_export_success(string s_path){
        this.app.cr.Show_msg("Export","Data export successful at path:\n"+s_path,Msg_Icon.Success);
    }

    public void Btn_get_api_proxy(){
        Carrot_Box box_api=this.app.cr.Create_Box();
        box_api.set_title("Get Api");
        box_api.set_icon(this.app.sp_icon_api);

        Carrot_Box_Item item_url=box_api.create_item("item_url_inp");
        item_url.set_icon(this.app.cr.icon_carrot_write);
        item_url.set_title("Url");
        item_url.set_tip("Enter url api");
        item_url.set_type(Box_Item_Type.box_value_input);

        Carrot_Box_Btn_Panel panel=box_api.create_panel_btn();
        Carrot_Button_Item btn_done=panel.create_btn("btn_done");
        btn_done.set_icon_white(this.app.cr.icon_carrot_done);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_bk_color(this.app.cr.color_highlight);
        btn_done.set_act_click(()=>{
            string url_api=item_url.get_val();
            StartCoroutine(GetProxyList(url_api));
            this.app.cr.Show_msg("Add Proxy","Add Proxy success!",Msg_Icon.Success);
            box_api.close();
        });

        Carrot_Button_Item btn_cancel=panel.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.cr.icon_carrot_cancel);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_bk_color(this.app.cr.color_highlight);
        btn_cancel.set_act_click(()=>{
            box_api.close();
        });
    }

    private IEnumerator GetProxyList(string url)
    {
        IList proxyList = (IList)Json.Deserialize("[]");

        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Lỗi khi tải proxy: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            string[] proxies = response.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string proxy in proxies)
            {
                string[] parts = proxy.Split(':');
                if (parts.Length == 2)
                {
                    IDictionary data_p = (IDictionary)Json.Deserialize("{}");
                    data_p["ip"] = parts[0];
                    data_p["port"] = parts[1];
                    proxyList.Add(data_p);
                }
                yield return null;
            }

            this.list_proxy = proxyList;
            PlayerPrefs.SetString("list_proxy", Json.Serialize(this.list_proxy));
            this.Update_list_UI();
        }
    }

    private async Task<bool> CheckProxyAsync(IDictionary data, int timeout = 2000)
    {
        using System.Net.Sockets.TcpClient client = new();
        var connectTask = client.ConnectAsync(data["ip"].ToString(), int.Parse(data["port"].ToString()));
        var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeout));

        if (completedTask == connectTask && client.Connected)
        {
            return true;
        }
        
        return false;
    }

    public int get_length_proxy(){
        return this.list_proxy.Count;
    }
}
