using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            Debug.Log(Application.persistentDataPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            result = "";
            return false;
        }
    }
}

public static class SaveDataManager
{
    public static void SaveJsonData(int slotIndex, string comment)
    {
        SaveData sd = new SaveData();

        sd.dataComment = comment;
        sd.playerData = ProgressManager.Instance.PlayerData;

        if (FileManager.WriteToFile("SaveData" + slotIndex.ToString("00") + ".dat", sd.ToJson()))
        {
            Debug.Log("Save successful as SaveData" + slotIndex.ToString("00") + ".dat");
        }
    }

    public static void LoadJsonData(int slotIndex)
    {
        if (FileManager.LoadFromFile("SaveData" + slotIndex.ToString("00") + ".dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            ProgressManager.Instance.ApplyLoadedData(sd.playerData);

            Debug.Log("Load SaveData" + slotIndex.ToString("00") + ".dat");
        }
    }

    public static string GetDataInfo(int slotIndex)
    {
        if (FileManager.LoadFromFile("SaveData" + slotIndex.ToString("00") + ".dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            string slotInfo = (slotIndex + 1).ToString() + "  Chapter " + ((sd.playerData.currentStage / 3)+1).ToString() + "-" + ((sd.playerData.currentStage % 3)+1).ToString();
            if (sd.dataComment != string.Empty) slotInfo += " [" + sd.dataComment + "]";
            return slotInfo;
        }

        return (slotIndex + 1).ToString() + "  <color=grey>No Data</color>";
    }


    public static bool IsDataExist(int slotIndex)
    {
        return (FileManager.LoadFromFile("SaveData" + slotIndex.ToString("00") + ".dat", out var json));
    }
}

[System.Serializable]
public class SaveData
{
    public string dataComment; // íçéﬂ
    public PlayerData playerData;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}