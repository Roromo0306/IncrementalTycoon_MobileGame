public interface ISaveRepository
{
    GameSaveData Load();

    void Save(GameSaveData data);

    void Delete();
}