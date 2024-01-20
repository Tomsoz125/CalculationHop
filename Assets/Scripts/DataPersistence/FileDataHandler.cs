using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "orange juice";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption) {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string name) {
        string fullPath = Path.Combine(dataDirPath, dataFileName.Replace("{name}", name));
        GameData loadedData = null;
        if (File.Exists(fullPath)) {
            try {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption) {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            } catch (Exception e) {
                Debug.LogError("Error occured when trying to read data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public GameData[] LoadAll() {
        string[] files = Directory.GetFiles(dataDirPath, "*.game", SearchOption.TopDirectoryOnly);
        List<GameData> data = new List<GameData>();
        foreach (string path in files) {
            try {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption) {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                data.Add(JsonUtility.FromJson<GameData>(dataToLoad));
            } catch (Exception e) {
                Debug.LogError("Error occured when trying to read data from file: " + path + "\n" + e);
            }
        }
        return data.ToArray();
    }

    public void Save(GameData data, string name) {
        string fullPath = Path.Combine(dataDirPath, dataFileName.Replace("{name}", name));
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption) {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        } catch (Exception e) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data) {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++) {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}
