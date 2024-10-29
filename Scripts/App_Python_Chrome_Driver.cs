using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class App_Python_Chrome_Driver : MonoBehaviour
{
    private string apiUrl = "http://127.0.0.1:5000/run";

    public void Run(string s_data_json)
    {
        StartCoroutine(SendRequest(s_data_json));
    }

    private IEnumerator SendRequest(string s_data_json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(s_data_json);

        var request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}
