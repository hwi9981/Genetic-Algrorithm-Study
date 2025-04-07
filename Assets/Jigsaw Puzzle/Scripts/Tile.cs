using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jigsaw_Puzzle.Scripts
{
    public class Tile : MonoBehaviour
    {
        // public Vector2Int target;
        // public Vector2Int current;
        public TileData data = new TileData();
        public SpriteRenderer spriteRenderer;
        public void SetCurrent(Vector2Int position) => data.current = position;
        public void SetTarget(Vector2Int position) => data.target = position;
        public bool IsCorrect() => data.IsCorrect();
        public void Init(Vector2Int position, Sprite sprite)
        {
            SetTarget(position);
            data.sprite = sprite;
            // target = position;
            spriteRenderer.sprite = sprite;
        }
        public void CopyData(TileData otherData)
        {
            // data.Copy(otherData);
            data = otherData;
            if (spriteRenderer && otherData.sprite)
                spriteRenderer.sprite = otherData.sprite;
        }

        public void OnStartDrag()
        {
            spriteRenderer.sortingOrder = 10; // Để tile được hiển thị lên trước các tile khác
        }

        public void OnEndDrag()
        {
            spriteRenderer.sortingOrder = 0;
        }
    }

    [System.Serializable]
    public class TileData
    {
        public int index;
        public Vector2Int target;
        public Vector2Int current;
        public Sprite sprite;
        public bool IsCorrect() => target == current;
        public void Copy(TileData other)
        {
            target = other.target;
            current = other.current;
            sprite = other.sprite;
        }
    }
}