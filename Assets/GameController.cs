using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;
    public Text endText;

    private enum State {
        Playing,
        GameOver,
    }
    private State state;

    void Start() {
        gameOverUI.SetActive(false);
        endText.enabled = false;
        state = State.Playing;
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
        endText.text = reason;
        endText.enabled = true;
        gameOverUI.SetActive(true);
    }
}
