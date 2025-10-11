using UnityEngine;

public class WebManager : MonoBehaviour
{
    [SerializeField]
    private string url = "https://www.example.com";

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}