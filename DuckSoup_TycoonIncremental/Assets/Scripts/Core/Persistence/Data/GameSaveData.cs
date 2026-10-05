using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int saveVersion;

    public string money;

    public int rebirthLevel;

    public string lastPlayedTimeUtc;

    public List<FloorSaveData> floors =
        new List<FloorSaveData>();
}