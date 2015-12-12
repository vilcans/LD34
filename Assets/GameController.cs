using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject gameOverUI;

    private bool isAlive = true;

    void Start() {
        gameOverUI.SetActive(false);
        isAlive = true;
    }

    public void Kill() {
        if(isAlive) {
            Debug.Log("Wasted!");
            isAlive = false;
            gameOverUI.SetActive(true);
        }
        else {
            Debug.Log("Wasted again!");
        }
    }
}
