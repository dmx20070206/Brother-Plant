using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceHitEffect : MonoBehaviour
{
    private void Start() => Invoke(nameof(DestroySelf), 0.1f);
    private void DestroySelf() => Destroy(gameObject);
}
