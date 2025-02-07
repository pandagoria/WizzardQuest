using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class SaveSystem {
    private JArray wordsDone = new JArray();
    JSON json = new JSON();
    private string saveFile;

    public SaveSystem(string pathName) {
        saveFile = Application.persistentDataPath + pathName;
    }

    void WriteToFile() {
        string jsonSerialized = json.CreatePrettyString();
        Debug.Log(jsonSerialized);
        StreamWriter writer = new StreamWriter(saveFile, false);
        writer.WriteLine(jsonSerialized);
        writer.Close();
    }

    void DeserealizeDataFromFile() {
        if (File.Exists(saveFile)) {
            StreamReader reader = new StreamReader(saveFile); 
            string jsonAsString = reader.ReadToEnd();
            reader.Close();
            Debug.Log(jsonAsString);
            json = JSON.ParseString(jsonAsString);
        } else {
            Debug.LogError("Save file not found " + saveFile);
        }
    }

    public void SavePlayer(float x_pos, float y_pos) {
        float[] player_position = new float[2];
        player_position[0] = x_pos;
        player_position[1] = y_pos;
        json.AddOrReplace("playerPosition", player_position);
        WriteToFile();
        Debug.Log("Player save file has been written" + saveFile);
    }

    public void SaveEnemy(bool exists, float pos_x, float pos_y) {
        float[] pos = new float[2];
        pos[0] = pos_x;
        pos[1] = pos_y;
        json.AddOrReplace("enemyPosition", pos);
        json.AddOrReplace("exists", exists);
        WriteToFile();
        Debug.Log("Enemy save file has been written" + saveFile);
    }

    public void SaveVolume() {
        json.AddOrReplace("volume", PlayerPrefs.GetFloat("volume", 1f));
        WriteToFile();
        Debug.Log("Volume save file has been written" + saveFile);
    }

    public float GetVolume() {
        DeserealizeDataFromFile();
        if (json.ContainsKey("volume")) {
            return json.GetFloat("volume");
        }
        return 1f;
    }

    public float[] GetPlayer() {
        DeserealizeDataFromFile();
        if (json.ContainsKey("playerPosition")) {
            JArray tmp = json.GetJArray("playerPosition");
            return tmp.AsFloatArray();
        }
        return null;
    }

    public float[] GetEnemy() {
        DeserealizeDataFromFile();
        if (json.ContainsKey("enemyPosition")) {
            JArray tmp = json.GetJArray("enemyPosition");
            return tmp.AsFloatArray();
        }
        float[] hui = new float[2];
        hui[0] = 0f;
        hui[1] = 0f;
        return hui;
    }

    public bool IsEnemyAlive() {
        if (json.ContainsKey("exists")) {
            return json.GetBool("exists");
        }
        return true;
    }

    public string[] GetWordsDone() {
        DeserealizeDataFromFile();
        if (json.ContainsKey("wordsDone")) {
            wordsDone = json.GetJArray("wordsDone");
            return wordsDone.AsStringArray();
        }
        return null;
    }

    public void SaveWordsDone(string word) {
        wordsDone.Add(word);
        json.AddOrReplace("wordsDone", wordsDone);
        WriteToFile();
        Debug.Log("Guessed words save file has been written" + saveFile);
    }
}