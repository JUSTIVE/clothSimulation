using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class main : MonoBehaviour {

    //publics 
    public MeshTopology mt;
    public bool showFrame = true;
    public Text text;
    public ComputeShader computeProgram;
    public ComputeShader normalComputeShader;
    public Shader renderProgram;
    public int vertn, vertm;
    public Vector2 ClothSize;
    //privates
    private ComputeBuffer computeBufferPosition;
    private ComputeBuffer computeBufferVelocity;
    private int computeShaderHandle;
    private int normalComputeShaderHandle;
    private Material mat;

    private float dx, dy;
    private Vector4[] positions;
    private Vector4[] velocities;
    //values for write Logfile
    private StreamWriter sw;
    private int logFrame = 0;
    //values for frame counting
    private float fpsSum = 0.0f;
    private int frameNum = 0;
    private int frameStep = 0;
    //camera
    private Vector3 Speed=new Vector3(120.0f,120.0f);
    private float yMinLimit = -20f;
    private float yMaxLimit = 80f;
    private Vector3 mouseDownCoordinate;
    private float camDist;
    private Vector3 mouseCurrentPosition;
    private bool mouseDownFlag = false;
    float x = 0.0f;
    float y = 0.0f;
    //
    private int vertextSize;
    // Use this for initialization
    void Start() {
        initLog();
        InitVertex();
        InitText();
        InitCompute();
        InitMaterial();
    }
    
    void InitText()
    {
        if (showFrame) {
            text.text = vertextSize + "\n";
        }
    }

    void initLog()
    {
        sw = new StreamWriter("Log.txt");
    } 
    void InitVertex()
    {
        vertextSize = vertn * vertm;
        positions = new Vector4[vertextSize];
        velocities = new Vector4[vertextSize];
        dx = 1.0f / (vertn - 1);
        dy = 1.0f / (vertm - 1);
        Vector4 p = new Vector4(0, 0, 0, 1);
        for (int i = 0; i < vertextSize; i++)
        {
            positions[i] = new Vector4(0, 0, 0, 0);
            velocities[i] = new Vector4(0, 0, 0, 0);
        }

        GameObject gb = new GameObject();
        //translate matrix setup
        Matrix4x4 mp = new Matrix4x4();
        mp.SetTRS(new Vector3(-0.5f, 1.0f, 0.0f), Quaternion.Euler(new Vector3(-80.0f, 0.0f, 0.0f)), new Vector3(1.0f,1.0f,1.0f));
        mp.m13 = 0.0f;
        
        for (int i = 0; i < vertm; i++)
        {
            for (int j = 0; j < vertn; j++)
            {
                positions[i * vertm + j].x = dx * j;
                positions[i * vertm + j].y = dy * i;
                positions[i * vertm + j].z = 0.0f;
                positions[i * vertm + j].w = 1.0f;
                positions[i * vertm + j] = mp * positions[i * vertm + j];
            }
        }
    }
    void InitCompute()
    {
        //find kernel
        computeShaderHandle = computeProgram.FindKernel("CSMain");
        //create buffer
        computeBufferVelocity = new ComputeBuffer(vertextSize, 16);
        computeBufferPosition = new ComputeBuffer(vertextSize, 16);
        //buffer set data
        computeBufferPosition.SetData(positions);
        computeBufferVelocity.SetData(velocities);
        //compute shader set buffer
        computeProgram.SetBuffer(computeShaderHandle, "Position", computeBufferPosition);
        computeProgram.SetBuffer(computeShaderHandle, "Velocity", computeBufferVelocity);
        //compute shader set variable
        computeProgram.SetFloat("RestLengthHoriz", dx);
        computeProgram.SetFloat("RestLengthVert", dy);
        computeProgram.SetFloat("RestLengthDiag", Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2)));
        computeProgram.SetFloat("vertn", vertn);
        computeProgram.SetFloat("vertm", vertm);
    }
    void InitMaterial()
    {
        //create material
        mat = new Material(renderProgram);
        //material set buffer
        mat.SetBuffer("Position", computeBufferPosition);
        mat.SetBuffer("Velocity", computeBufferVelocity);
    }

    private void OnPostRender()
    {
        mat.SetPass(0);
        switch (mt)
        {
            case MeshTopology.Points:
                Graphics.DrawProcedural(mt, vertextSize, 1);
                break;
            case MeshTopology.Triangles:
                Graphics.DrawProcedural(mt, vertextSize / 3, 1);
                break;
            default:
                Debug.Log("unhandled Mesh Topology\n");
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        //computeBufferPosition.GetData(positions);
        //if (logFrame < 200) { 
        //    sw.WriteLine(positions[0].x + ","+positions[0].y+","+positions[0].z);
        //    logFrame++;
        //}
        for (int i = 0; i < 500; i++) {
            computeProgram.Dispatch(computeShaderHandle, vertn / 8, vertm / 8, 1);
        }

        frameNum++;
        fpsSum += 1.0f / (Time.deltaTime);
        if (frameNum > 200)
        {
            if (frameStep < 10) {
                if (showFrame)
                    text.text += "frame = " + fpsSum / 200.0f + "\n";
                frameNum = 0;
                fpsSum = 0;
                frameStep++;
            }
        }
        
        //Debug.Log(positions[0].y);

    }
    
   
}


