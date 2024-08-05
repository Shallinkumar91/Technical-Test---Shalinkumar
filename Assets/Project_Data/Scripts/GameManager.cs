using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static bool Bool_IsAllPipeSet=false;
    public static int Int_LevelNo = 0;
    public static int Int_MoveCounts = 0;

    #region Locking-Unlocking Levels

    public static void SetLevelUnlocking(int no)
    {
        PlayerPrefs.SetInt("LevelManager", no);
    }
    public static int GetLevelUnlocking()
    {
        return PlayerPrefs.GetInt("LevelManager", 0);
    }

    #endregion


    #region Sound On-Off 
    public static void SetSoundOnOff(int no)
    {
        PlayerPrefs.SetInt("SoundManager", no);
    }
    public static int GetSoundOnOff()
    {
        return PlayerPrefs.GetInt("SoundManager", 0);
    }

    public static bool IsSoundOnOff()
    {
        return GetSoundOnOff() == 0 ? true : false;
    }

    #endregion

}
