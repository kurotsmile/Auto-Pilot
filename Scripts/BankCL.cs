using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Database;
//using Firebase.Extensions;

public class BankCL : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;

    [Header("Bank")]
    public Bank_Item[] bank_items;
    private int index_sel_bank;

    //private DatabaseReference databaseRef;
    //private FirebaseApp customApp;


    public void On_Load(){
        this.Load_config_app();
        this.index_sel_bank=PlayerPrefs.GetInt("index_sel_bank",0);
        this.Update_ui_list_bank();
    }

    public void Load_config_app(){
        /*
        AppOptions options = new AppOptions
        {
            ApiKey = "AIzaSyBKQ51navOWhgLHY1flH7eK4hPuj9knOa0", 
            AppId = "14228562704",
            ProjectId = "clbank",
            DatabaseUrl = new System.Uri("https://clbank-default-rtdb.asia-southeast1.firebasedatabase.app"),
            StorageBucket = "clbank.appspot.com"
        };

        if(customApp==null){
            this.app.txt_status_app.text="Start software initialization and server connection...";
            customApp = FirebaseApp.Create(options, "customApp_bank");
            databaseRef = FirebaseDatabase.GetInstance(customApp).RootReference;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                Firebase.DependencyStatus status = task.Result;
                if (status == Firebase.DependencyStatus.Available)
                {
                    Debug.Log("Firebase is ready with custom config.");
                    ReadDataFromFirebase();
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
                }
            });
        }
        else{
            Debug.Log("App da duoc tao!");
        } 
        */
    }

    private void Update_ui_list_bank(){
        for(int i=0;i<this.bank_items.Length;i++) this.bank_items[i].img_bk.color=this.app.color_nomal;
        this.bank_items[this.index_sel_bank].img_bk.color=this.app.color_sel;
    }


    void ReadDataFromFirebase()
    {
        //DatabaseReference usersRef = databaseRef.Child("lich_su_nap_rut");
        //usersRef.ValueChanged += HandleValueChanged;
    }

    /*
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Lỗi đọc dữ liệu: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists)
        {
            this.app.Clear_contain(this.app.tr_all_item);
            foreach (var dateSnapshot in args.Snapshot.Children)
            {
                string dateKey = dateSnapshot.Key; 
 
                foreach (var moneySnapshot in dateSnapshot.Children)
                {
                    string moneyKey = moneySnapshot.Key;
                    string moneyValue = moneySnapshot.Value.ToString();

                    GameObject obj_bill = Instantiate(this.app.prefab_item_bill);
                    obj_bill.transform.SetParent(this.app.tr_all_item);
                    obj_bill.transform.localPosition = Vector3.zero;
                    obj_bill.transform.localScale = new Vector3(1f, 1f, 1f);
                    Bill_Item bill = obj_bill.GetComponent<Bill_Item>();

                    if (moneySnapshot.HasChild("username")) bill.txt_name.text = moneySnapshot.Child("username").Value.ToString();
                    if (moneySnapshot.HasChild("money")) bill.txt_tip.text = moneySnapshot.Child("money").Value.ToString();
                    if (moneySnapshot.HasChild("date")) bill.txt_date.text = moneySnapshot.Child("date").Value.ToString();

                    bill.Set_Act_Click(() =>
                    {
                        this.app.adb.RunCommandWithMemu("start");
                    });
                }
            }
        }
        else
        {
            Debug.Log("Không có dữ liệu.");
        }
    }

    public void OnDisable() {
        if(databaseRef!=null) databaseRef.ValueChanged -= HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GoOffline();
    }

   public void QueryList()
    {
        DatabaseReference usersRef = databaseRef.Child("lich_su_nap_rut");
        Query ageQuery = usersRef.OrderByChild("status").EqualTo("done");

        ageQuery.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Lỗi truy vấn: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        Debug.Log("User ID: " + childSnapshot.Key);
                        Debug.Log("User Data: " + childSnapshot.GetRawJsonValue());
                    }
                }
            }
        });
    }

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            FirebaseDatabase.DefaultInstance.GoOffline();
        } else {
            FirebaseDatabase.DefaultInstance.GoOnline();
         }
    }

    
    public void Select_bank(int index){
        PlayerPrefs.SetInt("index_sel_bank",index);
        this.index_sel_bank=index;
        this.Update_ui_list_bank();
        this.app.cr.play_sound_click();
    }
    */
}
