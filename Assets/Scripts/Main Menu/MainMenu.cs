using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    [SerializeField] private Text _startGameText = null;
    private bool _holdingDown;
    void Update() {
        _startGameText.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1f));

        if (Input.anyKey) {
            _holdingDown = true;
        }

        if (!Input.anyKey && _holdingDown) {
            _holdingDown = false;
            SceneManager.LoadScene(1);
        }
    }

}
