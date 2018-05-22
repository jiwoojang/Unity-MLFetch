using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that automates the stick throwing for training purposes. Can be replaced by player behaviour later
/// </summary>
public class StickThrower : MonoBehaviour {

    // Horizontal angle range in which we could throw the stick
    // This is relative to the throw direction, +- half of the range on each side
    [SerializeField] private float _horizontalAngleRange;

    // Vertical range in which the stick could be thrown
    // This is relative to the thrown direction + vertical range in degrees
    [SerializeField] private float _verticalAngleRange;

    // Max force with which the stick can be thrown, will be randomized between 0 and this number
    [Range(1.0f, 10.0f)]
    [SerializeField] private float _forceMax;

    // Scales the force with which the stick may be thrown
    [SerializeField] private float _forceMultiplyer = 1.0f;

    private Vector3     _stickDefaultPosition;
    private Quaternion  _stickDefaultRotation;

    private Quaternion  _stickThrowerDefaultRotation;

    public Stick stick;

	private void Awake() {
        _stickDefaultPosition = stick.transform.position;
        _stickDefaultRotation = stick.transform.rotation;

        _stickThrowerDefaultRotation = transform.rotation;
	}

	public void RandomizeAndThrow() {
        // Randomizes the direction and force to throw the stick with
        // We shall treat the forward vector of the transform as the forward direction in which to throw

        // Reset to default
        ResetStickThrower();

        // Randomize the angles
        float horizontalRotation = Random.Range(-(_horizontalAngleRange/2f), (_horizontalAngleRange/2f));
        float verticalRotation = Random.Range(0f, _verticalAngleRange);

        // Randomize force
        float randomizedForceMagnitude = Random.Range(_forceMax / 2, _forceMax);
                                               
        // Apply the randomized angles relative to the current transform;
        Quaternion randomizedRotation = transform.rotation * Quaternion.Euler(verticalRotation,horizontalRotation,0);
        transform.rotation = randomizedRotation;

        // Throw the stick!
        ThrowStick(transform.forward, randomizedForceMagnitude * _forceMultiplyer);
    }

    public void ThrowStick(Vector3 direction, float magnitude) {
        // We use impulse here because its just one initial throwing force
        stick.stickRigidbody.AddForce(direction.normalized * magnitude, ForceMode.Impulse);
        stick.stickRigidbody.AddTorque(direction.normalized * magnitude, ForceMode.Impulse);
    }

    public void ResetStickThrower(){
        // Reset thrower positions. 
        transform.rotation = _stickThrowerDefaultRotation; 

        // Reset stick position
        stick.transform.position = _stickDefaultPosition;
        stick.transform.rotation = _stickDefaultRotation;

        // Reset stick parameters
        stick.hasBeenFetched = false;
        stick.gameObject.SetActive(true);
        stick.stickRigidbody.isKinematic = false;
    }
}
