using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour{

    [Header("Variables")]
    [SerializeField] private float _speed = 8.0f;
    private Vector3 _direction = Vector3.up;

    private bool _isEnemyLaser = false;

    void Update(){
        transform.Translate(_direction * _speed * Time.deltaTime);

        Vector3 laserPosition = transform.position;
        
        if(laserPosition.y >= 8.0f || laserPosition.y <= -6.0f) {

            if(transform.parent != null) {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag.Equals("Player") && _isEnemyLaser) {
            Player player = collision.GetComponent<Player>();
            if(player != null) {
                player.RemoveLife();
                Destroy(this.gameObject);
            }
        }
    }

    void SetDirection(Vector3 direction) {
        this._direction = direction;
    }
    public void SetDirectionUp() {
        SetDirection(Vector3.up);
    }
    public void SetDirectionDown() {
        SetDirection(Vector3.down);
    }

    void SetTag(string tag) {
        this.gameObject.tag = tag;
    }
    public void SetEnemyLaserTag() {
        SetTag("Enemy Laser");
    }
    public void SetPlayerLaserTag() {
        SetTag("Player Laser");
    }

    public void AssignEnemyLaser() {
        _isEnemyLaser = true;
        SetEnemyLaserTag();
        SetDirectionDown();
    }
}
