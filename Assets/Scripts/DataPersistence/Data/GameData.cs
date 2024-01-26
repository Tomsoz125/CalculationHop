using System.Collections.Generic;

[System.Serializable]
public class GameData {
    public string name;
    public SerializableDictionary<int, int> scores;
    public int stars;

    public GameData(string name) {
        this.name = name;
        scores = new SerializableDictionary<int, int>();
        stars = 0;
    }
}