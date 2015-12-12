using UnityEngine;

public class CarDamage : MonoBehaviour {

    public float health = 1000;
    public float lowerDamageLimit = 100;
    public float killDamage = 300;
    public float healthPerImpulse = .1f;

    public float fullVolumeVelocity = 25;
    public AudioClip damageAudio;

    private CarController carController;
    private GameController gameController;

    void Start() {
        carController = GetComponent<CarController>();
        gameController = GetComponentInParent<GameController>();
    }

    void OnCollisionEnter(Collision collision) {
        //Debug.LogFormat("Collsion: {0} impulse {1} velocity {2}", collision, collision.impulse.magnitude, collision.relativeVelocity.magnitude);
        float damage = collision.impulse.magnitude * healthPerImpulse;
        if(damage >= killDamage) {
            Debug.LogFormat("Killed by damage {0}", damage);
            carController.BreakSteering();
            gameController.Kill();
        }
        if(damage >= lowerDamageLimit) {
            if(health > 0) {
                health -= damage;
                Debug.LogFormat("Damage {0}, new health {1}", damage, health);
                if(health <= 0) {
                    health = 0;
                    carController.BreakEngine();
                    gameController.OnCarDestroyed();
                }
            }
        }
        else {
            Debug.LogFormat("Low damage {0}", damage);
        }
        float velocity = collision.relativeVelocity.magnitude;
        float volume = velocity / fullVolumeVelocity;
        //Debug.LogFormat("velocity {0}, volume {1}", velocity, volume);
        AudioSource.PlayClipAtPoint(damageAudio, collision.contacts[0].point, volume);
        /*foreach (ContactPoint contact in collision.contacts) {
            Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }*/
    }
}
