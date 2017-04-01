using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class main : MonoBehaviour {
    //publics 
    public bool showFrame=false;
    public Text text;
    public float[] size=new float[2];
    public ComputeShader computeProgram;
    public ComputeShader normalComputeShader;
    public Shader renderProgram;
    public int vertn, vertm;
    public Camera cam;
    //privates
    private ComputeBuffer computeBufferPosition;
    private ComputeBuffer computeBufferVelocity;
    private int computeShaderHandle;
    private int normalComputeShaderHandle;
    private float speed = 200.0f;
    private Material mat;

    private float dx, dy;
    private Vector4[] positions;
    private Vector4[] velocities;
    //values for frame counting
    private float fpsSum=0.0f;
    private int frameNum = 0;
    private int frameStep=0;
    //
    private int vertextSize;
    // Use this for initialization
    void Start () {
        InitCamera();
        InitVertex();
        InitText();
        InitCompute();
        InitMaterial();
	}
    void InitCamera()
    {
        cam.transform.position = new Vector3(3, 2, 5);
        cam.transform.LookAt(new Vector3(0,0,0), new Vector3(0, 1, 0));
    }
    void InitText()
    {
        if (showFrame) {
            text.text = vertextSize+"\n";
        }
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
            positions[i]= new Vector4(0, 0, 0, 0);
            velocities[i]= new Vector4(0, 0, 0, 0);
        }
        for (int i = 0; i < vertn; i++)
        {
            for (int j = 0; j < vertm; j++)
            {
                positions[i * vertm + j].x = dy * j - 0.5f;
                positions[i * vertm + j].y = dx * i - 0.25f;
                positions[i * vertm + j].z = 0.0f;
                positions[i * vertm + j].w = 1.0f;
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
        computeProgram.SetFloat("RestLengthHoritz",dx);
        computeProgram.SetFloat("RestLengthVert", dy);
        computeProgram.SetFloat("RestLengthDiag", Mathf.Sqrt(dx*dx+dy*dy));
        computeProgram.SetFloat("vertn",vertn);
        computeProgram.SetFloat("vertm", vertm);
    }
    void InitMaterial()
    {
        //create material
        mat = new Material(renderProgram);
        //material set buffer
        mat.SetBuffer("Position",computeBufferPosition);
        mat.SetBuffer("Velocity", computeBufferVelocity);
    }

    private void OnPostRender()
    {
        mat.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Triangles, vertextSize/3,1);
    }

    // Update is called once per frame
    void Update () {
        for(int i = 0; i < 1000;i++) { 
            computeProgram.Dispatch(computeShaderHandle,vertn/8, vertm/8, 1);
        }

        frameNum++;
        fpsSum += 1.0f / (Time.deltaTime);
        if (frameNum > 200)
        {
            if (frameStep < 10) { 
                if(showFrame)
                    text.text += "frame = " + fpsSum / 200.0f+"\n";
                frameNum = 0;
                fpsSum = 0;
                frameStep++;
            }
        }
        computeBufferPosition.GetData(positions);
        Debug.Log(positions[0].y);


    }
}
