using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupEnum {
    TripleShot, // 0
    Speed,      // 1
    Shield      // 2
}


public class Powerup : MonoBehaviour{

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private PowerupEnum powerup;
    [SerializeField] private AudioClip _audioClip = null;
    private SpawnManager _spawnManager;

    void Start() {
        LoadPowerup();
        transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 8.0f, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError("Unable to locate SpawnManager");
        }
    }

    void Update(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y <= -6) {
            DestroyPowerup();
        }
    }

    void LoadPowerup() {
        string powerupType = transform.tag;
        if (powerupType.Equals("Tripleshot")) {
            powerup = PowerupEnum.TripleShot;
        } else if (powerupType.Equals("Speed")) {
            powerup = PowerupEnum.Speed;
        } else if (powerupType.Equals("Shield")) {
            powerup = PowerupEnum.Shield;
        }
    }

    void DestroyPowerup() {
        _spawnManager.OnPowerupDeath();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            Player player = other.transform.GetComponent<Player>();
            if(player != null) {
                player.EnablePowerup(powerup);
            }
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            DestroyPowerup();
        }
    }

}
