using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// This class only reads the mesh vertices position in the world coordinate.
// RenderTextureFormat
// size: 512x512
// color format: R16G16B16A16_SFLOAT
// depth buffer: no depth buffer
public class SImpleSkinnedMeshBaker : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer m_skinnedMesh = null;
    [SerializeField] RenderTexture m_positionMap;
    [SerializeField] ComputeShader m_computeShader;

    private ComputeBuffer m_positionBuffer;
    private RenderTexture m_tempPositionMap;
    private int m_vertexCountID;
    private int m_transformID;
    private int m_positionBufferID;
    private int m_positionMapID;

    private Mesh m_mesh = null;
    private List<Vector3> m_positionList = new List<Vector3>();
    private List<Vector3> m_normalList = new List<Vector3>();

    void Start()
    {
        Init();
    }

    void Update()
    {
        GetMeshInfos();
        Bake();
    }

    void OnDestroy()
    {
        Destroy(m_mesh);
        m_mesh = null;

        m_positionBuffer.Dispose();

        Destroy(m_tempPositionMap);

        m_positionBuffer = null;
        m_tempPositionMap = null;
    }

    private void Init()
    {
        m_mesh = new Mesh();

        m_vertexCountID = Shader.PropertyToID("VertexCount");
        m_transformID = Shader.PropertyToID("Transform");
        m_positionBufferID = Shader.PropertyToID("PositionBuffer");
        m_positionMapID = Shader.PropertyToID("PositionMap");

        GetMeshInfos();

        var vertexCount = m_positionList.Count;
        m_positionBuffer = new ComputeBuffer(vertexCount * 3, sizeof(float));
        m_tempPositionMap = new RenderTexture(m_positionMap.width, m_positionMap.height, 0, RenderTextureFormat.ARGBHalf);
        m_tempPositionMap.enableRandomWrite = true;
        m_tempPositionMap.Create();
    }

    // getting the vertices and normals in realtime, but normals are for the debug.
    private void GetMeshInfos()
    {
        m_skinnedMesh.BakeMesh(m_mesh);
        m_mesh.GetVertices(m_positionList);
        m_mesh.GetVertices(m_normalList);
    }

    // bake the vertices to the rendertexture and pass to the compute shader
    private void Bake()
    {
        var w = m_positionMap.width;
        var h = m_positionMap.height;

        var parent = m_skinnedMesh.transform.parent;
        Matrix4x4 m = Matrix4x4.TRS(parent.position, parent.rotation, Vector3.one);

        for (int i = 0; i < m_positionList.Count; i++)
        {
            Vector3 pos = m.MultiplyPoint3x4(m_positionList[i]);
            Debug.DrawLine(pos, pos + m_normalList[i] * 0.01f, Color.red);
        }

        m_computeShader.SetInt(m_vertexCountID, m_positionList.Count);
        m_computeShader.SetMatrix(m_transformID, m);
        
        m_positionBuffer.SetData(m_positionList);

        m_computeShader.SetBuffer(0, m_positionBufferID, m_positionBuffer);
        m_computeShader.SetTexture(0, m_positionMapID, m_tempPositionMap);

        m_computeShader.Dispatch(0, w / 8, h / 8, 1);

        Graphics.CopyTexture(m_tempPositionMap, m_positionMap);
    }
}
