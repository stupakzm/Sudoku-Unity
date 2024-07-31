using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float time;
    private bool isPaused = true;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if (!isPaused) {
            time += Time.deltaTime;
            UpdateText();
        }
    }

    public void ResetTimer() {
        time = 0;
        UpdateText();
    }

    public void StartTimer() {
        ResetTimer();
        isPaused = false;
    }

    public void PauseTimer() {
        isPaused = true;
    }

    public float StopTimer() {
        isPaused = true;
        return time;
    }

    public float GetTime() {
        return time;
    }

    private void UpdateText() {
        text.text = "Timer: " + time.ToString("#.0");
    }
}
