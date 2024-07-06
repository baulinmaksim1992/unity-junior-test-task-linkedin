using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] NavMeshAgent _navMeshAgent;
    [SerializeField] LayerMask _obstacleLayer;
    [SerializeField] float _reactionTime;
    [SerializeField] float _chaseTime;
    [SerializeField] float _fireRate;
    [SerializeField] GunController _gun;

    GameManager _gm;
    Transform _player;
    SpotController[] _spots;
    Vector3 _spawnPosition;
    SpotController _currentDestenationSpot;
    Vector3 _lastPlayerSpot;
    Coroutine _chaseCooldownCO;
    Coroutine _fireCO;
    bool _chaseMode;
    bool _chaseCooldownMode;
    bool _isAttacking;


    void Awake()
    {
        _player = FindObjectOfType<PlayerController>().transform;
        _spots = FindObjectsOfType<SpotController>();
        _gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        _spawnPosition = transform.position;
        _currentDestenationSpot = GetRandomSpot();
        StartCoroutine(IsPlayerVisibleCO());
        _fireCO = StartCoroutine(FireCO());
    }

    void Update()
    {
        Move();
        Aim();
    }

    public void SetSpeed(float speed)
    {
        _navMeshAgent.speed = speed;
    }

    void Move()
    {
        if (_currentDestenationSpot == null)
        {
            _currentDestenationSpot = GetRandomSpot();
            _currentDestenationSpot.IsBeasy = true;
        }

        //преследуем игрока
        if (_chaseMode || _chaseCooldownMode)
        {
            _navMeshAgent.SetDestination(_player.position);
        }
        //случайно бродим по карте
        else
        {
            _navMeshAgent.SetDestination(_currentDestenationSpot.transform.position);

            if (Vector3.Distance(transform.position, _currentDestenationSpot.transform.position) < 1.2f)
            {
                var nextPoint = GetRandomSpot();
                nextPoint.IsBeasy = true;

                _currentDestenationSpot.IsBeasy = false;
                _currentDestenationSpot = nextPoint;
            }
        }
    }

    void Aim()
    {
        Vector3 direction = Vector3.zero;

        if (_chaseMode)
        {
            direction = _player.position - transform.position;
        }
        else if (_chaseCooldownMode)
        {
            direction = _lastPlayerSpot - transform.position;
        }
        else if (!_chaseMode && !_chaseCooldownMode && _currentDestenationSpot != null)
        {
            direction = _currentDestenationSpot.transform.position - transform.position;
        }

        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, angle - 90, 0));
    }

    IEnumerator ChaseCooldownCO()
    {
        _chaseCooldownMode = true;
        yield return new WaitForSeconds(_chaseTime);
        _chaseCooldownMode = false;
    }

    IEnumerator FireCO()
    {
        while (true)
        {
            if (_chaseMode)
            {
                _isAttacking = true;
                yield return new WaitForSeconds(1f / _fireRate);
                Fire();
            }
            else
            {
                _isAttacking = false;
                yield return new WaitForSeconds(_reactionTime);
            }
        }
    }

    void Fire()
    {
        _gun.MakeShot();
    }

    IEnumerator IsPlayerVisibleCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(_reactionTime);

            // Направление от врага к игроку
            Vector3 directionToPlayer = _player.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Выполняем RayCast
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, _obstacleLayer))
            {
                if (_chaseMode && !_chaseCooldownMode)
                {
                    if (_chaseCooldownCO != null)
                    {
                        StopCoroutine(_chaseCooldownCO);
                    }

                    _chaseCooldownCO = StartCoroutine(ChaseCooldownCO());
                    _lastPlayerSpot = _player.transform.position;
                }
                // Если RayCast попадает в объект на слое препятствий, игрок не виден
                _chaseMode = false;
            }
            else
            {
                if (_chaseCooldownCO != null)
                {
                    StopCoroutine(_chaseCooldownCO);
                }
                _chaseCooldownMode = false;
                // Если RayCast не попадает в препятствие, игрок виден
                _chaseMode = true;
            }
        }
    }

    SpotController GetRandomSpot()
    {
        var notBeasytSpots = _spots.Where(spot => !spot.IsBeasy).ToArray();
        return notBeasytSpots[Random.Range(0, notBeasytSpots.Count())];
    }

    public void SetSpawnPosition()
    {
        transform.position = _spawnPosition;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            _gm.EnemyGetShotHandler();

            _gm.SpawnedEnemies.Remove(this);

            if (_gm.SpawnedEnemies.Count() == 0)
            {
                _gm.StartNextRaund();
            }

            Destroy(gameObject);
        }
    }
}
