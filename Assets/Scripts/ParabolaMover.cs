using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MathPlus;
using UnityEngine;

public class ParabolaMover : MonoBehaviour
{
    [SerializeField] private FistContainer _projectile;
    [SerializeField] private float _flightDuration = 1.5f;
    [SerializeField] private float _arcHeight = 2f;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = _projectile.transform.position;
    }

    public async UniTask LaunchTo(Vector3 targetPosition, Vector3 targetNormal)
    {
        var newProjectile = Instantiate(_projectile, _startPosition, Quaternion.identity);
        await MoveParabola(newProjectile.transform, targetPosition, targetNormal, _flightDuration);
        newProjectile.DisableTrail();
        newProjectile.transform.DOMove(_startPosition, _flightDuration).OnComplete(() =>
        {
            Destroy(newProjectile.gameObject);
        });
    }

    private async UniTask  MoveParabola(Transform obj, Vector3 targetPos, Vector3 targetNormal, float duration)
    {
        var middlePoint = (obj.position + targetPos) / 2 + targetNormal * _arcHeight;
        var parabola = mathPlus.Vector3Parabola(obj.position, middlePoint, targetPos, 10);
        
        await obj.DOPath(parabola, duration).SetEase(Ease.InCubic).SetLookAt(0.1f).ToUniTask();
    }

}