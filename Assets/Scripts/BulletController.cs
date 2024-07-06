using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _destroyTime;
    [SerializeField] int _maxBounceCount;

    protected Vector3 _direction;
    int _bounceCount;
    GameManager _gm;

    void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    public virtual void Init(Vector3 direction, GameManager gameManager)
    {
        _gm = gameManager;

        _gm.OnGameReset += OnGameResetHandler;
        _gm.OnRestart += OnGameResetHandler;

        _direction = direction;
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        // Проверяем, столкнулась ли пуля со стеной или превысила максимальное количество отскоков
        if (other.gameObject.CompareTag("Wall") || _bounceCount >= _maxBounceCount)
        {
            Destroy(gameObject);
        }

        _direction = Vector3.Reflect(_direction, other.contacts[0].normal);
        _bounceCount++;
    }

    protected void OnGameResetHandler()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        _gm.OnGameReset -= OnGameResetHandler;
        _gm.OnRestart -= OnGameResetHandler;
    }
}
