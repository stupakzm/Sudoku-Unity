using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameController gameControler;

    private void Start() {
        GetComponent<Slider>().onValueChanged.AddListener(delegate { gameControler.OnMissingNumbersChanged(this.GetComponent<Slider>().value); });
        GetComponent<Slider>().onValueChanged.AddListener(delegate { UpdateText(); });
    }

    private void UpdateText() {
        text.text = "Missing numbers count - " + GetComponent<Slider>().value.ToString();
    }

    public int LoadSliderValue() {
        if (PlayerPrefs.HasKey("MissingNumbers")) {
            float savedValue = PlayerPrefs.GetInt("MissingNumbers");
            GetComponent<Slider>().value = savedValue;
            UpdateText();
        }
        return (int)GetComponent<Slider>().value;
    }
}