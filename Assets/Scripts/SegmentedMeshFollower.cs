using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SegmentedMeshFollower : MonoBehaviour
{
    [Serializable]
    public class Bone
    {
        public Transform transform;
        [HideInInspector] public float localY;
        [HideInInspector] public Quaternion originalLocalRotation;
        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 currentOffset;
    }

    [SerializeField] private List<Bone> bones = new();
    [SerializeField] private float stiffness = 10f;
    [SerializeField] private float damping = 1f;
    [SerializeField] private float maxAngle = 30f;


    private Mesh _mesh;
    private Vector3[] _originalVertices;
    private Vector3[] _deformedVertices;

    private int[] _boneIndexPerVertex;
    private Vector3[] _localPositionToBoneSpace;

    private void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh = Instantiate(GetComponent<MeshFilter>().sharedMesh);
        _originalVertices = _mesh.vertices;
        _deformedVertices = new Vector3[_originalVertices.Length];

        _boneIndexPerVertex = new int[_originalVertices.Length];
        _localPositionToBoneSpace = new Vector3[_originalVertices.Length];

        // Сортировка костей снизу вверх
        bones.Sort((a, b) =>
        {
            var ay = transform.InverseTransformPoint(a.transform.position).y;
            var by = transform.InverseTransformPoint(b.transform.position).y;
            return ay.CompareTo(by);
        });

        foreach (var bone in bones)
        {
            bone.localY = transform.InverseTransformPoint(bone.transform.position).y;
            bone.originalLocalRotation = bone.transform.localRotation;
        }

        // Назначаем каждой вершине кость
        for (var i = 0; i < _originalVertices.Length; i++)
        {
            var y = _originalVertices[i].y;
            var boneIndex = FindBoneIndex(y);
            _boneIndexPerVertex[i] = boneIndex;

            // Сохраняем смещение от кости в её локальных координатах
            _localPositionToBoneSpace[i] =
                bones[boneIndex].transform.InverseTransformPoint(transform.TransformPoint(_originalVertices[i]));
        }
    }
    
    public void ApplyImpulse(Vector3 worldForce)
    {
        var localForce = transform.InverseTransformDirection(worldForce);
        var impactDirection = new Vector3(localForce.x, 0, localForce.z);

        for (var i = 0; i < bones.Count; i++)
        {
            var verticalWeight = (float)i / (bones.Count - 1); // нижняя кость 0, верхняя 1
            var intensity = 1f - verticalWeight; // нижние кости получают больше

            // Поворот либо по X, либо по Z в зависимости от направления
            var impulseOffset = new Vector3(-impactDirection.z, 0, impactDirection.x) * intensity * 10f;

            bones[i].velocity += impulseOffset;
        }
    }

    public Transform GetBoneByYPosition(float yPosition)
    {
        return bones[FindBoneIndex(yPosition)].transform;
    }

    private int FindBoneIndex(float y)
    {
        if (bones[0].localY > y)
        {
            return 0;
        }
        
        for (var i = 0; i < bones.Count - 1; i++)
        {
            if (y >= bones[i].localY && y < bones[i + 1].localY)
                return i;
        }
        return bones.Count - 1;
    }

    private void LateUpdate()
    {
        foreach (var bone in bones)
        {
            var force = -bone.currentOffset * stiffness - bone.velocity * damping;
            bone.velocity += force * Time.deltaTime;
            bone.currentOffset += bone.velocity * Time.deltaTime;

            var clampedOffset = Vector3.ClampMagnitude(bone.currentOffset, maxAngle);
            var swing = Quaternion.Euler(clampedOffset);
            bone.transform.localRotation = bone.originalLocalRotation * swing;
        }
        
        for (var i = 0; i < _originalVertices.Length; i++)
        {
            var boneIndex = _boneIndexPerVertex[i];
            var bone = bones[boneIndex].transform;

            var worldPos = bone.TransformPoint(_localPositionToBoneSpace[i]);
            _deformedVertices[i] = transform.InverseTransformPoint(worldPos);
        }

        _mesh.vertices = _deformedVertices;
        _mesh.RecalculateNormals();
    }
}
