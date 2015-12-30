using UnityEngine;

public class CarController : MonoBehaviour {

    public Vector3 centerOfMass = new Vector3(0, -.5f, .3f);

    public float enginePower = 2000f;
    public float maxSteer = 30;
    public float brakingPower = .1f;

    public float neutralSoundSpeed = 10;
    public float idlePitch = .3f;
    public float engineVolume = .5f;

    public float angularVelocityLimit = 2;
    public float velocityLimit = 1;
    public float stillTimeout = 3;

    private Rigidbody rigidbodyComponent;
    private AudioSource audioSourceComponent;
    private WheelCollider[] wheels;

    private bool braking;
    private bool engineWorking;
    private bool steeringWorking;

    private float stillTime;

    public void BreakEngine() {
        engineWorking = false;
    }

    public void BreakSteering() {
        steeringWorking = false;
    }

    void Start() {
        engineWorking = steeringWorking = true;
        rigidbodyComponent = GetComponent<Rigidbody>();
        rigidbodyComponent.centerOfMass = centerOfMass;
        wheels = GetComponentsInChildren<WheelCollider>();
        audioSourceComponent = GetComponentInChildren<AudioSource>();
    }

    void FixedUpdate() {
        UpdateForces();
        CheckForStill();
    }

    private void UpdateForces() {
        float brake;
        float power;
        if(braking) {
            power = 0;
            brake = rigidbodyComponent.mass * brakingPower;
        }
        else {
            power = engineWorking ? enginePower : 0;
            brake = 0;
        }
        float steer = 0;
        if(steeringWorking) {
            steer = Input.GetAxis("Horizontal") * maxSteer;
            steer += GetTouchSteering() * maxSteer;
        }

        //Debug.LogFormat("Power {0}, brake {1}, steer {2}", power, brake, steer);

        for(int i = 0; i < wheels.Length; ++i) {
            WheelCollider wheel = wheels[i];
            if(i < 2) {
                wheel.steerAngle = steer;
                wheel.motorTorque = power;
            }
            wheel.brakeTorque = brake;

            Transform visual = wheel.transform.GetChild(0);
            Vector3 position;
            Quaternion rotation;
            wheel.GetWorldPose(out position, out rotation);
            visual.transform.position = position;
            visual.transform.rotation = rotation;
        }

        if(engineWorking) {
            float speed = rigidbodyComponent.velocity.magnitude;
            audioSourceComponent.pitch = speed / neutralSoundSpeed * (1 - idlePitch) + idlePitch;
            audioSourceComponent.volume = engineVolume;
        }
        else {
            audioSourceComponent.volume = 0.0f;
        }
    }

    private void CheckForStill() {
        if(!IsCurrentlyStill()) {
            stillTime = 0;
            return;
        }
        stillTime += Time.deltaTime;
        if(stillTime >= stillTimeout) {
            GetComponentInParent<GameController>().OnStill();
            braking = true;
        }
    }

    private bool IsCurrentlyStill() {
        return
            Mathf.Abs(rigidbodyComponent.angularVelocity.magnitude) < angularVelocityLimit &&
            rigidbodyComponent.velocity.sqrMagnitude < (velocityLimit * velocityLimit);
    }

    private float GetTouchSteering() {
        if(Input.touchCount == 0) {
            //return 0;
        }
        float steering = 0;
        var touches = Input.touches;
        for(int i = 0; i < touches.Length; ++i) {
            Touch touch = touches[i];
            if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
                steering += ScreenPointToSteering(touch.position);
            }
        }
        if(Input.GetMouseButton(0)) {
            steering += ScreenPointToSteering(Input.mousePosition);
        }
        Debug.LogFormat("steering {0} screen width {1}", steering, Screen.width);
        return steering;
    }

    private float ScreenPointToSteering(Vector2 p) {
        if(p.x < Screen.width * .25f) {
            return -1;
        }
        if(p.x > Screen.width * .75f) {
            return 1;
        }
        return 0;
    }
}
