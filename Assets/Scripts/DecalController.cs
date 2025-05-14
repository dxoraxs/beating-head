using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalController : MonoBehaviour
{
    [SerializeField] private SegmentedMeshFollower segmentedMeshFollower;
    [SerializeField] private DecalProjector _prefab;
    [SerializeField] private float _fadeValue = .5f;
    [SerializeField] private float _unfadeTime = .2f;
    [SerializeField] private float _fadeTime = 5;
    [SerializeField] private float _idleTime = 5;

    public void OnHit(Vector3 position, Vector3 normal)
    {
        var decalParent = segmentedMeshFollower.GetBoneByYPosition(position.y);
        var newDecal = Instantiate(_prefab, position, Quaternion.identity, decalParent);
        newDecal.fadeFactor = 0;
        newDecal.transform.forward = normal;

        DOVirtual.DelayedCall(.2f, () =>
        {
            DOVirtual.Float(0, _fadeValue, _unfadeTime, value => newDecal.fadeFactor = value);

            DOVirtual.DelayedCall(_idleTime, () =>
            {
                DOVirtual.Float(_fadeValue, 0, _fadeTime, value => newDecal.fadeFactor = value).OnComplete(() =>
                {
                    Destroy(newDecal.gameObject);
                });
            });
        });
    }
}
