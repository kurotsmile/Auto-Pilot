using System.Collections.Generic;
using SimpleFileBrowser;
using UnityEngine;

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
}
