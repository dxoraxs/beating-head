using UnityEngine;

namespace DefaultNamespace.Raycaster
{
    public readonly struct RaycastResult
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;

        public RaycastResult(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}