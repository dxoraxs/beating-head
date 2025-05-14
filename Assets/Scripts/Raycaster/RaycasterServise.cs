using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Raycaster;
using UniRx;
using UnityEngine;

public class RaycasterServise : MonoBehaviour
{
    private readonly Subject<RaycastResult> _raycastStream = new();
    [SerializeField] private Camera cam;
    
    public IObservable<RaycastResult> RaycastStream => _raycastStream;

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
                var result = new RaycastResult(hit.point, hit.normal);
                _raycastStream.OnNext(result);
            }
        }
    }
}