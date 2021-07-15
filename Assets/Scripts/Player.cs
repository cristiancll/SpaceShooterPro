using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerModel {
    private float speed;
    private string name;

    public PlayerModel() {

    }
    public PlayerModel(string name, float speed) {
        this.name = name;
        this.speed = speed;
    }
    public string Name {
        get { return name; }
        set { name = value; }
    }
    public float Speed {
        get { return speed; }
        set { speed = value; }
    }
}



public class Player : MonoBehaviour{

    [SerializeField] private PlayerModel model;

    [Header("Variables")]
    [SerializeField] private float _defaultSpeed = 10.0f;
    [SerializeField] private float _currentSpeed = 10.0f;
    [SerializeField] private float _speedBoost = 5f;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _score;
    [SerializeField] private bool _isTripleShot = false;
    [SerializeField] private bool _isSpeed = false;
    [SerializeField] private bool _isShield = false;
    [SerializeField] private float _powerUpTimeLimit = 15f;


    [Header("References")]
    [SerializeField] private GameObject _laserPrefab = null;
    [SerializeField] private GameObject _tripleShotPrefab = null;
    [SerializeField] private GameObject _shield = null;
    [SerializeField] private GameObject _thruster = null;
    [SerializeField] private GameObject _rightEngine = null;
    [SerializeField] private GameObject _leftEngine = null;
    [SerializeField] private GameObject _explosionPrefab = null;

    private UIManager _uiManager;
    private SpawnManager _spawnManager = null;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _laserClip = null;

    void UpdateThruster() {
        float horizontalInput = Mathf.Abs(Input.GetAxis("Horizontal"));
        float verticalInput = Mathf.Abs(Input.GetAxis("Vertical"));
        float xScale = 0f;
        if(horizontalInput != 0 || verticalInput != 0) {
            xScale = verticalInput;
            if(horizontalInput > verticalInput) xScale = horizontalInput;
            if (!_isSpeed) xScale = xScale / 2;
        }
        _thruster.transform.localScale = new Vector3(xScale, 1f, 1f);
    }

    void Start(){
        // SET PLAYER START POSITION
        transform.position = new Vector3(0, -3, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null) {
            Debug.LogError("Unable to locate SpawnManager");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null) {
            Debug.LogError("Unable to locate UIManager");
        }
        _audioSource = this.gameObject.GetComponent<AudioSource>();
        if(_audioSource == null) {
            Debug.LogError("Unable to locate LaserShot");
        } else {
            _audioSource.clip = _laserClip;

        }
    }

    void Update(){

        CalculateMovement();
        UpdateThruster();
        if ((Input.GetKey(KeyCode.Space)  || Input.GetButton("Fire1"))&& Time.time > _canFire) {
            FireLaser();
        }
    }

    void FireLaser() {
        _canFire = Time.time + _fireRate;
        if (_isTripleShot) {
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 0.05f, 0), Quaternion.identity);
        } else {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
        
    }

    void CalculateMovement() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if(horizontalInput != 0 || verticalInput != 0) {
            Vector3 playerPosition = transform.position;

            if ((playerPosition.y >= 5.5f && verticalInput > 0f) ||
                (playerPosition.y <= -3.75f && verticalInput < 0f)){
                verticalInput = 0f;
            }

            if(playerPosition.x > 11.25) {
                transform.position = new Vector3(-11.25f, playerPosition.y, 0);
            }else if(playerPosition.x < -11.25) {
                transform.position = new Vector3(11.25f, playerPosition.y, 0);
            }

            if(horizontalInput != 0 || verticalInput != 0) {
                Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

                if (_isSpeed) {
                    _currentSpeed = _defaultSpeed + _speedBoost;
                } else {
                    _currentSpeed = _defaultSpeed;
                }

                transform.Translate(direction * _currentSpeed * Time.deltaTime);
            }
        }
    }

    public void RemoveLife() {
        if (_isShield) {
            DisableShield();
            return;
        }
        _lives--;
        DamageShipEngine();
        _uiManager.UpdateLives(_lives);
        if(_lives == 0) {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject, 0.25f);
        }
    }

    public void AddLife() {
        _lives++;
    }

    void DamageShipEngine() {
        bool rightEngineDamage = false;
        bool leftEngineDamage = false;
        if (!_rightEngine.activeSelf && !_leftEngine.activeSelf) {
            float randomEngine = Random.Range(0.0f, 1.0f);
            if (randomEngine <= 0.5f) {
                rightEngineDamage = true;
            } else {
                leftEngineDamage = true;
            }
        } else if (_rightEngine.activeSelf && !_leftEngine.activeSelf) {
            leftEngineDamage = true;
        } else if (_leftEngine.activeSelf && !_rightEngine.activeSelf) {
            rightEngineDamage = true;
        }

        int offset = 1;
        if (rightEngineDamage) {
            Vector3 rightEnginePosition = _rightEngine.transform.position;
            Instantiate(_explosionPrefab, new Vector3(rightEnginePosition.x, rightEnginePosition.y + offset, rightEnginePosition.z), Quaternion.identity, _rightEngine.transform);
            _rightEngine.SetActive(true);
        }else if (leftEngineDamage) {
            Vector3 leftEnginePosition = _leftEngine.transform.position;
            Instantiate(_explosionPrefab, new Vector3(leftEnginePosition.x, leftEnginePosition.y + offset, leftEnginePosition.z), Quaternion.identity, _leftEngine.transform);
            _leftEngine.SetActive(true);
        }
    }
    public void EnablePowerup(PowerupEnum powerup) {
        switch (powerup) {
            case PowerupEnum.TripleShot:
                EnableTripleShot();
                break;
            case PowerupEnum.Speed:
                EnableSpeed();
                break;
            case PowerupEnum.Shield:
                EnableShield();
                break;
        }

    }
    void EnableTripleShot() {
        _isTripleShot = true;
        StartCoroutine(TripleShot());
    }

    void EnableSpeed() {
        _isSpeed = true;
        StartCoroutine(Speed());
    }

    void EnableShield() {
        if (!_isShield) {
            _isShield = true;
            _shield.SetActive(true);
        }
    }

    public IEnumerator TripleShot() {
        yield return new WaitForSeconds(_powerUpTimeLimit);
        DisableTripleShot();
    }

    public IEnumerator Speed() {
        yield return new WaitForSeconds(_powerUpTimeLimit);
        DisableSpeed();
    }

    void DisableTripleShot() {
        _isTripleShot = false;
    }

    void DisableSpeed() {
        _isSpeed = false;
    }

    void DisableShield() {
        _isShield = false;
        _shield.SetActive(false);
    }

    public void AddScore(int score) {
        _score += score;
        _uiManager.UpdateScore(_score);
    }
    public void RemoveScore(int score) {
        _score -= score;
    }
}
