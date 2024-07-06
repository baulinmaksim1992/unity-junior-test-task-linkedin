using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] protected BulletController _bullet;
    [SerializeField] BulletController _specialBullet;
    [SerializeField] protected Transform _bulletPoint;
    [SerializeField] float _specialCooldownTime;
    UIController _ui;
    protected GameManager _gameManager;
    Coroutine _cooldownCO;
    bool _isSpecialCooldown;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _ui = FindObjectOfType<UIController>();
    }

    public virtual void MakeShot()
    {
        var bullet = Instantiate(_bullet, _bulletPoint.position, Quaternion.identity);
        bullet.Init(transform.right, _gameManager);

        _gameManager.OnGameReset += OnGameResetHandler;
        _gameManager.OnRestart += OnGameResetHandler;
    }

    public void MakeSpecialShot()
    {
        if (_isSpecialCooldown)
        {
            return;
        }

        var bullet = Instantiate(_specialBullet, _bulletPoint.position, Quaternion.identity);
        bullet.Init(transform.right, _gameManager);

        _cooldownCO = StartCoroutine(SpecialFireCO());
    }

    IEnumerator SpecialFireCO()
    {
        _isSpecialCooldown = true;

        _ui.StartCooldownPanelAnimation(_specialCooldownTime);

        yield return new WaitForSeconds(_specialCooldownTime + 1);

        _isSpecialCooldown = false;
    }

    protected void OnGameResetHandler()
    {
        if (_cooldownCO != null)
        {
            StopCoroutine(_cooldownCO);
            _cooldownCO = null;
        }

        _isSpecialCooldown = false;
        _ui.OnGameResetHandler();
    }
}
