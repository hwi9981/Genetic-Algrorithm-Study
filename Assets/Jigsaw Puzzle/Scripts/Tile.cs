using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jigsaw_Puzzle.Scripts
{
    public class Tile : MonoBehaviour
    {
        public Vector2Int Target;
        public Vector2Int Current;
        [SerializeField] private SpriteRenderer _renderer;
        private bool _isLocked = false;
        
        public bool IsCorrect() => Target == Current;
        public void Init(Vector2Int position, Sprite sprite)
        {
            Target = position;
            _renderer.sprite = sprite;
        }
        public void Lock()
        {
            _isLocked = true; // Khóa tile
        }

        public bool IsLocked()
        {
            return _isLocked;
        }

        public void OnStartDrag()
        {
            _renderer.sortingOrder = 10; // Để tile được hiển thị lên trước các tile khác
        }

        public void OnEndDrag()
        {
            _renderer.sortingOrder = 0;
        }

        
    }
}