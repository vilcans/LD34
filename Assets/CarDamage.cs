using UnityEngine;

public class CarDamage : MonoBehaviour {

    public float health = 1000;
    public float lowerDamageLimit = 100;
    public float killDamage = 300;
    public float healthPerImpulse = .1f;

    public float fullVolumeDamage = 200;
    public AudioClip[] damageAudio;

    public RectTransform healthBar;

    private CarController carController;
    private GameController gameController;
    private float maxHealth;

    void Start() {
        maxHealth = health;
        carController = GetComponent<CarController>();
        gameController = GetComponentInParent<GameController>();
    }

    void OnCollisionEnter(Collision collision) {
        //Debug.LogFormat("Collsion: {0} impulse {1} velocity {2}", collision, collision.impulse.magnitude, collision.relativeVelocity.magnitude);
        float damage = collision.impulse.magnitude * healthPerImpulse;
        GiveDamage(damage, collision.contacts[0].point);
        /*foreach (ContactPoint contact in collision.contacts) {
            Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }*/
    }

    public void GiveDamage(float damage, Vector3 position) {
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
        float volume = damage / fullVolumeDamage;
        AudioClip clip = damageAudio[Random.Range(0, damageAudio.Length)];
        AudioSource.PlayClipAtPoint(clip, position, volume);

        float newSize = Mathf.Max(health / maxHealth, 0) * 128;
        healthBar.sizeDelta = new Vector2(newSize, 16);
    }
}
