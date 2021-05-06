using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    public void OnButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}