using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Fetch Agent is responsible for interpreting the world, sending its observations to the brain, and acting on the decisions the brain imparts on it
/// </summary>
public class FetchAgent : Agent {
    [SerializeField] private StickThrower _stickThrower;
    [SerializeField] private float _fetchSpeed;
    [SerializeField] private Transform _floorTransform;

    private Rigidbody _agentRigidbody;
    private Vector3 _startPosition;
    private Transform _target;
    private float _previousTargetDistance = float.MaxValue;

	void Start () {
        // Capture the starting transform
        _startPosition = transform.position;
        _target = _stickThrower.stick.transform;

        // Initialize components
        _agentRigidbody = GetComponent<Rigidbody>();

        // Fetch!
        _stickThrower.RandomizeAndThrow();
	}

    // Collects observations about the world
	public override void CollectObservations() {
        // Get relative position to stick
        Vector3 relativePosition = _target.position - transform.position;

        AddVectorObs(relativePosition.x / 15f);
        AddVectorObs(relativePosition.y / 15f);

        AddVectorObs((transform.position.x + 7.5f) / 15);
        AddVectorObs((transform.position.x - 7.5f) / 15);
        AddVectorObs((transform.position.z + 7.5f) / 15);
        AddVectorObs((transform.position.z - 7.5f) / 15);

        // Observe agent velocity as not to overshoot
        AddVectorObs(_agentRigidbody.velocity.x / 15f);
        AddVectorObs(_agentRigidbody.velocity.y / 15f);
	}

	// Lights, camera, action!
	public override void AgentAction(float[] vectorAction, string textAction) {

        // Check how close we are to the stick
        float distancetoTarget = Vector3.Distance(transform.position, _target.position);

        // If we are still on the way to the stick
        if (!_stickThrower.stick.hasBeenFetched){
            
            // We got the stick, switch targets and come back
            if (distancetoTarget < 1.0f) {
                _stickThrower.stick.hasBeenFetched = true;
                _target.position = _startPosition;

                // Reset the distance
                _previousTargetDistance = float.MaxValue;

                // REMOVE THIS LATER TO ENABLE RETRIEVE
                Done();
                AddReward(4.0f);
            }
        } else {
            if (distancetoTarget < 1.0f){
                // Big bonus points for making it back AFTER fetching the stick
                Done();
                AddReward(10.0f);
            }
        }

        // Get closer to the target
        if (distancetoTarget < _previousTargetDistance){
            AddReward(0.2f);
        }

        // Dont fall off the ledge 
        if ((transform.position.y - _floorTransform.position.y) < 0) {
            Done();
            AddReward(-8.0f);
        }

        // Time penalty, do it quickly!
        AddReward(-0.1f);

        // Cache the distance to target
        _previousTargetDistance = distancetoTarget;

        // Act on the actions given by the brain 
        Vector3 movementAction = Vector3.zero;

        movementAction.x = Mathf.Clamp(vectorAction[0], -1, 1);
        movementAction.z = Mathf.Clamp(vectorAction[1], -1, 1);

        // Move the agent
        _agentRigidbody.AddForce(movementAction * _fetchSpeed);
	}

	// Reset function
	public override void AgentReset() {

        // Agent is done, reset the stick, the thrower and the agent
        if (IsDone()){
            Debug.Log("Resetting");
            _stickThrower.ResetStickThrower();

            // Reset physics
            transform.position = _startPosition;

            //transform.position = new Vector3 (_floorTransform.position.x + Random.Range(-10f, 10f), _startPosition.y, _floorTransform.position.z + Random.Range(-10f, 10f));
            transform.rotation = Quaternion.identity;

            _agentRigidbody.angularVelocity = Vector3.zero;
            _agentRigidbody.velocity = Vector3.zero;

            // Throw the stick again!
            _stickThrower.RandomizeAndThrow();

            _target = _stickThrower.stick.transform;
        }
	}
}
