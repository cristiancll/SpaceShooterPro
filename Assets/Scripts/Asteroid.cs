using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour{

    [SerializeField] private float _rotationSpeed = 20f;
    private int direction;
    [SerializeField] private GameObject _explosionPrefab = null;
    private bool _isAlive;
    private UIManager _uiManager;

    private SpawnManager _spawnManager;

    void Start(){
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null) {
            Debug.LogError("UI Manager is null.");
        }

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null) {
            Debug.LogError("Unable to find SpawnManager");
        }

        _isAlive = true;
        float rotationDirection = Random.Range(0f, 1f);
        direction = 1;
        if(rotationDirection <= 0.5f) {
            direction = -1;
        }

        //float size = Random.Range(0.5f, 1f);
        //transform.localScale = new Vector3(size, size, size);
    }

    void Update(){
        transform.Rotate(0, 0, (_rotationSpeed * direction * Time.deltaTime));
    }



    private void OnTriggerEnter2D(Collider2D other) {
        string otherName = other.transform.tag;
        if (_isAlive) {
            _isAlive = false;
            if (otherName.Equals("Player")) {
                Player player = other.GetComponent<Player>();
                if (player != null) {
                    player.RemoveLife();
                }
                DestroyAsteroid();
            } else if (otherName.Equals("Player Laser")) {
                Destroy(other.gameObject);
                DestroyAsteroid();
            }
            //else if(otherName.Equals("Enemy")) {
            //    Enemy enemy = other.GetComponent<Enemy>();
            //    if(enemy != null) {
            //        enemy.DestroyEnemy();
            //    }
            //    DestroyAsteroid();

            //}
        }
    }

    void DestroyAsteroid() {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _spawnManager.StartSpawning();
        Destroy(this.gameObject, 0.25f);
        _uiManager.DisplayHelpPanel(false);
    }




}
