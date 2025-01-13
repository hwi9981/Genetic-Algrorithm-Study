using System;
using UnityEngine;

namespace Jigsaw_Puzzle.Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] protected BoardGen _boardGen;
        [Header("UI")]
        [SerializeField] private GameObject _levelCompleteUI;
        
        public void CheckComplete()
        {
            if (_boardGen.IsCompleted())
            {
                Debug.Log("Game Complete!");
                _levelCompleteUI.SetActive(true);
            }
        }
        public void ResetGame()
        {
            _boardGen.ResetBoard();
        }
    }
}