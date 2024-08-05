using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    //[HideInInspector]
    public bool Bool_IsValve = false;

    [SerializeField]
    private List<int> Int_PipeRightPositionList;

    //[HideInInspector]
    public bool Bool_IsPipeSet = false;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, GamePlayManager.instance.Int_PipeRotationValue[Random.Range(0, GamePlayManager.instance.Int_PipeRotationValue.Count)]));
        Bool_IsPipeSet = Int_PipeRightPositionList.Contains((int)transform.eulerAngles.z) ? true : false;
    }

    private void Update()
    {
        if (Bool_IsValve && Bool_IsPipeSet && GameManager.Bool_IsAllPipeSet)
            transform.Rotate(Vector3.forward * 5);
    }

    private void OnMouseDown()
    {
        if (!Bool_IsValve)
        {
            transform.Rotate(new Vector3(0, 0, 45));
            GamePlayManager.instance.Text_MoveShow.text = "" + (++GameManager.Int_MoveCounts);

            if (Int_PipeRightPositionList.Contains((int)transform.eulerAngles.z))
            {
                Bool_IsPipeSet = true;
                GamePlayManager.instance.CheckGameLevelCompleted();
            }
            else
                Bool_IsPipeSet = false;

        }
        else if (Bool_IsValve && GameManager.Bool_IsAllPipeSet)
        {
            GamePlayManager.instance.StartCoroutine("WaitForLevelCompletePopup");
            Bool_IsPipeSet = true;
        }
    }
}
