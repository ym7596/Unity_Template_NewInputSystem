using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private bool _canMove = false;

    private void OnEnable()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetData()
    {
        
    }

    public void SetClick()
    {
        _meshRenderer.material.color = Color.green;
    }
}
