using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;
    public Text endText;

    public float fadeInTime = .5f;
    public float restartDelay = 2.0f;
    public float fadeOutTime = .5f;
    public Image faderImage;

    private enum State {
        Playing,
        GameOver,
    }
    private State state;

    private float fadeInProgress;
    private float gameOverProgress;

    IEnumerator Start() {
        // delay fade in until textures have been loaded
        fadeInProgress = -1;

        Color newColor = faderImage.color;
        newColor.a = 1;
        faderImage.color = newColor;

        // Wait one frame to load the UI textures
        yield return null;
        gameOverUI.SetActive(false);
        endText.enabled = false;
        state = State.Playing;

        fadeInProgress = 0;
    }

    void Update() {
        Color newColor = faderImage.color;
        float fade = newColor.a;
        if(fadeInProgress < fadeInTime) {
            fadeInProgress += Time.deltaTime;
            fade = 1 - Mathf.Clamp01(fadeInProgress / fadeInTime);
        }
        if(state == State.GameOver) {
            gameOverProgress += Time.deltaTime;

            fade = Mathf.Clamp01(1 - (restartDelay - gameOverProgress) / fadeOutTime);
            if(gameOverProgress > restartDelay) {
                Application.LoadLevel(0);
            }
        }
        newColor.a = fade;
        faderImage.color = newColor;
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

    private void StopGame(string reason) {
        state = State.GameOver;
        gameOverProgress = 0;
        endText.text = reason;
        endText.enabled = true;
        gameOverUI.SetActive(true);
    }
}
