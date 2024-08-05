using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    public Text Text_LevelNumber;

    public void LevelNumberClicked(Text lvlNo)
    {
        //GamePlayManager.instance.UpdateLevels(int.Parse(lvlNo.text));
        GameManager.Int_LevelNo = int.Parse(lvlNo.text);
        GamePlayManager.instance.StartCoroutine("WaitForLevelLoading");
    }
}
