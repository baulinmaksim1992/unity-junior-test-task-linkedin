using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotController : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [HideInInspector] public bool IsBeasy;

    void Start()
    {
        _meshRenderer.enabled = false;
    }
}
