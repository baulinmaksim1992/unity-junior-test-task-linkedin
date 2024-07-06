using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] GunController _gun;
    [SerializeField] float _moveSpeed;
    [SerializeField] GameManager _gm;
    [SerializeField] float _rotationCheckAngle;
    [SerializeField] float _rotationShootWindow;
    [SerializeField] float _doubleShotTime;

    Vector3 _spawnPosition;
    Vector3 _moveInput;
    bool _canWalk;
    float _lastAngle;

    Coroutine _doubleShotCooldownCO;
    bool _doubleShotCooldownIsActive;
    Quaternion _lastRotation;

    void Awake()
    {
        _spawnPosition = new Vector3(26, 0, 0);
        _lastRotation = transform.rotation;
    }

    void Start()
    {
        _canWalk = true;
    }

    void Update()
    {
        Aim();
        Move();
        CalculateRotation();
    }

    void CalculateRotation()
    {
        Vector3 currentEulerAngles = transform.eulerAngles;
        Vector3 lastEulerAngles = _lastRotation.eulerAngles;

        // Рассчитываем разницу углов по оси Y
        float deltaY = currentEulerAngles.y - lastEulerAngles.y;

        // Обработка перехода через 360 градусов
        if (deltaY > 180f)
        {
            deltaY -= 360f;
        }
        else if (deltaY < -180f)
        {
            deltaY += 360f;
        }

        _lastAngle += deltaY;

        if (_lastAngle > 5000 || _lastAngle < -5000)
        {
            _lastAngle = 300;
        }

        _lastRotation = transform.rotation;
    }

    void CheckRotationShot()
    {
        if (_lastAngle >= _rotationCheckAngle || _lastAngle <= -_rotationCheckAngle)
        {
            _gm.AddRotateAndShotStat();
        }

        _lastAngle = 0;
    }

    IEnumerator DoubleShotCooldwonCO()
    {
        _doubleShotCooldownIsActive = true;
        yield return new WaitForSeconds(_doubleShotTime);
        _doubleShotCooldownIsActive = false;
    }

    void CheckDoubleShot()
    {
        if (_doubleShotCooldownIsActive)
        {
            if (_doubleShotCooldownCO != null)
            {
                StopCoroutine(_doubleShotCooldownCO);
                _doubleShotCooldownCO = null;
            }

            _doubleShotCooldownIsActive = false;
            _gm.AddDoubleShotStat();
        }
        else
        {
            if (_doubleShotCooldownCO != null)
            {
                StopCoroutine(_doubleShotCooldownCO);
            }

            _doubleShotCooldownCO = StartCoroutine(DoubleShotCooldwonCO());
        }
    }

    void OnMove(InputValue value)
    {
        var vector2 = value.Get<Vector2>();
        _moveInput = new Vector3(vector2.x, 0f, vector2.y);
    }

    void Move()
    {
        if (_canWalk)
        {
            transform.Translate(_moveInput * _moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void Aim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.y = transform.position.y;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, angle - 90, 0));
    }

    void OnFire(InputValue value)
    {
        _gun.MakeShot();
        UpdateStats();
    }

    void OnSpecialFire(InputValue value)
    {
        _gun.MakeSpecialShot();
        UpdateStats();
    }

    void UpdateStats()
    {
        CheckDoubleShot();
        CheckRotationShot();
    }

    void OnReset(InputValue value)
    {
        Debug.Log("New game started");
        _canWalk = false;
        _gm.ResetGame();
        _canWalk = true;
    }

    public void SetSpawnPosition()
    {
        transform.position = _spawnPosition;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            _gm.PlayerGetShotHandler();
        }
    }
}
