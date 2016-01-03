using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;
    public GameObject successUI;
    public Text endText;

    public float fadeInTime = .5f;
    public float fadeOutTime = .5f;
    public Image faderImage;
    public AudioSource musicSource;
    public GameObject menu;

    public enum State {
        Playing,
        Paused,
        GameOver,
        Restarting,  // forced by user
    }
    private State state;

    private float fadeInProgress;
    private float restartProgress;

    private HashSet<GameObject> pickups;
    private MapView mapView;

    private float defaultMusicVolume;

    IEnumerator Start() {
        mapView = GetComponentInChildren<MapView>();

        defaultMusicVolume = musicSource.volume;

        // delay fade in until textures have been loaded
        fadeInProgress = -999;

        Color newColor = faderImage.color;
        newColor.a = 1;
        faderImage.color = newColor;

        // Wait a while for textures to load
        yield return new WaitForSeconds(.5f);

        successUI.SetActive(false);
        menu.SetActive(false);
        gameOverUI.SetActive(false);
        endText.enabled = false;
        state = State.Playing;

        fadeInProgress = 0;

        pickups = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Pickup"));
        //Debug.LogFormat("Number of pickups: {0}", pickups.Count);
        mapView.SetPickups(pickups);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(state == State.Playing) {
                Pause();
            }
            else if(state == State.Paused) {
                Unpause();
            }
        }
        Color newColor = faderImage.color;
        float fade = newColor.a;
        if(fadeInProgress < fadeInTime) {
            fadeInProgress += Time.unscaledDeltaTime;
            fade = 1 - Mathf.Clamp01(fadeInProgress / fadeInTime);
        }
        if(state == State.Restarting) {
            restartProgress += Time.unscaledDeltaTime;

            fade = Mathf.Clamp01(restartProgress / fadeOutTime);
            if(restartProgress > fadeOutTime) {
                Application.LoadLevel(0);
            }
        }
        newColor.a = fade;
        faderImage.color = newColor;
        faderImage.enabled = fade > 0;

        musicSource.volume = defaultMusicVolume - defaultMusicVolume * restartProgress / fadeOutTime;
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

    public void OnQuit() {
        Application.Quit();
    }

    public void OnRestart() {
        Unpause();
        Restart();
    }

    private void Restart() {
        //Debug.Log("Restarting...");
        menu.SetActive(false);
        state = State.Restarting;
        restartProgress = 0;
    }

    private void StopGame(string reason, bool success=false) {
        state = State.GameOver;
        restartProgress = 0;
        endText.text = reason;
        endText.enabled = true;
        if(success) {
            successUI.SetActive(true);
        }
        else {
            gameOverUI.SetActive(true);
        }
        menu.SetActive(true);
    }

    private void Pause() {
        state = State.Paused;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    private void Unpause() {
        state = State.Playing;
        Time.timeScale = 1;
        menu.SetActive(false);
    }
}
