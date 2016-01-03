using UnityEngine;

public class WheelDamage : MonoBehaviour {

    public float minDamageForce = 10000;
    public float damagePerNewton = 1e-3f;

    private CarDamage carDamage;
    private WheelCollider wheelCollider;

    void Start() {
        carDamage = GetComponentInParent<CarDamage>();
        wheelCollider = GetComponent<WheelCollider>();
    }

    void Update() {
        WheelHit hit;
        if(wheelCollider.GetGroundHit(out hit)) {
            float damage = (hit.force - minDamageForce) * damagePerNewton;
            if(damage > 0) {
                //Debug.LogFormat("{0} giving damage {1}", this, damage);
                carDamage.GiveDamage(damage, transform.position);
            }
        }
    }
}
