using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{

    [Header("Variables")]
    [SerializeField] private bool _isAlive;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private int _pointValue = 10;
    [SerializeField] private float _chanceToFire = 0.25f;
    [SerializeField] private float _fireRate = 0.5f;
    private float _xMovement;
    private float _initializationTime;
    private float _startingXPosition;

    [Header("References")]
    [SerializeField] private GameObject _laserPrefab = null;
    [SerializeField] private GameObject _enemyThruster = null;


    private SpawnManager _spawnManager;
    private Player _player;
    private Animator _animator;

    private Color[] _laserColors = { Color.green, Color.blue, Color.yellow };
    private Color _laserColor;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionClip = null;


    void Start(){
        _initializationTime = Time.timeSinceLevelLoad;
        _xMovement = Random.Range(0f, 1.5f);
        _isAlive = true;
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) {
            Debug.LogError("Unable to locate Player");

        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError("Unable to locate SpawnManager");
        }
        _animator = this.GetComponent<Animator>();
        if(_animator == null) {
            Debug.LogError("Unable to locate Animator");
        }
        _audioSource = this.gameObject.GetComponent<AudioSource>();
        if (_audioSource == null) {
            Debug.LogError("Unable to locate LaserShot");
        } else {
            _audioSource.clip = _explosionClip;

        }
        Respawn();
        StartCoroutine(EnemyFireRoutine());

    }

    void Respawn() {
        _laserColor = _laserColors[Random.Range(0, _laserColors.Length)];
        _startingXPosition = Random.Range(-9.0f, 9.0f);
        _xMovement = Random.Range(0f, 1.5f);
        transform.position = new Vector3(_startingXPosition, 8.0f, 0);
    }

    void CalculateMovement() {
        Vector3 currPos = transform.position;
        float xMovement = Mathf.Lerp(-_xMovement, +_xMovement, Mathf.PingPong(Time.timeSinceLevelLoad - _initializationTime, 1f));


        Vector3 movement = new Vector3(xMovement, -1, 0);
        transform.Translate(movement * _speed * Time.deltaTime);

        if(currPos.y <= -6 && _isAlive) {
            Respawn();
        }
    }
    void Update(){
        CalculateMovement();
    }

    IEnumerator EnemyFireRoutine() {
        
        while (_isAlive) {
            float fire = Random.Range(0.0f, 1.0f);
            if(fire <= _chanceToFire) {
                GameObject laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, -0.6f, 0), Quaternion.identity);
                laser.GetComponent<SpriteRenderer>().color = _laserColor;
                laser.GetComponent<Laser>().AssignEnemyLaser();
            }
            yield return new WaitForSeconds(_fireRate);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_isAlive) {
            string otherName = other.transform.tag;

            if (otherName.Equals("Player")) {
                Player player = other.GetComponent<Player>();
                if(player != null) {
                    player.RemoveLife();
                }
                DestroyEnemy();
            }else if (otherName.Equals("Player Laser")) {
                if(_player != null) {
                    _player.AddScore(_pointValue);
                }
                Destroy(other.gameObject);
                DestroyEnemy();
            }
        }
    }

    public void DestroyEnemy() {
        _isAlive = false;
        _audioSource.clip = _explosionClip;
        _audioSource.Play();
        _spawnManager.OnEnemyDeath();
        _animator.SetTrigger("OnEnemyDeath");
        StartCoroutine(DisableThrusterRoutine());
        Destroy(this.gameObject, 2.35f);
    }

    IEnumerator DisableThrusterRoutine() {
        Vector3 t = _enemyThruster.transform.localScale;
        while(t.x > 0) {
            t.Set(t.x - 0.01f, t.y, t.z);
            _enemyThruster.transform.localScale = t;
            yield return new WaitForSeconds(.01f);
        }
    }


    
}
