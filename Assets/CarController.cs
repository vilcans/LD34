using UnityEngine;

public class CarController : MonoBehaviour {

    public Vector3 centerOfMass = new Vector3(0, -.5f, .3f);

    public float enginePower = 2000f;
    public float maxSteer = 30;
    public float brakingPower = .1f;

    public float neutralSoundSpeed = 10;

    private Rigidbody rigidbodyComponent;
    private AudioSource audioSourceComponent;
    private WheelCollider[] wheels;

    private bool destroyed;

    public void Destroy() {
        destroyed = true;
    }

    void Start() {
        rigidbodyComponent = GetComponent<Rigidbody>();
        rigidbodyComponent.centerOfMass = centerOfMass;
        wheels = GetComponentsInChildren<WheelCollider>();
        audioSourceComponent = GetComponentInChildren<AudioSource>();
    }

    void FixedUpdate() {
        bool braking = Input.GetKey("space");
        float brake;
        float power;
        if(braking) {
            power = 0;
            brake = rigidbodyComponent.mass * brakingPower;
        }
        else {
            power = destroyed ? 0 : enginePower;
            brake = 0;
        }
        float steer = Input.GetAxis("Horizontal") * maxSteer;

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

        float speed = rigidbodyComponent.velocity.magnitude;
        audioSourceComponent.pitch = speed / neutralSoundSpeed;
    }
}
