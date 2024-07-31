using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SiteOpener : MonoBehaviour
{
    [SerializeField] private string url = "https://stupak.itch.io/";

    private void Start() {
        gameObject.GetComponent<Button>().onClick.AddListener(() => Open());
    }

    public void Open() {
        Application.OpenURL(url);
    }
}
