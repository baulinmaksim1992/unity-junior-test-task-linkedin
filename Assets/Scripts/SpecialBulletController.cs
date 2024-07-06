using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBulletController : BulletController
{
    protected override void OnCollisionEnter(Collision other)
    {
        // Проверяем, столкнулась ли пуля со стеной
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        _direction = Vector3.Reflect(_direction, other.contacts[0].normal);
    }
}
