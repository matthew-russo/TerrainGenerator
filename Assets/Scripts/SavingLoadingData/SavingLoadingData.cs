using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavingLoadingData : MonoBehaviour
{
    private string path;
    private string file = "terrains.dat";

    public Queue<SaveDataModel> savesQueue;
    private List<SaveDataModel> savesList;

    public InputField inputField;

    private void Awake()
    {
        path = Application.streamingAssetsPath;

        LoadInitialData();
    }

    public void Save()
    {
        string filePath = Path.Combine(path, file);
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, savesList);
        fs.Close();
    }

    // Saves the current terrain as the name the player gives it in the input field
    //
    public void SaveNewTerrain()
    {
        string filePath = Path.Combine(path, file);

        // Make sure someone actually entered something
        //
        if (string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        // Creates a new save file with the current informaion
        //
        SaveDataModel newSave = new SaveDataModel(inputField.text,
                                                        new SerializableVector3(GameObject.FindGameObjectWithTag("Player").transform.position), 
                                                        TerrainManager.Instance.randXOffset, 
                                                        TerrainManager.Instance.randYOffset);

        // Add new save to the list
        // If the list of save files is greater than 5, it gets rid of the oldest one.
        //
        savesQueue.Enqueue(newSave);
        if (savesQueue.Count > 5)
        {
            savesQueue.Dequeue();
        }

        Debug.Log(savesQueue.Count);


        // SaveNewTerrain Data
        savesList = new List<SaveDataModel>(savesQueue);
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, savesList);
        fs.Close();

        // Update the toggles with the new save information
        UIManager.Instance.UpdateToggles();
    }

    // Loads the selected terrain
    //
    public void Load()
    {
        Time.timeScale = 0;
        // Load Data
        string filePath = Path.Combine(path, file);
        FileStream stream = new FileStream(filePath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        savesList = (List<SaveDataModel>)bf.Deserialize(stream);
        savesQueue = new Queue<SaveDataModel>(savesList);
        stream.Close();

        // Loops through the toggles to find which one is on
        // Once it finds the selected one it loops through the save files to see which one's name matches the selected one.
        // This is also sort of janky and I want to refactor it to just work off of indices. ie toggle 4 = save file 4.
        //
        for (int i = 0; i < UIManager.Instance.toggles.Length; ++i)
        { 
            if (UIManager.Instance.toggles[i].isOn)
            {
                // SaveNewTerrain reference to rearrange list
                SaveDataModel saveFileToRearrange = savesList[savesList.Count - i - 1];

                savesList.Remove(saveFileToRearrange);
                savesQueue = new Queue<SaveDataModel>(savesList);
                savesQueue.Enqueue(saveFileToRearrange);

                UIManager.Instance.UpdateToggles();

                TerrainManager.Instance.showLoading = true;
                TerrainManager.Instance.timeToBreak = true;
                TerrainManager.Instance.randXOffset = saveFileToRearrange.xSeed;
                TerrainManager.Instance.randYOffset = saveFileToRearrange.ySeed;
                foreach (Transform child in TerrainManager.Instance.transform)
                {
                    Destroy(child.gameObject);
                }
                StartCoroutine(DelayRestartGeneration());
                //GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(saveFile.playerPosition.x, saveFile.playerPosition.y + 10f, saveFile.playerPosition.z);
                GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(10, 300, 10);
                GameObject.FindGameObjectWithTag("Player").transform.eulerAngles = new Vector3(0, 45, 0);
            }
        }

        stream = new FileStream(filePath, FileMode.OpenOrCreate);
        bf = new BinaryFormatter();
        bf.Serialize(stream, savesList);
        stream.Close();

        Time.timeScale = 1f;
    }

    // Loads the data in the List
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

    // Needed a delay between when the generation started and doubled the delete loop since not everything was getting properly cleared. Kinda janky
    //
    public IEnumerator DelayRestartGeneration()
    {
        yield return new WaitForSecondsRealtime(1f);
        foreach (Transform child in TerrainManager.Instance.transform)
        {
            Destroy(child.gameObject);
        }
        TerrainManager.Instance.timeToBreak = false;
        StartCoroutine(TerrainManager.Instance.Initialize());
        UIManager.Instance.OpenOrCloseWindow();
    }
}
