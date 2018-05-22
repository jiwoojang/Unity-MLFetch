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
    [SerializeField] private float _repositionFactor = 1.0f;

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
        _stickThrower.stick.transform.position = new Vector3(_floorTransform.position.x + Random.value * 8 - 4, _floorTransform.position.y + 0.5f, _floorTransform.position.z + Random.value * 8 - 4);
    }

    // Collects observations about the world
    public override void CollectObservations() {
        // Get relative position to stick
        Vector3 relativePosition = _target.position - transform.position;

        // Note all values here are normalized to the size of the space
        AddVectorObs(relativePosition.x / 5f);
        AddVectorObs(relativePosition.z / 5f);

        AddVectorObs((transform.position.x + 5f) / 5);
        AddVectorObs((transform.position.x - 5f) / 5);
        AddVectorObs((transform.position.z + 5f) / 5);
        AddVectorObs((transform.position.z - 5f) / 5);

        // Observe agent velocity as not to overshoot
        AddVectorObs(_agentRigidbody.velocity.x / 5f);
        AddVectorObs(_agentRigidbody.velocity.z / 5f);
	}

	// Lights, camera, action!
	public override void AgentAction(float[] vectorAction, string textAction) {

        // Check how close we are to the stick
        float distancetoTarget = Vector3.Distance(transform.position, _target.position);

        // If we are still on the way to the stick
        if (!_stickThrower.stick.hasBeenFetched){
            
            // We got the stick, switch targets and come back
            if (distancetoTarget < 1.5f) {
                // Got the stick, now come back
                AddReward(1.0f);

                _stickThrower.stick.hasBeenFetched = true;
                _target.position = _startPosition;

                // Reset the distance
                _previousTargetDistance = Vector3.Distance(transform.position, _target.position);
            }
        } else {
            // More lenient criteria for making it back home
            if (distancetoTarget < 1.5f) {
                // Reward for making it back AFTER fetching the stick
                Done();
                AddReward(1.0f);
            }
        }

        // Get closer to the target
        if (distancetoTarget < _previousTargetDistance){
            AddReward(0.1f);
        }

        // Dont fall off the ledge 
        if ((transform.position.y - _floorTransform.position.y) < 0) {
            Done();
            AddReward(-1.0f);
        }

        // Time penalty, do it quickly!
        AddReward(-0.05f);

        // Cache the distance to target
        _previousTargetDistance = distancetoTarget;

        // Act on the actions given by the brain 
        Vector3 movementAction = Vector3.zero;

        movementAction.x = Mathf.Clamp(vectorAction[0], -1, 1);
        movementAction.z = Mathf.Clamp(vectorAction[1], -1, 1);

        // Move the agent
        //_agentRigidbody.AddForce(movementAction * _fetchSpeed);
        transform.position = Vector3.Lerp(transform.position, transform.position + movementAction, Time.deltaTime * _fetchSpeed);
	}

	// Reset function
	public override void AgentReset() {

        // Agent is done, reset the stick, the thrower and the agent
        if (IsDone()){
            Debug.Log("Resetting");
            _stickThrower.ResetStickThrower();

            // Reset physics
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;

            _agentRigidbody.angularVelocity = Vector3.zero;
            _agentRigidbody.velocity = Vector3.zero;

            // Throw the stick again!
            _stickThrower.RandomizeAndThrow();

            // Randomize stick position
            //_stickThrower.stick.transform.position = new Vector3(_floorTransform.position.x + Random.value * _repositionFactor - (_repositionFactor / 2f), _floorTransform.position.y + 0.5f, _floorTransform.position.z + Random.value * _repositionFactor - (_repositionFactor / 2f));
            _target = _stickThrower.stick.transform;
        }
	}
}
