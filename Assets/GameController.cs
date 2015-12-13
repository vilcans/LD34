using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;
    public Text endText;

    public float restartDelay = 2.0f;

    private enum State {
        Playing,
        GameOver,
    }
    private State state;

    private float gameOverProgress;

    void Start() {
        gameOverUI.SetActive(false);
        endText.enabled = false;
        state = State.Playing;
    }

    void Update() {
        if(state == State.GameOver) {
            gameOverProgress += Time.deltaTime;
            if(gameOverProgress >= restartDelay) {
                Application.LoadLevel(0);
            }
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

    private void StopGame(string reason) {
        state = State.GameOver;
        gameOverProgress = 0;
        endText.text = reason;
        endText.enabled = true;
        gameOverUI.SetActive(true);
    }
}
