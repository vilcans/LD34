using UnityEngine;

public class PickingUp : MonoBehaviour {

    public AudioClip pickupAudio;
    public float audioVolume = 1;

    private GameController gameController;

    void Start() {
        gameController = GetComponentInParent<GameController>();
    }

    public void OnTriggerEnter(Collider collider) {
        //Debug.LogFormat("Collided with {0}", collider);
        gameController.PickUp(collider.gameObject);
        Destroy(collider.gameObject);
        AudioSource.PlayClipAtPoint(pickupAudio, collider.transform.position, audioVolume);
    }
}
