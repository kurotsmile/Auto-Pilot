using System.Collections;
using Carrot;
using SimpleFileBrowser;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Networking;

public class Proxy_Manager : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    public GameObject panel_btn;
    private IList list_proxy;

    public void On_Load(){
        this.panel_btn.SetActive(false);
        if(PlayerPrefs.GetString("list_proxy","")!="")
            this.list_proxy=(IList) Json.Deserialize(PlayerPrefs.GetString("list_proxy"));
        else
            this.list_proxy=(IList) Json.Deserialize("[]");
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
            btn_check.set_act(()=>{
                if(this.CheckProxy(data_vpn)){
                    app.cr.Show_msg("Check Proxy","Proxy connection good!",Msg_Icon.Success);
                    btn_check.set_color(Color.green);
                }                    
                else{
                    app.cr.Show_msg("Check Proxy","Proxy connection failed!",Msg_Icon.Error);
                    btn_check.set_color(Color.red);
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
    }

    public void Close(){
        this.panel_btn.SetActive(false);
    }

    public void Btn_import_proxy(){
        this.app.file.Set_filter(Carrot_File_Data.JsonData);
        this.app.file.Open_file(paths=>{

        });
    }

    public void Btn_export_proxy(){
        this.app.excel.Show_export(type=>{
            if(type==TYPE_DATA_IE.data_json){
                this.app.file.Set_filter(Carrot_File_Data.JsonData);
                this.app.file.Save_file(paths=>{
                    FileBrowserHelpers.WriteTextToFile(paths[0],Json.Serialize(this.list_proxy));
                });
            }

            if(type==TYPE_DATA_IE.data_txt){
                this.app.file.Set_filter(Carrot_File_Data.TextDocument);
                this.app.file.Save_file(paths=>{
                    for(int i=0;i<this.list_proxy.Count;i++){
                        FileBrowserHelpers.WriteTextToFile(paths[0],Json.Serialize(this.list_proxy));
                    }
                });
            }
        });
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
        IList proxyList =(IList) Json.Deserialize("[]");

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
                IDictionary data_p = (IDictionary)Json.Deserialize("{}");
                data_p["ip"] = parts[0].ToString();
                data_p["port"] = parts[1].ToString();
                proxyList.Add(data_p);
            }
            this.list_proxy=proxyList;
            PlayerPrefs.SetString("list_proxy",Json.Serialize(this.list_proxy));
            this.Update_list_UI();
        }
    }

    private bool CheckProxy(IDictionary data)
    {
        using System.Net.Sockets.TcpClient client = new();
        System.IAsyncResult result = client.BeginConnect(data["ip"].ToString(),int.Parse(data["port"].ToString()), null, null);
        bool success = result.AsyncWaitHandle.WaitOne(System.TimeSpan.FromMilliseconds(2000));

        if (!success) return false;

        client.EndConnect(result);
        return true;
    }
}
