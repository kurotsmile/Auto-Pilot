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

    public Carrot_Box Show_export(UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.Show_frm_export_and_import(true,act_done);
        box.set_title("Export data");
        box.set_icon(this.app.sp_icon_export);
        return box;
    }

    public Carrot_Box Show_import(UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.Show_frm_export_and_import(false,act_done);
        box.set_title("Import data");
        box.set_icon(this.app.sp_icon_import);
        return box;
    }

    private Carrot_Box Show_frm_export_and_import(bool is_export,UnityAction<TYPE_DATA_IE> act_done){
        Carrot_Box box=this.app.cr.Create_Box();
        Carrot_Box_Item item_json=box.create_item("item_json");
        item_json.set_icon(this.app.cr.icon_carrot_database);
        if(is_export){
            item_json.set_title("Export json");
            item_json.set_tip("Export data to json file");
        }else{
            item_json.set_title("Import json");
            item_json.set_tip("Import data to json file");
        }

        item_json.set_act(()=>{
            box.close();
            act_done?.Invoke(TYPE_DATA_IE.data_json);
        });

        Carrot_Box_Item item_txt=box.create_item("item_txt");
        item_txt.set_icon(this.app.sp_icon_text_file);
        if(is_export){
            item_txt.set_title("Export Text");
            item_txt.set_tip("Export data to text file (*.txt)");
        }else{
            item_txt.set_title("Import Text");
            item_txt.set_tip("Import data to text file (*.txt)");
        }
        item_txt.set_act(()=>{
            box.close();
            act_done?.Invoke(TYPE_DATA_IE.data_txt);
        });

        Carrot_Box_Item item_excel=box.create_item("item_excel");
        item_excel.set_icon(this.app.sp_icon_excel_file);
        if(is_export){
            item_excel.set_title("Export Excel");
            item_excel.set_tip("Export data to excel file (*.csv)");
        }else{
            item_excel.set_title("Import Excel");
            item_excel.set_tip("Import data to excel file (*.csv)"); 
        }
        item_excel.set_act(()=>{
            box.close();
            act_done?.Invoke(TYPE_DATA_IE.data_excel);
        });
        return box;
    }

    public void Show_export_success(string s_path){
        this.app.cr.Show_msg("Export","Data export successful at path:\n"+s_path,Msg_Icon.Success);
    }

    public void Show_import_success(string s_path){
        this.app.cr.Show_msg("Import","Data import successful at path:\n"+s_path,Msg_Icon.Success);
    }
}
