using UnityEngine;

public class CarDamage : MonoBehaviour {

    void OnCollisionEnter(Collision collision) {
		Debug.LogFormat("Collsion: {0} impulse {1} velocity {2}", collision, collision.impulse.magnitude, collision.relativeVelocity.magnitude);
        foreach (ContactPoint contact in collision.contacts) {
            Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }
    }
}
