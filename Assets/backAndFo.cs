using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backAndFo : MonoBehaviour {
    private float weight =1;
	// Use this for initialization
	void Start () {
        this.transform.position = new Vector3(-0.5f,-0.5f,0.9f);
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.z<-1.7f)
        {
            weight = 1;
        }
        if (transform.position.z > 1f)
        {
            weight = -1;
        }
        if(weight==1)
            transform.position = Vector3.Lerp(transform.position, new Vector3(-0.5f, -0.5f, 2f), Time.deltaTime/4);
        else
            transform.position = Vector3.Lerp(transform.position, new Vector3(-0.5f, -0.5f, -2f), Time.deltaTime/4);
        
    }
}
