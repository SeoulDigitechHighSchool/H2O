using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinedMeshColider : MonoBehaviour {

    private void Awake()
    {
         gameObject.GetComponent<SkinnedMeshRenderer>().BakeMesh(gameObject.GetComponent<MeshCollider>().sharedMesh);
    }
}
