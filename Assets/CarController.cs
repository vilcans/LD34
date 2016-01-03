using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour {

    public Vector3 centerOfMass = new Vector3(0, -.5f, .3f);

    public float steeringSpeed = 3.0f;
    public float steeringResetSpeed = 3.0f;

    public float enginePower = 2000f;
    public float maxSteer = 30;
    public float brakingPower = .1f;

    public float neutralSoundSpeed = 10;
    public float idlePitch = .3f;
    public float engineVolume = .5f;

    public float angularVelocityLimit = 2;
    public float velocityLimit = 1;
    public float stillTimeout = 3;

    public Image leftArrow;
    public Image rightArrow;
    public Color arrowPressedColor;

    private Rigidbody rigidbodyComponent;
    private AudioSource audioSourceComponent;
    private WheelCollider[] wheels;

    private bool braking;
    private bool engineWorking;
    private bool steeringWorking;

    private float stillTime;

    private float steering = 0;

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
            steer = Input.GetAxis("Horizontal");
            steer += GetTouchSteering();
        }

        leftArrow.color = Color.Lerp(Color.white, arrowPressedColor, -steer);
        rightArrow.color = Color.Lerp(Color.white, arrowPressedColor, steer);

        //Debug.LogFormat("Power {0}, brake {1}, steer {2}", power, brake, steer);
        //
        float steerAngle = steer * maxSteer;

        for(int i = 0; i < wheels.Length; ++i) {
            WheelCollider wheel = wheels[i];
            if(i < 2) {
                wheel.steerAngle = steerAngle;
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
        bool leftPressed = false;
        bool rightPressed = false;
        var touches = Input.touches;
        for(int i = 0; i < touches.Length; ++i) {
            Touch touch = touches[i];
            if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
                GetPressesFromScreenPoint(touch.position, ref leftPressed, ref rightPressed);
            }
        }
#if UNITY_EDITOR
        if(Input.GetMouseButton(0)) {
            GetPressesFromScreenPoint(Input.mousePosition, ref leftPressed, ref rightPressed);
        }
#endif

        // Update axis
        if(leftPressed) {
            if(steering > 0) {
                steering = 0;  // Snap
            }
            steering = Mathf.Max(-1, steering - Time.deltaTime * steeringSpeed);
        }
        else if(steering < 0) {
            steering = Mathf.Min(0, steering + Time.deltaTime * steeringResetSpeed);
        }
        if(rightPressed) {
            if(steering < 0) {
                steering = 0;  // Snap
            }
            steering = Mathf.Min(1, steering + Time.deltaTime * steeringSpeed);
        }
        else if(steering > 0) {
            steering = Mathf.Max(0, steering - Time.deltaTime * steeringResetSpeed);
        }

        Debug.LogFormat("steering {0}", steering);
        return steering;
    }

    private void GetPressesFromScreenPoint(Vector2 p, ref bool leftPressed, ref bool rightPressed) {
        if(p.x < Screen.width * .25f) {
            leftPressed = true;
        }
        else if(p.x > Screen.width * .75f) {
            rightPressed = true;
        }
    }
}
