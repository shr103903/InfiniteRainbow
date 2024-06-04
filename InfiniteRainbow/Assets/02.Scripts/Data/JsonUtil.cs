using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class JsonData
{
    public int floor;

    public int[] hpUpgrade;
    public int[] atkUpgrade;
    public int[] defUpgrade;
    public int[] dodgeUpgrade;
    public int[] criChanceUpgrade;
    public int[] criMultiUpgrade;
    public int[] speedUpgrade;
}

public class JsonUtil : MonoBehaviour
{
    public void SaveData()
    {
        JsonData data = new JsonData();
        data.floor = StatusData.floor;
        data.hpUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.hpUpgrade[i] = StatusData.hpUpgrade[i];
        }
        data.atkUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.atkUpgrade[i] = StatusData.atkUpgrade[i];
        }
        data.defUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.defUpgrade[i] = StatusData.defUpgrade[i];
        }
        data.dodgeUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.dodgeUpgrade[i] = StatusData.dodgeUpgrade[i];
        }
        data.criChanceUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.criChanceUpgrade[i] = StatusData.criChanceUpgrade[i];
        }
        data.criMultiUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.criMultiUpgrade[i] = StatusData.criMultiUpgrade[i];
        }
        data.speedUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.speedUpgrade[i] = StatusData.speedUpgrade[i];
        }

        string toJson = JsonUtility.ToJson(data);
        if (!Directory.Exists(Application.persistentDataPath + "/Saved"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saved");
        }
        string path = Path.Combine(Application.persistentDataPath + "/Saved/", "savedData.json");
        File.WriteAllText(path, toJson);
    }

    public bool LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath + "/Saved/", "savedData.json");
        if (!File.Exists(path))
        {
            Debug.Log("저장된 내용 없습니다.");
            return false;
        }
        else
        {
            JsonData data = new JsonData();
            string jsonData = File.ReadAllText(path);
            data = JsonUtility.FromJson<JsonData>(jsonData);

            if (data != null)
            {
                try
                {
                    StatusData.floor = data.floor;
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.hpUpgrade[i] = data.hpUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.atkUpgrade[i] = data.atkUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.defUpgrade[i] = data.defUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.dodgeUpgrade[i] = data.dodgeUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.criChanceUpgrade[i] = data.criChanceUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.criChanceUpgrade[i] = data.criChanceUpgrade[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        StatusData.speedUpgrade[i] = data.speedUpgrade[i];
                    }
                    return true;
                }
                catch
                {
                    Debug.Log("저장된 데이터 형태가 잘못 되었습니다.");
                    return false;
                }
            }
            Debug.Log("저장된 데이터 형태가 잘못 되었습니다.");
            return false;
        }
    }
}
