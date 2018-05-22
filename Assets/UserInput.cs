using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public Rigidbody ballRigidbody;
    public float forceScale = 1.0f;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        Vector3 appliedForce = Vector3.zero;

        if (Input.GetKeyDown(up)) {
            appliedForce = new Vector3(0, 0, 1.0f);
        } else if (Input.GetKeyDown(down)) {
            appliedForce = new Vector3(0, 0, -1.0f);
        } else if (Input.GetKeyDown(left)) {
            appliedForce = new Vector3(-1.0f, 0, 0);
        } else if (Input.GetKeyDown(right)) {
            appliedForce = new Vector3(1.0f, 0, 0);
        }

        if (appliedForce != Vector3.zero) {
            Debug.Log("Applying force!");
            ballRigidbody.AddForce(appliedForce.normalized * forceScale, ForceMode.Impulse);
        }
    }
}
