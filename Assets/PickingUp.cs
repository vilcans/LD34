using UnityEngine;

public class PickingUp : MonoBehaviour {

    public AudioClip pickupAudio;
    public float audioVolume = 1;

    public void OnTriggerEnter(Collider collider) {
        //Debug.LogFormat("Collided with {0}", collider);
        Destroy(collider.gameObject);
        AudioSource.PlayClipAtPoint(pickupAudio, collider.transform.position, audioVolume);
    }
}
