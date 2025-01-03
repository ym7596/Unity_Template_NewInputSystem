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

    public void SetDefault()
    {
        _meshRenderer.material.color = Color.white;
    }

    public void SetClick(bool isOn)
    {
        if(isOn)
            _meshRenderer.material.color = Color.green;
        else
        {
            _meshRenderer.material.color = Color.red;
        }
    }
}
