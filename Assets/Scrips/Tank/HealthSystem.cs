using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    UIManager _UIManager;
    [SerializeField]
    ParticleSystem _smoke;
    [SerializeField]
    private float _maxLife = 100f;

    private float _health;

    TankType _typeOwner = TankType.TankAI1;

    int lastDamageReceive = 0;
    private void Start()
    {
        _health = _maxLife;

        if (GetComponent<PlayerController>() != null)
            _typeOwner = TankType.Player;
        else
            _typeOwner = TankType.TankAI1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && _typeOwner == TankType.Player)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Loose Scene");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && _typeOwner == TankType.TankAI1)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Win Scene");
        }
    }

    public int LastBulletHitted()
    {
        int hitted = lastDamageReceive;
        lastDamageReceive = 0;
        return hitted;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        lastDamageReceive = 1;

        _UIManager.UpdateLife(_typeOwner, _health / _maxLife);

        if (_health <= 0)
        {
            if (_typeOwner == TankType.TankAI1)
            {
                SceneManager.LoadScene("Win Scene");
            }
            else
            {
                SceneManager.LoadScene("Loose Scene");
            }
        }
        else if(_health < _maxLife / 2)
        {
            _smoke.gameObject.SetActive(true);
        }
    }
}
