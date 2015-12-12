using UnityEngine;

public class PickingUp : MonoBehaviour {

    public void OnTriggerEnter(Collider collider) {
        //Debug.LogFormat("Collided with {0}", collider);
        Destroy(collider.gameObject);
    }
}
