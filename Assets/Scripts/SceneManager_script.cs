using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager_script : MonoBehaviour {

    GameObject button_omok;
    GameObject button_chess;
    GameObject button_start;
    GameObject button_exit;
    GameObject button_back;
	// Use this for initialization
	void Start () {
        // 오목, 체스 버튼 비활성화
        button_omok = GameObject.Find("ButtonGoOmok");
        button_chess = GameObject.Find("ButtonGoChess");
        button_start = GameObject.Find("ButtonStart");
        button_exit = GameObject.Find("ButtonExit");
        button_back = GameObject.Find("ButtonBack");

        button_omok.SetActive(false);
        button_chess.SetActive(false);
        button_back.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScene(int gameID)
    {
        switch(gameID) {
            case 1:
                SceneManager.LoadScene(1);
                break;
            case 2:
                SceneManager.LoadScene("ChessScene");
                break;
        }
    }

    public void ActivateButton(bool act)
    {
        // Start 버튼 OnClick -> 오목, 체스, back 버튼 활성화
        if (act)
        {
            button_start.SetActive(false);
            button_exit.SetActive(false);

            button_omok.SetActive(true);
            button_chess.SetActive(true);
            button_back.SetActive(true);
        }

        // Back 버튼 OnClick -> act == false. 오목, 체스, back 버튼 비활성화. Start, Exit 버튼 활성화
        else
        {
            button_start.SetActive(true);
            button_exit.SetActive(true);

            button_omok.SetActive(false);
            button_chess.SetActive(false);
            button_back.SetActive(false);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
