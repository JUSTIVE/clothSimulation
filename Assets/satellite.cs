using UnityEngine;
using System.Collections;

public class satellite : MonoBehaviour {

    public GameObject MotherPlanet;
    public Vector3 axis = Vector3.up;
    public float radius=0.2f;
    public Vector3 desiredPosition;
    public float radiusSpeed=0.05f;
    public float rotationSpeed=10.0f;
    private Transform center;
    // Use this for initialization
    void Start () {
        center = MotherPlanet.transform;
        //transform.position = (transform.position - center.position).normalized * radius + center.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
