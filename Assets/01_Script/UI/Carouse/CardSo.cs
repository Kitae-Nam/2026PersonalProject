using UnityEngine;

namespace _01_Script.UI.Carouse
{
    [CreateAssetMenu(fileName = "CardSo", menuName = "SO/Ui/Card", order = 0)]
    public class CardSo : ScriptableObject
    {
        public int cost;
        public int increaseCost = 1;

        public void NextLevel()
        {
            cost += increaseCost;
        }
    }
}