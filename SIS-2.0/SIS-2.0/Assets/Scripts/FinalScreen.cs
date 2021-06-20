using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalScreen : MonoBehaviour
{

    public Text _header;
    public Text _score;
    public Text _waves;
    public Text _money;

    private void Start()
    {
        _header.text = PlayerController.win ? $"Congratulations" : $"Too bad... maybe next time!";
        _score.text = $"You've scored {PlayerController.score} point{(PlayerController.score == 1 ? "" : "s")}";
        _waves.text = $"You survived up to wave {EnemiesSpawner.waveNumber}";
        _money.text = $"You've earned a total of {PlayerController.deltaMoney} Krux";
    }

    public void OnQuitPressed()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
