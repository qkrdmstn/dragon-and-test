using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

[System.Serializable]
public enum SheetType
{
    Dialog,
    Tutorial,
    SpawnA,
    SpawnB,
    SpawnC,
    SkillDB,
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.data;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.data = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] data;
    }
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    const string url = "https://script.google.com/macros/s/AKfycbzMvWN6RUvMGi2cADNoCDwTJdlA364gncc1IUHyYW0m94MIkKrLcBkwMATagkKkA61-8Q/exec";

    public async Task SetValues<T>(SheetType sheetType, T[] datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "setValue");
        form.AddField("sheetType", sheetType.ToString());

        string jsonData = JsonHelper.ToJson<T>(datas);
        form.AddField("values", jsonData);

        await PostSetRequestAsync(form);
    }
  
    public async Task<T[]> GetValues<T>(SheetType sheetType, string range)
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "getValue");
        form.AddField("sheetType", sheetType.ToString());
        form.AddField("range", range);
        
        string data = await PostGetRequestAsync(form);

        return JsonHelper.FromJson<T>(data);
    }

    async Task<string> PostGetRequestAsync(WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
           await Task.Yield();
        }

        string output = "";
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("응답: " + www.downloadHandler.text);
            output = www.downloadHandler.text;
        }
        else
        {
            Debug.Log("에러: " + www.error);
        }
        www.Dispose();
        return output;
    }

    async Task<bool> PostSetRequestAsync(WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        bool output;
        if (www.result == UnityWebRequest.Result.Success)
        {
            output = true;
        }
        else
        {
            Debug.Log("에러: " + www.error);
            output = false;
        }
        www.Dispose();
        return output;
    }
}

