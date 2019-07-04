using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone_script : MonoBehaviour {

    public GameObject stoneColor = null;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    
	}

    public void MyInstantiate (Vector3 point)
    {
        Instantiate(stoneColor, new Vector3(point.x, (float)3.15, point.z), transform.rotation);
    }
}
