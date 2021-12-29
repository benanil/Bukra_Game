
public interface ISaveLoad<T>
{
    /// <summary>
    /// saves for xml
    /// </summary>
    T Save();

    /// <summary>
    /// Saves For Xml
    /// </summary>
    /// <param name="loadData"></param>
    void Load(T loadData);
}

