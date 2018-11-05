using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanarShadowCaster : MonoBehaviour {

    [SerializeField]
    private Transform reciever;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Shader.SetGlobalMatrix("_World2Ground", reciever.worldToLocalMatrix);
        Shader.SetGlobalMatrix("_Ground2World", reciever.localToWorldMatrix);
    }
}
