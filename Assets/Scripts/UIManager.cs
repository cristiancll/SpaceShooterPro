using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour{

    [SerializeField] private GameObject _helpPanel = null;
    [SerializeField] private GameObject _pausePanel = null;
    [SerializeField] private Text _scoreText = null;
    [SerializeField] private Text _gameOverText = null;
    [SerializeField] private Text _restartText = null;
    [SerializeField] private Text _timeText = null;
    [SerializeField] private Sprite[] _livesSprites = null;
    [SerializeField] private Image _livesImage = null;
    

private GameManager _gameManager;
    void Start(){
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null) {
            Debug.LogError("GameManager is null");
        }
        SetDefaults();
    }

    public void UpdateTime(float gameTime){
        System.TimeSpan time = System.TimeSpan.FromSeconds((double) gameTime);
        _timeText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)time.Hours, time.Minutes, time.Seconds);
        
    }

    public void UpdateScore(int score) {
        _scoreText.text = score.ToString();
    }
    void SetDefaults() {
        _livesImage.sprite = _livesSprites[3];
        _scoreText.text = "0";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    public void GameOverOverlay() {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _gameManager.GameOver();
    }

    IEnumerator GameOverFlickerRoutine() {
        for(int i = 0; i < 3; i++) {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
        _gameOverText.text = "GAME OVER";
    }
    public void UpdateLives(int lives) {
        if(lives >= 0 || lives <= _livesSprites.Length) {
            _livesImage.sprite = _livesSprites[lives];
            if(lives == 0) {
                GameOverOverlay();
            }
        } else {
            Debug.LogError("UpdateLives out of bounds. Expected 0 ~ "+_livesSprites.Length+", received " + lives);
        }
    }

    public void DisplayHelpPanel(bool isActive) {
        _helpPanel.gameObject.SetActive(isActive);
    }

    public void PauseGame(bool status) {
        _pausePanel.SetActive(status);
    }
}
