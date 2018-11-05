using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeeShadowCamera : MonoBehaviour {

    public Renderer target;
    public Light lightTrans;
    public bool useSoftShadow;

    Camera _shadowCamera;
    Transform targetTrans;

    // Use this for initialization
    void Start()
    {
        targetTrans = target.transform;
        GameObject cameraObject = new GameObject();
        cameraObject.name = "Light Camera";
        cameraObject.transform.SetParent(lightTrans.transform);
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localRotation = Quaternion.identity;

        _shadowCamera = cameraObject.AddComponent<Camera>();
        _shadowCamera.clearFlags = CameraClearFlags.Color;
        _shadowCamera.backgroundColor = Color.black;
        _shadowCamera.orthographic = true;

        _shadowCamera.cullingMask = 1<<target.gameObject.layer;

        _shadowCamera.enabled = false;
        lightTrans.enabled = false;

        Camera viewCamera = GetComponent<Camera>();
        SetFitToScene(_shadowCamera);


        _shadowCamera.SetReplacementShader(Shader.Find("Dee/Caster"), "");

        Shader.SetGlobalFloat("_gShadowStrength", 0.5f);

        AllocateTexture();
    }

    List<Vector3> GetSceneBoundVertexsBounds(Renderer render, Bounds bounds)
    {
        List<Vector3> vertexs = new List<Vector3>();

        Matrix4x4 trans = targetTrans.localToWorldMatrix;

        vertexs.Add(bounds.min);
        vertexs.Add(bounds.min + new Vector3(bounds.extents.x, 0, 0));
        vertexs.Add(bounds.min + new Vector3(0, bounds.extents.y, 0));
        vertexs.Add(bounds.min + new Vector3(bounds.extents.x, bounds.extents.y, 0));

        vertexs.Add(bounds.max);
        vertexs.Add(bounds.max - new Vector3(bounds.extents.x, 0, 0));
        vertexs.Add(bounds.max - new Vector3(0, bounds.extents.y, 0));
        vertexs.Add(bounds.max - new Vector3(bounds.extents.x, bounds.extents.y, 0));

        return vertexs;
    }

    RoninBound3D ConvertToBound(Matrix4x4 trans, List<Vector3> sceneBoundVertexs)
    {
        RoninBound3D bound = new RoninBound3D();
        sceneBoundVertexs.ForEach(vertex => bound.Update(trans.MultiplyPoint(vertex)));
        return bound;
    }

    void SetFitToScene(Camera lightCamera, GameObject i_light, List<Vector3> sceneBoundVertexs)
    {
        RoninBound3D bound = ConvertToBound(i_light.transform.worldToLocalMatrix, sceneBoundVertexs);

        lightCamera.transform.localPosition = new Vector3(bound.xCenter, bound.yCenter, bound.zMin <= 0.1f ? bound.zMin - 0.1f : 0);
        lightCamera.orthographicSize = Mathf.Max(bound.xSize / 2, bound.ySize / 2);
        lightCamera.nearClipPlane = bound.zMin - lightCamera.transform.localPosition.z;
        lightCamera.farClipPlane = bound.zMax - lightCamera.transform.localPosition.z;
    }

    void OnDisable()
    {
        if (_shadowCamera != null)
            _shadowCamera.targetTexture = null;
        DestroyImmediate(_shadowCamera);
        DestroyImmediate(_shadowTexture);
        _shadowTexture = null;
    }

    private void SetFitToScene(Camera _lightCamera)
    {
        List<Vector3> sceneBoundVertexs = GetSceneBoundVertexsBounds(target,
              target.bounds);

        SetFitToScene(_lightCamera, lightTrans.gameObject, sceneBoundVertexs);
    }

    void OnPreCull()
    {
        if(_shadowCamera != null)
        {
            if ((int)TextureSize != _shadowTexture.width)
            {
                AllocateNewTexture();
            }

            SetFitToScene(_shadowCamera);
            _shadowCamera.Render();


            Shader.SetGlobalTexture("_gShadowMapTexture", _shadowTexture);
            Shader.SetGlobalColor("_gSunColor", lightTrans.color * lightTrans.intensity);

            //soft shadow
            if (useSoftShadow)
            {
                Shader.EnableKeyword("USESOFTSHADOW");
            }
            else
            {
                Shader.DisableKeyword("USESOFTSHADOW");
            }
            Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(_shadowCamera.projectionMatrix, false);
            Shader.SetGlobalMatrix("_gWorldToShadow", projectionMatrix * _shadowCamera.worldToCameraMatrix);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    private RenderTexture _shadowTexture;
    public enum Dimension
    {
        x256 = 256,
        x512 = 512,
        x1024 = 1024,
        x2048 = 2048,
    }

    [Space] public Dimension TextureSize = Dimension.x512;

    void AllocateTexture()
    {
        _shadowTexture = new RenderTexture((int) TextureSize, (int) TextureSize, 24, RenderTextureFormat.ARGB32);
        _shadowTexture.filterMode = FilterMode.Bilinear;
        _shadowTexture.useMipMap = false;
        _shadowTexture.autoGenerateMips = false;

        _shadowCamera.targetTexture = _shadowTexture;

        _shadowTexture.name = "dee shadowmap";
    }

    void AllocateNewTexture()
    {
        _shadowCamera.targetTexture = null;
        DestroyImmediate(_shadowTexture);

        _shadowTexture = new RenderTexture((int)TextureSize, (int)TextureSize, 24, RenderTextureFormat.ARGB32);
        _shadowTexture.filterMode = FilterMode.Bilinear;
        _shadowTexture.useMipMap = false;
        _shadowTexture.autoGenerateMips = false;
        _shadowCamera.targetTexture = _shadowTexture;

        _shadowTexture.name = "dee shadowmap";
    }
}
