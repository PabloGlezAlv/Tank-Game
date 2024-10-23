using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    AudioClip _explosionSound;
    [SerializeField]
    GameObject _explosionPrefab;
    [SerializeField]
    float _explosionArea = 2;
    [SerializeField]
    int _damage = 20;

    TankType _typeOwner = TankType.TankAI1;

    bool _explosion = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (_explosion) return;
        //Direct Explosion
        if (collision.gameObject.GetComponentInParent<HealthSystem>() != null)
        {
            collision.gameObject.GetComponentInParent<HealthSystem>().TakeDamage(_damage);
        }

        //Indirect Explosion
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionArea);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponentInParent<HealthSystem>() != null && hitCollider.gameObject != collision.gameObject)
            {
                hitCollider.GetComponentInParent<HealthSystem>().TakeDamage(_damage >> 1);
            }
        }

        GameManager.Instance.TankFired(_typeOwner);
        SoundManager.Instance.PlaySound(_explosionSound);
        Instantiate(_explosionPrefab, transform.position - transform.forward, Quaternion.identity);

        _explosion = true;
        GetComponent<TrailRenderer>().emitting = false;
    }


    public void SetOwner(TankType typeOwner)
    {
        _typeOwner = typeOwner;
    }
}
