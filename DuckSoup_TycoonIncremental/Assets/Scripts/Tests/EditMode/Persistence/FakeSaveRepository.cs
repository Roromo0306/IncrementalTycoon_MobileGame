public class FakeSaveRepository : ISaveRepository
{
    public GameSaveData LastSavedData
    {
        get;
        private set;
    }

    public int SaveCount
    {
        get;
        private set;
    }


    public GameSaveData Load()
    {
        return LastSavedData;
    }


    public void Save(GameSaveData data)
    {
        LastSavedData = data;

        SaveCount++;
    }


    public void Delete()
    {
        LastSavedData = null;
    }
}