using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {
    public ComputeShader computeProgram;
    public ComputeShader normalComputeShader;
    private ComputeBuffer computeBuffer;
    public Shader renderProgram;
    public int width, height;
    private int computeShaderHandle;
    private int normalComputeShaderHandle;
    private float speed = 200.0f;

    struct Data
    {
        Vector3 initPos;
        Vector3 initVel;
        float initTc;
    };
    Data[] particles;
    // Use this for initialization
    void Start () {
        
        particles = new Data[width * height];
        float dx = 1.0f / (width - 1);
        float dy = 1.0f / (width - 1);
        Vector4 p = new Vector4( 0.0f,0.0f,0.0f,1.0f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                        
                //particles[i*height+j]
            }
        }
        computeShaderHandle = computeProgram.FindKernel("CSMain");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
