using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HitFXController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitFX;
    [SerializeField] private ParticleSystem _starFX;
    [SerializeField] private ParticleSystem _bloodFX;
    [SerializeField] private int _countOfPunchBlood;
    [SerializeField] private float _delayToDisableStars = 2;
    [SerializeField] private int _countOfPunchStars = 3;
    private int _counterPunchs = 0;
    private Tween _tween;

    public void OnHit(Vector3 hitPoint, Vector3 normal)
    {
        _counterPunchs++;

        var currentHitVFX = _counterPunchs % _countOfPunchBlood == 0 ? _bloodFX : _hitFX;
        SpawnHitFX(currentHitVFX, hitPoint, normal);
        TryEnableStarFX();
    }

    private void SpawnHitFX(ParticleSystem vfx, Vector3 hitPoint, Vector3 normal)
    {
        var newVfx = Instantiate(vfx, hitPoint, Quaternion.identity);
        newVfx.Play();
        newVfx.transform.forward = normal;
    }

    private void TryEnableStarFX()
    {
        if (_counterPunchs % _countOfPunchStars == 0)
        {
            _starFX.Play();
            if (_tween != null)
            {
                _tween.Kill();
            }
            _tween = DOVirtual.DelayedCall(_delayToDisableStars, () =>
            {
                _starFX.Stop();
                _tween = null;
            });
        }
    }
}
