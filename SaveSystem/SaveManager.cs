using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData saveData;
    private string path;
    private float secondsPassed = 0;
    private BinaryFormatter bf;

    void Awake(){
        if(!Instance)
            Instance = this;
        else
            Destroy(this.gameObject);
        path = Application.persistentDataPath + "/savedata/save.chicken";
        bf = new BinaryFormatter();
        GetSaveData();
    }

    void Update(){
        // Count time played
        if(GameManager.Instance != null && !GameManager.Instance.end){
            secondsPassed += Time.unscaledDeltaTime;
            if(secondsPassed >= 1){
                saveData.secondsPlayed++;
                secondsPassed -= 1;
            }
        }
    }

    void GetSaveData(){
        if(File.Exists(path)){
            DeserializeSaveData();
        }else{
            if(!Directory.Exists(Application.persistentDataPath + "/savedata"))
                Directory.CreateDirectory(Application.persistentDataPath + "/savedata");
            // Create new save data if it doesn't exist, and add default items
            saveData = new SaveData();
            saveData.unlockedSkins.Add("Mighty Chuck");
            saveData.unlockedEffects.Add("None");
            SerializeSaveData();
        }
    }

    public void SerializeSaveData(){
        FileStream fs = new FileStream(path, FileMode.Create);
        bf.Serialize(fs, saveData);
        fs.Close();
    }

    void DeserializeSaveData(){
        FileStream fs = new FileStream(path, FileMode.Open);
        saveData = (SaveData) bf.Deserialize(fs);
        fs.Close();
    }

    public void LoadByteData(byte[] data){
        // Load save obtained from from cloud
        if(data == null || data.Length == 0)
            return;
        SaveData newData;
        using (MemoryStream ms = new MemoryStream(data)){
            newData = (SaveData) bf.Deserialize(ms);
        }
        if(newData.secondsPlayed > saveData.secondsPlayed){
            saveData = newData;
        }else if(newData.secondsPlayed == saveData.secondsPlayed && newData.money > saveData.money)
            saveData = newData;
    }

    public byte[] GetByteData(){
        // Turn save data into bytes to save in cloud
        using (MemoryStream ms = new MemoryStream()){
            bf.Serialize(ms, saveData);
            return ms.ToArray();
        }
    }
}
