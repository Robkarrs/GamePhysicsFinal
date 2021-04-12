using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSimulation : MonoBehaviour
{
    
    /// //////////////////////// FUNCTIONS NOT USED ANYMORE
    Rigidbody rb;
    public float CRCoeff;
    Vector3 currVelocity;
    Vector3[] Impulses;

    void OnCollisionEnter(Collision C)
    {
        //Resolve Collisions
        ContactPoint contact = C.GetContact(0);
        Vector3 pos = contact.point;
        Vector3 norm = contact.normal;
        float dist = contact.separation;
        float m1 = rb.mass;
        Vector3 arm1 = pos - transform.position;

        if (C.gameObject.tag == "Wall")
        {
            //Debug.Log("Hit a Wall");
            float j = (1 + CRCoeff) * Vector3.Dot(C.relativeVelocity, norm) / (1/m1);
            Vector3 impulse = norm * (j / m1);
            //Debug.Log(impulse);
            rb.AddForce(impulse, ForceMode.Impulse);
        }


    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
