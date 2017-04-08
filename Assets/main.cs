using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class main : MonoBehaviour {
    //publics 
    public MeshTopology mt;
    public bool showFrame = false;
    public Text text;
    public ComputeShader computeProgram;
    public ComputeShader normalComputeShader;
    public Shader renderProgram;
    public int vertn, vertm;
    public Camera cam;
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
    //values for triangle


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
        InitCamera();
        InitVertex();
        InitText();
        InitCompute();
        InitMaterial();
    }
    void InitCamera()
    {
        
        cam.transform.position = new Vector3(1, 2, 3);
        cam.transform.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        camDist = Vector3.Distance(cam.transform.position, Vector3.zero);
        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;
    }
    void InitText()
    {
        if (showFrame) {
            text.text = vertextSize + "\n";
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
        }
    }

    // Update is called once per frame
    void Update() {
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
        UpdateCamera();
        computeBufferPosition.GetData(positions);
        //Debug.Log(positions[0].y);

    }
    
    void UpdateCamera()
    {
        if (Input.GetMouseButton(0)==true)
        {
            if (!mouseDownFlag) {
                mouseDownFlag = true;
                mouseDownCoordinate = Input.mousePosition;
                Debug.Log("down");
            }
            else {
                x += Input.GetAxis("Mouse X") * Speed.x * camDist * 0.02f;
                y -= Input.GetAxis("Mouse Y") * Speed.y * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                Quaternion rotation = Quaternion.Euler(y, x, 0);

                Vector3 negDist = new Vector3(0.0f, 0.0f, -camDist);
                Vector3 position = rotation *negDist;

                transform.rotation = rotation;
                transform.position = position;
            }
        }
        else
        {
            mouseDownFlag = false;
            Debug.Log("up");
        }
        cam.transform.LookAt(Vector3.zero);
            
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}


