# Save System
The game utilizes save system in order to make the player progress persistent. The game saves information like:
- Money
- Equipped Items
- Bought Items
- Unlocked Stages
- Amount of Played Matches
- Seconds Played

Main menu shows information about the status of the cloud save. It gives a nice information to the player, however it's main purpose was for testing.

![image](https://user-images.githubusercontent.com/42221923/144412557-9ad34efb-eeb3-4ec7-a374-004da16c9326.png)

## How it works

SaveManager uses BinaryFormatter to serialize SaveData object to file.
```c#
 public void SerializeSaveData(){
    FileStream fs = new FileStream(path, FileMode.Create);
    bf.Serialize(fs, saveData);
    fs.Close();
}
```

Local save data is loaded on launch, and the game looks for cloud save right after connecting to the Google Play Games Services. When this information is obtained, SaveManager deserializes bytes obtained from Google Play Games Services, and determins if it should overwrite the local save data in the memory.

```c#
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
```

Users have the possibility to opt into the cloud saves from the Options menu, as well as opt out. For convenience, players are also able to fully wipe their progress.

![image](https://user-images.githubusercontent.com/42221923/144413961-330848c7-2ce4-408b-937e-58ed722d4868.png)
