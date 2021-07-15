using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour{


    // Enemy Spawn Manager
    [SerializeField] private GameObject _enemyPrefab = null;
    [SerializeField] private GameObject _enemyContainer = null;
    [SerializeField] private float _enemySpawnRate = 1.0f;
    [SerializeField] private int _maxEnemies = 3;
    private int _totalEnemies = 0;
    private bool _isEnemySpawning = true;



    // Powerup Spawn Manager

    [SerializeField] private GameObject[] _powerUpPrefabs = null;
    [SerializeField] private GameObject _powerupContainer = null;
    [SerializeField] private float _chanceToSpawnPowerup = 0.25f;
    [SerializeField] private int _maxPowerups = 1;
    private int _totalPowerups = 0;
    private bool _isPowerupSpawning = true;

    public void StartSpawning() {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());

    }
    IEnumerator SpawnEnemyRoutine() {
        yield return new WaitForSeconds(3f);
        while (_isEnemySpawning) {
            if(_totalEnemies < _maxEnemies) {
                GameObject enemySpawned = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
                enemySpawned.transform.parent = _enemyContainer.transform;
                _totalEnemies++;
                yield return new WaitForSeconds(_enemySpawnRate);
            } else {
                yield return null;
            }
        }
    }

    IEnumerator SpawnPowerupRoutine() {
        yield return new WaitForSeconds(3f);
        while (_isPowerupSpawning) {
            yield return new WaitForSeconds(10);
            if(_totalPowerups < _maxPowerups) {
                float rolledNumber = Random.Range(0.0f, 1.0f);
                if(rolledNumber < _chanceToSpawnPowerup) {
                    _totalPowerups++;

                    int randomPowerup = Random.Range(0, System.Enum.GetValues(typeof(PowerupEnum)).Length);
                    GameObject powerupSpawned = Instantiate(_powerUpPrefabs[randomPowerup], transform.position, Quaternion.identity);
                    powerupSpawned.transform.parent = _powerupContainer.transform;
                }
            } else {
                yield return null;
            }
        }
    }

    public void OnPowerupDeath() {
        _totalPowerups--;
    }
    public void OnEnemyDeath() {
        _totalEnemies--;
    }

    public void OnPlayerDeath() {
        _isEnemySpawning = false;
    }
}
