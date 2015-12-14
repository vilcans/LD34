using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;
    public GameObject successUI;
    public Text endText;

    public float fadeInTime = .5f;
    public float restartDelay = 2.0f;
    public float fadeOutTime = .5f;
    public Image faderImage;

    private enum State {
        Playing,
        GameOver,
        Restarting,  // forced by user
    }
    private State state;

    private float fadeInProgress;
    private float gameOverProgress;

    private HashSet<GameObject> pickups;
    private MapView mapView;

    IEnumerator Start() {
        mapView = GetComponentInChildren<MapView>();

        // delay fade in until textures have been loaded
        fadeInProgress = -1;

        Color newColor = faderImage.color;
        newColor.a = 1;
        faderImage.color = newColor;

        // Wait one frame to load the UI textures
        yield return null;
        gameOverUI.SetActive(false);
        successUI.SetActive(false);
        endText.enabled = false;
        state = State.Playing;

        fadeInProgress = 0;

        pickups = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Pickup"));
        Debug.LogFormat("Number of pickups: {0}", pickups.Count);
        mapView.SetPickups(pickups);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && state == State.Playing) {
            Debug.Log("Restarting...");
            state = State.Restarting;
            gameOverProgress = restartDelay - fadeOutTime;
        }
        Color newColor = faderImage.color;
        float fade = newColor.a;
        if(fadeInProgress < fadeInTime) {
            fadeInProgress += Time.deltaTime;
            fade = 1 - Mathf.Clamp01(fadeInProgress / fadeInTime);
        }
        if(state == State.GameOver || state == State.Restarting) {
            gameOverProgress += Time.deltaTime;

            fade = Mathf.Clamp01(1 - (restartDelay - gameOverProgress) / fadeOutTime);
            if(gameOverProgress > restartDelay) {
                Application.LoadLevel(0);
            }
        }
        newColor.a = fade;
        faderImage.color = newColor;
    }

    public void PickUp(GameObject pickup) {
        bool found = pickups.Remove(pickup);
        if(!found) {
            Debug.LogWarningFormat("Could not pick up unknown object {0}", pickup);
            return;
        }
        mapView.PickUp(pickup);
        Destroy(pickup);
        if(pickups.Count == 0 && state == State.Playing) {
            StopGame("You collected them all", true);
        }
    }

    public void Kill() {
        if(state == State.Playing) {
            StopGame("You're smashed");
        }
    }

    public void OnCarDestroyed() {
        if(state == State.Playing) {
            StopGame("Your car is wrecked");
        }
    }

    public void OnStill() {
        if(state == State.Playing) {
            StopGame("You managed to stop,\nbut didn't pick up all cones", true);
        }
    }

    private void StopGame(string reason, bool success=false) {
        state = State.GameOver;
        gameOverProgress = 0;
        endText.text = reason;
        endText.enabled = true;
        if(success) {
            successUI.SetActive(true);
        }
        else {
            gameOverUI.SetActive(true);
        }
    }
}
