using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Script.UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private string title;
        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void Quit()
        {
            SceneManager.LoadScene(title);
        }
    }
}