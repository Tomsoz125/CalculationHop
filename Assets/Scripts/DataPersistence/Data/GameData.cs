using System.Collections.Generic;

[System.Serializable]
public class GameData {
    public string name;
    public SerializableDictionary<int, int> scores;

    public GameData(string name) {
        this.name = name;
        scores = new SerializableDictionary<int, int>();
    }
}