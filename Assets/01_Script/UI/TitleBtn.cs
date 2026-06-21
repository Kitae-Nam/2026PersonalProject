using _01_Script.Train;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Script.UI
{
    public class TitleBtn : MonoBehaviour
    {
        [SerializeField] private string title;
        
        public void GameStart()
        {
            SceneManager.LoadScene(title);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}