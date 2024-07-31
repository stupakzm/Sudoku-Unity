using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject message;
    [SerializeField] private TextMeshProUGUI messageText;
    [Range(2,12)]
    [SerializeField] private float disableTime;

    public void ShowMessage(string messageText) {
        message.SetActive(true);
        this.messageText.text = messageText;
        Invoke(nameof(DisableMessage), disableTime);
    }

    public void ShowMessage(string messageText, float time) {
        message.SetActive(true);
        this.messageText.text = messageText;
        Invoke(nameof(DisableMessage), time);
    }

    public void HandleSize(string message) {
        //handle scale widht and height depends on message lenght
    }

    private void DisableMessage() {
        message.SetActive(false);
    }
}
