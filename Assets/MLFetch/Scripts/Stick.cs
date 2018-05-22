using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stick that is thrown
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Stick : MonoBehaviour {
    public Rigidbody stickRigidbody;
    public bool hasBeenFetched;

	private void Awake() {
        stickRigidbody = GetComponent<Rigidbody>();
	}

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "Floor") {
            stickRigidbody.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (hasBeenFetched)
            gameObject.SetActive(false);
	}
}
