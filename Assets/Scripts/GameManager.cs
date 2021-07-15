using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    [Header("Variables")]
    [SerializeField] private bool _isGameOver;
    [SerializeField] private bool _isGamePaused = false;
    private float _gameTime;
    private UIManager _uiManager;

    void Start() {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null) {
            Debug.LogError("UI Manager is null.");
        }
    }

    void Update() {
        if (_isGameOver) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(1);
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        } else {
            _gameTime = Time.timeSinceLevelLoad;
            _uiManager.UpdateTime(_gameTime);
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Esc pressed. _isGamePaused: " + _isGamePaused);
                if (_isGamePaused) {
                    Time.timeScale = 1;
                    _isGamePaused = false;
                } else {
                    Time.timeScale = 0;
                    _isGamePaused = true;
                }
                _uiManager.PauseGame(_isGamePaused);
            }
        }
    }


    public void GameOver() {
        _isGameOver = true;
    }
    public bool IsGameOver() {
        return _isGameOver;
    }
}
