using SFB;
using SimpleFileBrowser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Carrot
{
    public enum Carrot_File_Type {SimpleFileBrowser,StandaloneFileBrowser}
    public enum Carrot_File_Data {Image,JsonData,ExelData,AudioData,PDFDocument,TextDocument}
    public class Carrot_File_Query
    {
        public List<string> s_title_file_data;
        public List<string[]> list_extension_file;
        public string s_DefaultFilter_extension_file = "";

        public Carrot_File_Query()
        {
            this.s_title_file_data= new List<string>();
            this.list_extension_file=new List<string[]>();
        }

        public void SetDefaultFilter(string s_extension_file)
        {
            this.s_DefaultFilter_extension_file = s_extension_file;
        }
        public void Add_filter(string title_type_file_data, params string[] extension_file)
        {
            s_title_file_data.Add(title_type_file_data);
            list_extension_file.Add(extension_file);
        }
    }

    public class Carrot_File : MonoBehaviour
    {
        [Header("Main")]
        public Carrot_File_Type type = Carrot_File_Type.SimpleFileBrowser;

        private FileBrowser.Filter[] SimpleFileBrowser_filter;
        private ExtensionFilter[] StandaloneFileBrowser_filter;

        public void Set_filter(Carrot_File_Data type_data)
        {
            Carrot_File_Query q = new();
            if (type_data == Carrot_File_Data.Image)
            {
                q.SetDefaultFilter("jpg");
                q.Add_filter("Images", "jpg", "png", "jpeg");
                q.Add_filter("Pain", "bmp", "tiff", "tga");
            }

            if (type_data == Carrot_File_Data.JsonData)
            {
                q.SetDefaultFilter("json");
                q.Add_filter("Json Data", "json", "jsons");
                q.Add_filter("Text Data", "txt");
            }

            if (type_data == Carrot_File_Data.ExelData)
            {
                q.SetDefaultFilter("csv");
                q.Add_filter("Exel Data", "csv","rtf","xlsx");
                q.Add_filter("Text Data", "txt");
            }

            if (type_data == Carrot_File_Data.AudioData)
            {
                q.SetDefaultFilter("mp3");
                q.Add_filter("Compressed audio", "mp3", "ogg", "s3m");
                q.Add_filter("Uncompressed audio", "wav", "aiff");
            }

            if (type_data == Carrot_File_Data.PDFDocument)
            {
                q.SetDefaultFilter("pfd");
                q.Add_filter("Portable Document Format", "pdf");
                q.Add_filter("Forms Data Format", "xfdf", "aiff", "fdf");
            }

            if (type_data == Carrot_File_Data.TextDocument)
            {
                q.SetDefaultFilter("txt");
                q.Add_filter("Text Data", "txt");
            }
            this.Handle_filter(q);
        }

        private void Handle_filter(Carrot_File_Query query)
        {
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                this.SimpleFileBrowser_filter = new FileBrowser.Filter[query.list_extension_file.Count];
                for (int i = 0; i < query.list_extension_file.Count; i++)
                {

                    string[] extension_file = new string[query.list_extension_file[i].Length];
                    string[] extension_file_handle = new string[extension_file.Length];

                    for (int y = 0; y < extension_file.Length; y++)
                    {
                        extension_file_handle[y] = "." + query.list_extension_file[i][y];
                    }
                    this.SimpleFileBrowser_filter[i] = new FileBrowser.Filter(query.s_title_file_data[i], extension_file_handle);
                }
                FileBrowser.SetFilters(true, this.SimpleFileBrowser_filter);
                if (query.s_DefaultFilter_extension_file != "") FileBrowser.SetDefaultFilter("." + query.s_DefaultFilter_extension_file);
            }
            else
            {
                this.StandaloneFileBrowser_filter = new ExtensionFilter[query.list_extension_file.Count + 1];

                for (int i = 0; i < this.StandaloneFileBrowser_filter.Length - 1; i++)
                {
                    string[] extension_file = query.list_extension_file[i];
                    this.StandaloneFileBrowser_filter[i] = new ExtensionFilter(query.s_title_file_data[i], extension_file);
                }

                this.StandaloneFileBrowser_filter[this.StandaloneFileBrowser_filter.Length - 1] = new ExtensionFilter("All Files", "*");
            }
        }

        public void Open_file(FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel= null)
        {
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                FileBrowser.ShowLoadDialog(Act_done, Act_cancel, FileBrowser.PickMode.Files, false);
            }
            else
            {
                StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", this.StandaloneFileBrowser_filter, false, (string[] paths) => {
                    if (paths.Length > 0)
                        Act_done(paths);
                    else
                        Act_cancel?.Invoke();
                });
            }
        }

        public void Open_file(Carrot_File_Query query,FileBrowser.OnSuccess Act_done,FileBrowser.OnCancel Act_cancel = null)
        {
            this.Handle_filter(query);
            this.Open_file(Act_done, Act_cancel);
        }

        public void Save_file(FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel=null)
        {
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                FileBrowser.ShowSaveDialog(Act_done, Act_cancel, FileBrowser.PickMode.Files, false);
            }
            else
            {
                StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "Myfile", this.StandaloneFileBrowser_filter, (string path) => {
                    if (path != "")
                    {
                        string[] paths = new string[1];
                        paths[0] = path;
                        Act_done(paths);
                    }
                    else
                    {
                        Act_cancel?.Invoke();
                    }
                });
            }
        }

        public void Open_folders(FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel = null)
        {
            if (this.type == Carrot_File_Type.SimpleFileBrowser)
            {
                FileBrowser.ShowLoadDialog(Act_done, Act_cancel, FileBrowser.PickMode.Folders, false);
            }
            else
            {
                string[] s_path=StandaloneFileBrowser.OpenFolderPanel("Open Folde", "", false);
                    if (s_path.Length > 0)
                        Act_done(s_path);
                    else
                        Act_cancel?.Invoke();
            }
        }

        public void Save_file(Carrot_File_Query query,FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel=null)
        {
            this.Handle_filter(query);
            this.Save_file(Act_done, Act_cancel);
        }

        public void Save_fold(Carrot_File_Query query, FileBrowser.OnSuccess Act_done, FileBrowser.OnCancel Act_cancel = null)
        {
            this.Handle_filter(query);
            this.Save_file(Act_done, Act_cancel);
        }
    }
}
