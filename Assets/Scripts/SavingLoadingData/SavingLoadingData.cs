using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavingLoadingData : MonoBehaviour {
    private string path;
    private string file = "terrains.dat";

    public Queue<SaveDataModel> savesQueue;
    private List<SaveDataModel> savesList;

    public InputField inputField;

    private void Awake()
    {
        path = Application.streamingAssetsPath;
    }

    public void Save()
    {
        string filePath = Path.Combine(path, file);

        if (string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        //if (File.Exists(filePath))
        //{
            //Rearrange Queue
            SaveDataModel newSave = new SaveDataModel(inputField.text,
                                                        new SerializableVector3(GameObject.FindGameObjectWithTag("Player").transform.position), 
                                                        TerrainManager.Instance.randXOffset, 
                                                        TerrainManager.Instance.randYOffset);

        if (savesQueue.Count > 5)
        {
            savesQueue.Dequeue();
        }
        savesQueue.Enqueue(newSave);

        // Save Data
        savesList = new List<SaveDataModel>(savesQueue);

        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, savesList);
        fs.Close();

        UIManager.Instance.UpdateToggles();
    }

    public void Load()
    {
        // Load Data
        string filePath = Path.Combine(path, file);
        FileStream stream = new FileStream(filePath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        savesList = (List<SaveDataModel>)bf.Deserialize(stream);
        savesQueue = new Queue<SaveDataModel>(savesList);
        stream.Close();
        foreach(Toggle option in UIManager.Instance.toggles)
        {
            if (option.isOn)
            {
                foreach (SaveDataModel saveFile in savesList)
                {
                    if (saveFile.name == option.GetComponentInChildren<Text>().text)
                    TerrainManager.Instance.randXOffset = saveFile.xSeed;
                    TerrainManager.Instance.randYOffset = saveFile.ySeed;
                    foreach (Transform child in TerrainManager.Instance.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    StartCoroutine(TerrainManager.Instance.Initialize());
                    GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(saveFile.playerPosition.x, 200f, saveFile.playerPosition.z);
                }
            }
        }
        
    }

    public void LoadInitialData()
    {
        string filePath = Path.Combine(path, file);

        if (File.Exists(filePath))
        {
            // Load Data
            FileStream stream = new FileStream(filePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            savesList = (List<SaveDataModel>)bf.Deserialize(stream);
            savesQueue = new Queue<SaveDataModel>(savesList);
            stream.Close();
        }
        else
        {
            throw new UnityException("Trying to open a file that does not exist");
        }
    }
}
