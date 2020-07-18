using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DepthToMesh : MonoBehaviour
{
    public GameObject outputGameObject;

    public ComputeShader computeShader;
    private int depthToPosKernelIndex;
    private ComputeBuffer posBuffer;
    private float[] rawPosBuffer;
    private Mesh outputMesh;

    private Camera mainCamera;
    private int width;
    private int height;

    // Start is called before the first frame update
    void Start()
    {
        // set camera
        this.mainCamera = Camera.main;

        this.depthToPosKernelIndex = this.computeShader.FindKernel("DepthToPos");
        SetComputeShaderFields();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Check if render target size has changed, if so, fix fields
        if (source.width != width || source.height != height)
        {
            SetComputeShaderFields();
        }
        this.computeShader.SetTexture(this.depthToPosKernelIndex, "Source", source);
        this.computeShader.SetBuffer(this.depthToPosKernelIndex, "Pos", this.posBuffer);
        this.computeShader.Dispatch(this.depthToPosKernelIndex, (this.width + 7) / 8, (this.height + 7) / 8, 1);

        MeshFilter meshFilter = this.outputGameObject.GetComponent<MeshFilter>();

        this.posBuffer.GetData(this.rawPosBuffer);
        for (int i = 0; i < this.rawPosBuffer.Length; i += 4)
        {
            this.outputMesh.vertices[i / 4].Set(this.rawPosBuffer[i], this.rawPosBuffer[i + 1], this.rawPosBuffer[i + 2]);
        }
        Debug.Log(this.rawPosBuffer);

        meshFilter.mesh = this.outputMesh;

        // Throw the source
        Graphics.Blit(source, destination);
    }

    private void SetComputeShaderFields()
    {
        this.outputMesh = new Mesh();
        this.outputMesh.vertices = new Vector3[this.mainCamera.pixelWidth * this.mainCamera.pixelHeight];

        this.posBuffer = new ComputeBuffer(this.mainCamera.pixelWidth * this.mainCamera.pixelHeight, sizeof(float) * 4);
        this.rawPosBuffer = new float[this.posBuffer.count * this.posBuffer.stride];
        this.computeShader.SetFloat("fov_x", this.mainCamera.fieldOfView);
        this.computeShader.SetFloat("fov_y", this.mainCamera.fieldOfView * this.mainCamera.aspect);
        this.computeShader.SetFloat("far", this.mainCamera.farClipPlane);
        this.width = this.mainCamera.pixelWidth;
        this.computeShader.SetFloat("far", this.mainCamera.pixelWidth);
        this.height = this.mainCamera.pixelHeight;
        this.computeShader.SetFloat("far", this.mainCamera.pixelHeight);
    }
}
