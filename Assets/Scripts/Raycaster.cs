using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    [SerializeField] private SegmentedMeshFollower deformer;
    [SerializeField] private HitFXController _fxController;
    [SerializeField] private ParabolaMover mover;
    [SerializeField] private DecalController decalController;
    [SerializeField] private Camera cam;
    [SerializeField] private CameraShaker _cameraShaker;
    [SerializeField] private float _forceImpulse;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                WaitAnimate(hit).Forget();
            }
        }
    }

    private async UniTask WaitAnimate(RaycastHit hit)
    {
        var directionToCenter = GetDirectionToCenter(hit.point);
        
        await mover.LaunchTo(hit.point, directionToCenter);
        
        _cameraShaker.Shake();
        _fxController.OnHit(hit.point, hit.normal);
        decalController.OnHit(hit.point, hit.normal);

        Vector3 force = directionToCenter * -_forceImpulse;
        deformer.ApplyImpulse(force);
    }

    private Vector3 GetDirectionToCenter(Vector3 hitPoint)
    {
        var vector3 = deformer.transform.position - hitPoint;
        return vector3.normalized;
    }
}