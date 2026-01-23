[System.Serializable]
public class WordData
{
    public string word;
    public string categoryId;

    public WordData(string word, string categoryId)
    {
        this.word = word;
        this.categoryId = categoryId;
    }
}
