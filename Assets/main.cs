﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class main : MonoBehaviour {

    //publics 
    public MeshTopology meshTopology;
    public enum Mode{Hang,FreeDrop};
    public Mode mode;
    public bool showFrame = true;
    public Text text;
    public ComputeShader computeProgram;
    public ComputeShader normalComputeShader;
    public Shader renderProgram;
    public int vertn, vertm;
    public Vector2 ClothSize;
    public GameObject Satellite;

    //privates
    private Mesh mesh;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer computeBufferPosition;
    private ComputeBuffer computeBufferVelocity;
    private int computeShaderHandleHang;
    private int computeShaderHandleFreeDrop;
    private int normalComputeShaderHandle;
    private Material mat;
    private float dx, dy;
    private Vector4[] positions;
    private Vector4[] velocities;
    //values for write Logfile
    private StreamWriter streamWriter;
    private int logFrame = 0;
    //values for frame counting
    private float fpsSum = 0.0f;
    private int frameNum = 0;
    private int frameStep = 0;
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
        streamWriter = new StreamWriter("Log.txt");
    } 
    void InitVertex()
    {
        
        vertextSize = vertn * vertm;
        positions = new Vector4[vertextSize];
        velocities = new Vector4[vertextSize];
        //distance between particles
        dx = ClothSize.x / (vertn - 1);
        dy = ClothSize.y / (vertm - 1);
        //setup initial datas
        Vector4 p = new Vector4(0, 0, 0, 1);
        for (int i = 0; i < vertextSize; i++)
        {
            positions[i] = new Vector4(0, 0, 0, 0);
            velocities[i] = new Vector4(0, 0, 0, 0);
        }
        //translate matrix setup
        Matrix4x4 mp = new Matrix4x4();
        mp.SetTRS(new Vector3(-1.0f, 1.0f, 0.5f), Quaternion.Euler(new Vector3(-80.0f, 0.0f, 0.0f)), new Vector3(1.0f,1.0f,1.0f));
        mp.m13 = 0.0f;
        //setup initial vertex positions
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
        //setupMesh
        mesh = new Mesh();
        mesh.vertices = new Vector3[vertextSize];
        for(int i = 0; i < vertextSize; i++)
        {
            mesh.vertices[i] = new Vector3(positions[i].x, positions[i].y, positions[i].z);
        }

        //setup for triangle
        triangleBuffer = new ComputeBuffer(vertextSize/3,12);
        //triangleBuffer.SetData


    }
    void InitCompute()
    {
        //find kernel
        computeShaderHandleHang = computeProgram.FindKernel("HANG");
        computeShaderHandleFreeDrop = computeProgram.FindKernel("FREEDROP");
        //create buffer
        computeBufferVelocity = new ComputeBuffer(vertextSize, 16);
        computeBufferPosition = new ComputeBuffer(vertextSize, 16);
        //buffer set data
        computeBufferPosition.SetData(positions);
        computeBufferVelocity.SetData(velocities);
        //compute shader set buffer
        if (mode == Mode.Hang) { 
            computeProgram.SetBuffer(computeShaderHandleHang, "Position", computeBufferPosition);
            computeProgram.SetBuffer(computeShaderHandleHang, "Velocity", computeBufferVelocity);
        }
        else
        {
            computeProgram.SetBuffer(computeShaderHandleFreeDrop, "Position", computeBufferPosition);
            computeProgram.SetBuffer(computeShaderHandleFreeDrop, "Velocity", computeBufferVelocity);
        }
        //compute shader set variable
        computeProgram.SetVector("sphere1", new Vector4(0,-1.1f,-0.5f,1.0f));
        computeProgram.SetVector("sphere2", new Vector4(0,-0.329f,-0.5f,1.0f));
        
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

    private void OnRenderObject()
    {
        //set Current Material 
        mat.SetPass(0);
        switch (meshTopology)
        {
            case MeshTopology.Points:
                Graphics.DrawProcedural(meshTopology, vertextSize, 1);
                break;
            case MeshTopology.Triangles:
                Graphics.DrawProcedural(meshTopology, vertextSize / 3, 1);
                break;
            default:
                Debug.Log("unhandled Mesh Topology\n");
                break;
        }
        //Graphics.DrawMesh()
    }

    // Update is called once per frame
    void Update() {
        computeProgram.SetVector("sphere3", new Vector4(Satellite.transform.position.x, Satellite.transform.position.y, Satellite.transform.position.z, 1.0f));
        if (mode == Mode.Hang)
            for (int i = 0; i < 500; i++) {

                computeProgram.Dispatch(computeShaderHandleHang, vertn / 8, vertm / 8, 1);
            }
        else
            for (int i = 0; i < 500; i++)
            {
                
                computeProgram.Dispatch(computeShaderHandleFreeDrop, vertn / 8, vertm / 8, 1);
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
    }
}


