using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunController : GunController
{
    public override void MakeShot()
    {
        var bullet = Instantiate(_bullet, _bulletPoint.position, Quaternion.identity);
        bullet.Init(transform.right, _gameManager);

        _gameManager.OnGameReset += OnGameResetHandler;
        _gameManager.OnRestart += OnGameResetHandler;
    }
}
