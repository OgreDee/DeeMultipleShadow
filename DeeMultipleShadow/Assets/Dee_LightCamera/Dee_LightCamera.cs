using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dee_LightCamera : MonoBehaviour {
    [SerializeField]
    Light directionalLight;
    [SerializeField]
    Shader casterShader;
    [SerializeField]
    bool debug;
    [SerializeField]
    LayerMask layer;

    Camera lightCamera;



    void buildLightCamera()
    {
        if (directionalLight.type != LightType.Directional)
            return;

        GameObject cameraObj = new GameObject("_lightCamera_");
        cameraObj.hideFlags = HideFlags.DontSave;
        cameraObj.transform.SetPositionAndRotation(directionalLight.transform.position, directionalLight.transform.rotation);

        lightCamera = cameraObj.AddComponent<Camera>();
        lightCamera.backgroundColor = Color.clear;
        lightCamera.clearFlags = CameraClearFlags.SolidColor;
        lightCamera.cullingMask = layer.value;
        lightCamera.allowHDR = false;
        lightCamera.allowMSAA = false;
        lightCamera.orthographic = true;

        lightCamera.farClipPlane = 100;

        lightCamera.enabled = false;

        lightCamera.SetReplacementShader(casterShader, "");
    }

    private void OnDestroy()
    {
        if(lightCamera != null)
        {
            Destroy(lightCamera.gameObject);
        }
    }

    private void OnPreRender()
    {
        if(lightCamera == null)
        {
            buildLightCamera();
            lightCamera.targetTexture = AllocShadowMap();
        }

        lightCamera.Render();

        Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(lightCamera.projectionMatrix, false);
        Shader.SetGlobalMatrix("_gWorldToShadow", projectionMatrix * lightCamera.worldToCameraMatrix);
    }


    RenderTexture shadowMapTexture;
    RenderTexture AllocShadowMap()
    {
        if(shadowMapTexture != null)
        {
            shadowMapTexture.width = 512;
            shadowMapTexture.height = 512;
            shadowMapTexture.depth = 16;
        }
        else
        {
            shadowMapTexture = new RenderTexture(512, 512, 16);
        }

        Shader.SetGlobalTexture("_gShadowMapTexture", shadowMapTexture);
        Shader.SetGlobalFloat("_gShadowStrength", 1);

        return shadowMapTexture;
    }
}
