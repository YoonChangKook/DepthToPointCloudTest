using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRenderTexture : MonoBehaviour
{
    public Material Mat;
    public Camera thisCam;

    public void Start()
    {
        //카메라 뎁스맵 활성화 시켜주고
        thisCam = GetComponent<Camera>();
        thisCam.depthTextureMode = DepthTextureMode.Depth;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, Mat);
    }
}
