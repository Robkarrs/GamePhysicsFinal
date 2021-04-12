using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FluidSimulation : MonoBehaviour
{
    public int partCount;
    public GameObject particle;
    public float h;
    public float rho0;
    public float k;
    public float mu;
    public float sigma;
    public float nthreshold;
    GameObject[] particles;
    Vector3[] a;
    Vector3[] f;
    float[] rho;
    float[] dens;
    float[] p;
    public float size;
    public int height;
    public int side;

    // Start is called before the first frame update
    void Start()
    {
        partCount = side * side * height;
        a = new Vector3[partCount];
        f = new Vector3[partCount];
        rho = new float[partCount];
        dens = new float[partCount];
        p = new float[partCount];
        particles = new GameObject[partCount];

        // Initialize Fluid particles
        Vector3 pos;
        //float side = Mathf.Sqrt(partCount);
        float stepSize = size / side;
        float start = size / 2;
        int i = 0;
        for (int x = 0; x < side; x++)
        {
            for (int z = 0; z < side; z++)
            {
                for (int w = 0; w < height; w++)
                {
                    pos = new Vector3(-start + x * stepSize, 4 + w * stepSize, -start + z * stepSize);
                    //Debug.Log(pos);
                    particles[i] = Instantiate(particle, pos, Quaternion.identity, transform);
                    i++;
                }
            }

        }
        // Initialize densities
        for (int j = 0; j < partCount; j++)
        {
            calcDensity(j);
        }
    }

    // Update is called once per frame
    void Update()
    {        
        // Main loop to calculate acceleration
        for(int i = 0; i < partCount; i++)
        {

            a[i] = new Vector3(0, 0, 0); rho[i] = 0; f[i] = new Vector3(0,0,0);
            calcAcceleration(i);
            particles[i].GetComponent<Rigidbody>().AddForce(a[i], ForceMode.Acceleration);
        }
        // Exit scene
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

    }

    private void calcDensity(int i)
    {
        for(int j = 0; j < partCount; j++)
        {
            if (j != i)
            {
                float m = particles[j].GetComponent<Rigidbody>().mass;
                Vector3 r = particles[i].transform.position - particles[j].transform.position;
                if (r.sqrMagnitude < h * h)
                {
                    rho[i] += m * 315 / (64 * Mathf.PI * Mathf.Pow(h, 9)) * (Mathf.Pow((Mathf.Max(h * h - Mathf.Pow(r.sqrMagnitude, 2), 0)), 3));
                }
            }
        }
        p[i] = k * (rho[i] - rho0);
       
    }
    private void calcAcceleration(int i)
    {
        calcDensity(i);
        //Calculate forces
        f[i] = calcPressure(i) + calcViscosity(i) + calcSurface(i);
        a[i] = f[i] / rho[i];
    }

    private Vector3 calcPressure(int i)
    {
        Vector3 fp = new Vector3(0, 0, 0);
        for (int j = 0; j < partCount; j++)
        {
            if (j != i)
            {
                float m = particles[j].GetComponent<Rigidbody>().mass;
                Vector3 r = particles[i].transform.position - particles[j].transform.position;
                if (r.sqrMagnitude < h * h)
                {
                    fp += -m * (p[i] + p[j]) / (2 * rho[j]) * -45 / (Mathf.PI * Mathf.Pow(h, 6)) * Mathf.Pow(Mathf.Max(0f, (h - r.magnitude)), 2) * r.normalized;
                }
            }
        }
        
        return fp;
    }
    private Vector3 calcViscosity(int i)
    {
        Vector3 fv = new Vector3(0, 0, 0);
        for (int j = 0; j < partCount; j++)
        {
            if (j != i)
            {
                float m = particles[j].GetComponent<Rigidbody>().mass;
                Vector3 r = particles[i].transform.position - particles[j].transform.position;
                if (r.sqrMagnitude < h * h)
                {
                    fv += m * (particles[j].GetComponent<Rigidbody>().velocity - particles[i].GetComponent<Rigidbody>().velocity) / (rho[j]) * 45 / (Mathf.PI * Mathf.Pow(h, 6)) * Mathf.Max(0f, (h - r.magnitude));
                }
            }
        }
        return fv * mu;
    }

    private Vector3 calcSurface(int i)
    {
        Vector3 n = new Vector3(0, 0, 0);
        float cs = 0;
        for (int j = 0; j < partCount; j++)
        {
            if (j != i)
            {
                float m = particles[j].GetComponent<Rigidbody>().mass;
                Vector3 r = particles[i].transform.position - particles[j].transform.position;
                if (r.sqrMagnitude < h * h)
                {
                    n += m / (rho[j]) * -945 / (32 * Mathf.PI * Mathf.Pow(h, 9)) * Mathf.Pow(Mathf.Max(0f, (h - r.sqrMagnitude)), 2) * r;
                }
            }
        }
        //Debug.Log(n.magnitude);
        if (n.magnitude < nthreshold)
            return new Vector3(0, 0, 0);

        for (int j = 0; j < partCount; j++)
        {
            if (j != i)
            {
                float m = particles[j].GetComponent<Rigidbody>().mass;
                Vector3 r = particles[i].transform.position - particles[j].transform.position;
                if (r.sqrMagnitude < h * h)
                {
                    cs += m / (rho[j]) * -945 / (32 * Mathf.PI * Mathf.Pow(h, 9)) * Mathf.Pow(Mathf.Max(0f, (h * h - r.sqrMagnitude)), 2) * (3 * h * h - 7 * r.sqrMagnitude);
                }
            }
        }

        return -sigma * cs * n.normalized;
    }

}
