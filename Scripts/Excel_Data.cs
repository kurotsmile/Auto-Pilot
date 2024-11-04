using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Events;

public enum TYPE_DATA_IE{
    data_txt,
    data_json,
    data_excel
}
public class Excel_Data : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    private List<List<string>> list_data_temp;
    public void Open_file(){
        this.app.file.Set_filter(Carrot.Carrot_File_Data.ExelData);
        this.app.file.Open_file(this.Load_file_excel);
    }

    private void Load_file_excel(string[] s_path){

    }

    public void Save_file(List<List<string>> list_data){
        this.list_data_temp=list_data;
        this.app.file.Set_filter(Carrot.Carrot_File_Data.ExelData);
        this.app.file.Save_file(this.Write_file_excel);
    }

    public void Write_file_excel(string[] s_paths){
        string s_path=s_paths[0];
        for(int i=0;i<list_data_temp.Count;i++){
            List<string> l_data_item=list_data_temp[i];
            FileBrowserHelpers.AppendTextToFile(s_path,l_data_item[0]);
        }
    }

    public Carrot_Box Show_export(UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.Show_frm_export_and_import(act_done);
        box.set_title("Export data");
        box.set_icon(this.app.sp_icon_export);
        return box;
    }

    public Carrot_Box Show_import(UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.Show_frm_export_and_import(act_done);
        box.set_title("Import data");
        box.set_icon(this.app.sp_icon_import);
        return box;
    }

    private Carrot_Box Show_frm_export_and_import(UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.app.cr.Create_Box();
        Carrot_Box_Item item_json=box.create_item("item_json");
        item_json.set_icon(this.app.cr.icon_carrot_database);
        item_json.set_title("Export json");
        item_json.set_tip("Export data to json file");
        item_json.set_act(()=>{
            act_done?.Invoke(TYPE_DATA_IE.data_json);
        });

        Carrot_Box_Item item_txt=box.create_item("item_txt");
        item_txt.set_icon(this.app.sp_icon_text_file);
        item_txt.set_title("Export Text");
        item_txt.set_tip("Export data to text file (*.txt)");
        item_txt.set_act(()=>{
            act_done?.Invoke(TYPE_DATA_IE.data_txt);
        });

        Carrot_Box_Item item_excel=box.create_item("item_excel");
        item_excel.set_icon(this.app.sp_icon_excel_file);
        item_excel.set_title("Export Excel");
        item_excel.set_tip("Export data to excel file (*.csv)");
        item_excel.set_act(()=>{
            act_done?.Invoke(TYPE_DATA_IE.data_excel);
        });
        return box;
    }
}
