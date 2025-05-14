using Cysharp.Threading.Tasks;
using DG.Tweening;
using MathPlus;
using UnityEngine;

public class FistContainer : MonoBehaviour
{
    [SerializeField] private TrailRenderer[] _trailRenderer;

    public void DisableTrail()
    {
        foreach (var trailRenderer in _trailRenderer)
        {
            trailRenderer.transform.SetParent(null);
            Destroy(trailRenderer.gameObject, trailRenderer.time);
        }
    }

    [ContextMenu("Find Trails")]
    private void FindTrails()
    {
        _trailRenderer = GetComponentsInChildren<TrailRenderer>();
    }
}