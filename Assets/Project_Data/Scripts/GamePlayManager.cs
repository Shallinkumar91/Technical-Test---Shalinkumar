using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;
    public List<GameObject> GameObject_LevelPrefab;
    public List<GameObject> GameObject_PopupBG;
    public GameObject GameObject_LevelNumberPrefab;
    public AudioSource AudioSource_GameMusic;
    public AudioClip AudioClip_GameCompleteSound;
    public Transform Transform_LevelNumberParent;
    public Transform Transform_LevelGeneratedParent;
    public Button Button_LevelCompleteNextButton;
    public Text Text_MoveShow;
    public Slider Slider_LoadingLevel;

    [SerializeField]
    private List<ParticleSystem> Particle_WinParticle;

    [HideInInspector]
    public List<int> Int_PipeRotationValue;
    
    private List<GameObject> GameObject_LevelNumberAList;

    private GameObject GameObject_LevelGenerated;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Screen.sleepTimeout =SleepTimeout.NeverSleep;

        instance = this;

        GameObject_LevelNumberAList = new List<GameObject>();
        Int_PipeRotationValue = new List<int> { 0, 45, 90, 135, 180, 225, 270, 315 };

        foreach(ParticleSystem p in Particle_WinParticle)
            p.Stop();

        LevelGeneration();
        PopupBGOnOff(0);
        SoundOnOff();
    }


    #region For Level Generating in Level Selection screen and Locking-Unlocking purpose
    private void LevelGeneration()
    {
        for (int i = 0; i < GameObject_LevelPrefab.Count; i++)
        {
            GameObject obj = Instantiate(GameObject_LevelNumberPrefab);
            obj.transform.parent = Transform_LevelNumberParent;
            obj.transform.localScale = Vector3.one;
            obj.GetComponent<LevelScript>().Text_LevelNumber.text = "" + (i + 1);
            GameObject_LevelNumberAList.Add(obj);

            LevelColorUpdate(i, obj);
        }
    }

    private void LevelColorUpdate(int i, GameObject obj)
    {
        if (i < GameManager.GetLevelUnlocking())
            obj.GetComponent<Image>().color = Color.green;
        else if (i == GameManager.GetLevelUnlocking())
            obj.GetComponent<Image>().color = Color.yellow;
        else
        {
            obj.GetComponent<Image>().color = Color.gray;
            obj.GetComponent<Button>().enabled = false;
        }
    }

    private void UpdateLevels(int no)
    {
        if (no > GameManager.GetLevelUnlocking())
            GameManager.SetLevelUnlocking(no);

        for (int i = 0; i < GameObject_LevelNumberAList.Count; i++)
        {
            GameObject obj = GameObject_LevelNumberAList[i];
            obj.GetComponent<Button>().enabled = true;

            LevelColorUpdate(i, obj);
        }

    }
    #endregion

    #region Game Play Core Logic
    public IEnumerator WaitForLevelLoading()
    {
        Slider_LoadingLevel.value = 0;
        MainLevelLoading(GameManager.Int_LevelNo);
        PopupBGOnOff(3);

        for(int i=0; i<21; i++)
        {
            yield return new WaitForSeconds(0.25f);

            Slider_LoadingLevel.value += 0.05f;
        }

        //yield return new WaitForSeconds(3);

        PopupBGOnOff();
    }

    private void MainLevelLoading(int no)
    {
        GameObject obj = Instantiate(GameObject_LevelPrefab[no - 1], new Vector3(0,0,0), new Quaternion(0,0,0,0));
        GameObject_LevelGenerated = obj;
        obj.transform.parent = Transform_LevelGeneratedParent;
    }

    public void CheckGameLevelCompleted()
    {
        //Debug.Log(GameObject_LevelGenerated.transform.childCount);
        for (int i = 0; i < GameObject_LevelGenerated.transform.childCount; i++)
        {
            Transform trm = GameObject_LevelGenerated.transform.GetChild(i);
            if (trm.gameObject.activeSelf && trm.GetComponent<PipeScript>()
                    && !trm.GetComponent<PipeScript>().Bool_IsPipeSet && !trm.GetComponent<PipeScript>().Bool_IsValve)
                return;
        }

        GameManager.Bool_IsAllPipeSet = true;
        Text_MoveShow.text = "Open Valve";

        //Debug.Log("calling");
    }

    private IEnumerator WaitForLevelCompletePopup()
    {
        foreach (ParticleSystem p in Particle_WinParticle)
            p.Play();
        GameObject_LevelGenerated.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(2);

#if UNITY_EDITOR             
        Camera.main.gameObject.GetComponent<CameraShakeScript>().CameraShake();
#endif
        yield return new WaitForSeconds(1);

        foreach (ParticleSystem p in Particle_WinParticle)
            p.Stop();
        DestroyAndReset();
        LevelCompleted();
    }

    private void LevelCompleted()
    {
        PopupBGOnOff(2);
        GameManager.Bool_IsAllPipeSet = false;
        UpdateLevels(GameManager.Int_LevelNo);

#if UNITY_ANDROID
        Handheld.Vibrate();
#endif

        Button_LevelCompleteNextButton.interactable = GameManager.Int_LevelNo < +GameObject_LevelPrefab.Count ? true : false;

        if (GameManager.IsSoundOnOff())
            AudioSource.PlayClipAtPoint(AudioClip_GameCompleteSound, Camera.main.transform.position, 100);
    }

    #endregion

    #region User Interface Manage Methods

    private void PopupBGOnOff(int no)
    {
        PopupBGOnOff();
        GameObject_PopupBG[no].SetActive(true);
    }

    private void PopupBGOnOff()
    {
        foreach (GameObject obj in GameObject_PopupBG)
            obj.SetActive(false);
    }

    private void DestroyAndReset()
    {
        Text_MoveShow.text = "0";
        GameManager.Int_MoveCounts = 0;
        Slider_LoadingLevel.value = 0;
        Destroy(GameObject_LevelGenerated);
        GameObject_LevelGenerated = null;
        StopCoroutine("WaitForLevelLoading");
    }

    private void SoundOnOff()
    {
        if(GameManager.IsSoundOnOff())
            AudioSource_GameMusic.Play();
        else 
            AudioSource_GameMusic.Pause();
    }

    #region User Interface Button Clicked

    public void Home_LevelButtonClicked() => Home_LevelButton();
    public void Home_SoundButtonClicked() => Home_SoundButton();
    public void LevelComplete_HomeButtonClicked() => LevelComplete_HomeButton();
    public void LevelComplete_RestartButtonClicked() => LevelComplete_RestartButton();
    public void LevelComplete_NextButtonClicked() => LevelComplete_NextButton();
    public void GameScene_PauseButtonClicked() => GameScene_PauseButton();
    public void Pause_HomeButtonClicked() => Pause_HomeButton();
    public void Pause_RestartButtonClicked() => Pause_RestartButton();
    public void Pause_ResumeButtonClicked() => Pause_ResumeButton();

    private void Pause_HomeButton()
    {
        DestroyAndReset();
        PopupBGOnOff(0);
    }

    private void Pause_RestartButton()
    {
        DestroyAndReset();
        StartCoroutine("WaitForLevelLoading");
    }

    private void Pause_ResumeButton()
    {
        PopupBGOnOff();
    }

    private void GameScene_PauseButton()
    {
        PopupBGOnOff(4);
    }

    void Home_LevelButton()
    {
        DestroyAndReset();
        PopupBGOnOff(1);
    }

    void Home_SoundButton()
    {
        GameManager.SetSoundOnOff(GameManager.GetSoundOnOff() == 0 ? 1 : 0);
        SoundOnOff();
    }

    void LevelComplete_HomeButton()
    {
        PopupBGOnOff(0);
    }

    void LevelComplete_RestartButton()
    {
        DestroyAndReset();
        StartCoroutine("WaitForLevelLoading");
    }

    void LevelComplete_NextButton()
    {
        GameManager.Int_LevelNo++;
        DestroyAndReset();
        StartCoroutine("WaitForLevelLoading");
    }
    #endregion

    #endregion

}
