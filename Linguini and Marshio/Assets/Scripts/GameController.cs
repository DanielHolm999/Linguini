using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog}

public class GameController : MonoBehaviour
{
    //public static GameController gameController;

    //public void Awake()
    //{
    //    if(gameController != null)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    gameController = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    [SerializeField] Playercontroller playercontroller;
    [SerializeField] BattleSystem battleSystem;

    GameState state;

    private void Start()
    {
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
            
        };
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.FreeRoam:
                playercontroller.HandleUpdate();
                break;
            case GameState.Dialog:
                DialogManager.Instance.HandleUpdate();
                break;
            case GameState.Battle:
                //battleSystem.HandleUpdate();
                break;
        }
       
        
    }

}
