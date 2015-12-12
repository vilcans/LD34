using UnityEngine;

public class CarDamage : MonoBehaviour {

    public float health = 100000;
    public float lowerDamageLimit = .02f;
    public float healthPerImpulse = 1.0f / 100000;

    private CarController carController;

    void Start() {
        carController = GetComponent<CarController>();
    }

    void OnCollisionEnter(Collision collision) {
        //Debug.LogFormat("Collsion: {0} impulse {1} velocity {2}", collision, collision.impulse.magnitude, collision.relativeVelocity.magnitude);
        float damage = collision.impulse.magnitude * healthPerImpulse;
        if(damage >= lowerDamageLimit) {
            health = Mathf.Max(health - damage, 0);
            Debug.LogFormat("Damage {0}, new health {1}", damage, health);
            if(health <= 0) {
                carController.BreakEngine();
            }
        }
        else {
            Debug.LogFormat("Low damage {0}", damage);
        }
        /*
        foreach (ContactPoint contact in collision.contacts) {
            //Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }
        */
    }
}
