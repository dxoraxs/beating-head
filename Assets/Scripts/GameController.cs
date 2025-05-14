using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Raycaster;
using UniRx;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private RaycasterServise _raycaster;
        [SerializeField] private SegmentedMeshFollower _head;
        [SerializeField] private HitFXController _fxController;
        [SerializeField] private ParabolaMover _fistMover;
        [SerializeField] private DecalController _decalController;
        [SerializeField] private CameraShaker _cameraShaker;
        [SerializeField] private float _forceImpulse;

        private void Start()
        {
            GameLogic().Forget();
        }

        private async UniTask GameLogic()
        {
            var result = await _raycaster.RaycastStream.First().ToUniTask();
            WaitAnimate(result).Forget();
            GameLogic().Forget();
        }

        private async UniTask WaitAnimate(RaycastResult hit)
        {
            var directionToCenter = GetDirectionToCenter(hit.Position);

            await _fistMover.LaunchTo(hit.Position, directionToCenter);

            _cameraShaker.Shake();
            _fxController.OnHit(hit);
            _decalController.OnHit(hit);

            var force = directionToCenter * -_forceImpulse;
            _head.ApplyImpulse(force);
        }

        private Vector3 GetDirectionToCenter(Vector3 hitPoint)
        {
            var vector3 = _head.transform.position - hitPoint;
            return vector3.normalized;
        }
    }
}