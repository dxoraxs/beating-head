using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _strength;
    [SerializeField] private int _vibrato;
    private Vector3 _defaultRotation;

    private void Start()
    {
        _defaultRotation = transform.localRotation.eulerAngles;
    }

    public void Shake()
    {
        transform.DOKill();
        transform.DOShakeRotation(_duration, _strength, _vibrato).OnComplete(() =>
        {
            transform.DORotate(_defaultRotation, 180f).SetSpeedBased();
        });
    }
}