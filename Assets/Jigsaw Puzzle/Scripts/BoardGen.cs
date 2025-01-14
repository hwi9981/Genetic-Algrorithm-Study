using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jigsaw_Puzzle.Scripts
{
    //Some functions written by AI to make faster game then focus to algorithm
    public class BoardGen : MonoBehaviour
    {
        public Texture2D sourceTexture; // Texture lớn ban đầu
        public int rows = 3; // Số dòng
        public int columns = 3; // Số cột
        public Tile tilePrefab; // Prefab của mỗi mảnh ghép (phải có SpriteRenderer)
        public float backgroundAlpha = 0.3f; // Độ trong suốt của ảnh nền (0 = trong suốt, 1 = không trong suốt)

        private float _worldPieceWidth;
        private float _worldPieceHeight;

        private Vector2 _toCenterOffset;
        
        private Tile[,] _slots; // Mảng lưu trữ thông tin các mảnh đã ghép
        private List<Tile> _tiles = new List<Tile>(); // lưu trữ các mảnh ban đầu
        
        void Awake()
        {
            Generate();
        }
        public Tile GetRandomTile() => _tiles[Random.Range(0, _tiles.Count)];
        public List<Tile> GetTiles() => _tiles;
        #region Initization
        void Generate()
        {
            AddBackgroundImage();
            GeneratePuzzlePieces();
            ArrangeAllPiecesBelowImage(_tiles);
        }

        void AddBackgroundImage()
        {
            if (sourceTexture == null)
            {
                Debug.LogError("Source texture is not assigned!");
                return;
            }

            // Tạo sprite từ ảnh gốc
            Sprite backgroundSprite = Sprite.Create(
                sourceTexture,
                new Rect(0, 0, sourceTexture.width, sourceTexture.height),
                new Vector2(0.5f, 0.5f) // Tâm của ảnh gốc
            );

            // Tạo GameObject ảnh nền
            GameObject backgroundObject = new GameObject("BackgroundImage");
            backgroundObject.transform.SetParent(transform);
            backgroundObject.transform.localPosition = Vector3.zero;

            // Thêm SpriteRenderer và gắn sprite
            SpriteRenderer backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
            backgroundRenderer.sprite = backgroundSprite;
            backgroundRenderer.color = new Color(1, 1, 1, backgroundAlpha); // Điều chỉnh độ trong suốt
            backgroundRenderer.sortingOrder = -1;
        }
        void GeneratePuzzlePieces()
        {
            if (sourceTexture == null || tilePrefab == null)
            {
                Debug.LogError("Source texture or piece prefab is not assigned!");
                return;
            }
            _tiles.Clear();
            int pieceWidth = sourceTexture.width / columns;
            int pieceHeight = sourceTexture.height / rows;

            // Tính kích thước thế giới của mỗi mảnh ghép
            _worldPieceWidth = pieceWidth / 100f; // Đơn vị Unity (mỗi pixel = 1/100)
            _worldPieceHeight = pieceHeight / 100f;

            // Tính offset để căn giữa
            float xOffset = (columns - 1) * (_worldPieceWidth) / 2f;
            float yOffset = (rows - 1) * (_worldPieceHeight) / 2f;
            _toCenterOffset = new Vector2(xOffset, yOffset);
            
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // Tạo vùng cắt từ texture
                    Texture2D pieceTexture = new Texture2D(pieceWidth, pieceHeight);

                    // Đảo ngược trục Y khi lấy pixel
                    int flippedRow = rows - 1 - row;
                    Color[] pixels = sourceTexture.GetPixels(
                        column * pieceWidth,
                        flippedRow * pieceHeight,
                        pieceWidth,
                        pieceHeight
                    );
                    pieceTexture.SetPixels(pixels);
                    pieceTexture.Apply();

                    // Tạo sprite từ texture
                    Sprite pieceSprite = Sprite.Create(
                        pieceTexture,
                        new Rect(0, 0, pieceWidth, pieceHeight),
                        new Vector2(0.5f, 0.5f) // Điểm pivot
                    );
                    // Tạo mảnh ghép mới
                    Tile tile = Instantiate(tilePrefab, transform);
                    // Tính toán vị trí của mảnh ghép trong không gian
                    tile.transform.position = TileWorldPosition(row, column);
                    tile.name = $"({row} : {column})";
                    tile.Init(new Vector2Int(row, column), pieceSprite);
                    _tiles.Add(tile);
                }
            }
            _slots = new Tile[rows, columns];
        }

        Vector2 TileWorldPosition(int row, int column)
        {
            float xPos = column * (_worldPieceWidth) - _toCenterOffset.x;
            float yPos = -(row * (_worldPieceHeight)) + _toCenterOffset.y;
            return new Vector2(xPos, yPos);
        }
        public void ArrangeAllPiecesBelowImage(List<Tile> tiles,float yOffset = 0.5f)
        {
            foreach (Tile tile in tiles)
            { 
                ArrangeSinglePiecesBelowImage(tile, yOffset);
                tile.SetCurrent(new Vector2Int(-1, -1)); // Reset vị trí hiện tại của tile
            }
        }
        void ArrangeSinglePiecesBelowImage(Tile tile, float yOffset = 0.5f)
        {
            float totalWidth = columns * _worldPieceWidth; // Tổng chiều dài ảnh
            float totalHeight = rows * _worldPieceHeight; // Tổng chiều cao ảnh
            
            float startY = - totalHeight / 2f - _worldPieceHeight / 2f - yOffset; // Vị trí dọc theo trục Y dưới ảnh
            System.Random rng = new System.Random(); // Sử dụng để tạo giá trị ngẫu nhiên

            float xPos = Random.Range(- totalWidth / 2f, totalWidth / 2f); // Tạo giá trị ngẫu nhiên trên trục X
            tile.transform.position = new Vector3(xPos, startY, 0);
        }

        public void ResetBoard()
        {
            _slots = new Tile[rows, columns];
            ArrangeAllPiecesBelowImage(_tiles);
        }

        #endregion

        #region Game Function

        // TO DO Move this to GameController
        public void SnapTile(Tile tile)
        {
            Vector2 tilePosition = tile.transform.position; // Lấy vị trí hiện tại của tile
            float closestDistance = float.MaxValue;
            Vector2 targetGridPosition = Vector2.zero; // Vị trí lưới mục tiêu của tile

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Vector2 boardPosition = TileWorldPosition(row, column); // Tính vị trí của từng ô trên board
                    float distance = Vector2.Distance(tilePosition, boardPosition); // Khoảng cách từ tile đến vị trí đó

                    // Kiểm tra xem vị trí này có gần hơn vị trí gần nhất hiện tại không
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetGridPosition = new Vector2(row, column);
                    }
                }
            }
            // Nếu tile nằm trong phạm vi cho phép (snapThreshold), gắn tile vào vị trí gần nhất
            float snapThreshold = Mathf.Min(_worldPieceWidth, _worldPieceHeight) * 0.5f; // Ngưỡng để snap
            if (closestDistance <= snapThreshold)
            {
                StickTileToBoard(tile, (int)targetGridPosition.x, (int)targetGridPosition.y);
                // tile.Lock(); 
            }
        }

        public Tile GetNearestTile(Vector2 mousePosition)
        {
            Tile nearestTile = null;
            float maxDistance = _worldPieceWidth;
            float closestDistanceSqr = maxDistance * maxDistance; // Sử dụng bình phương khoảng cách tối đa

            foreach (Tile tile in _tiles)
            {
                float distanceSqr = ((Vector2)tile.transform.position - mousePosition).sqrMagnitude;

                // Kiểm tra nếu khoảng cách nằm trong phạm vi và nhỏ hơn khoảng cách gần nhất hiện tại
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    nearestTile = tile;
                }
            }
            if (nearestTile != null)
            {
                RemoveTileFromSlot(nearestTile); // Xóa tile khỏi slot nếu nằm trong slot
            }
            return nearestTile; // Trả về Tile gần nhất hoặc null nếu không tìm thấy
        }

        void RemoveTileFromSlot(Tile tile)
        {
            if (tile != null)
            {
                for (int row = 0; row < _slots.GetLength(0); row++) // Duyệt qua từng hàng
                {
                    for (int col = 0; col < _slots.GetLength(1); col++) // Duyệt qua từng cột
                    {
                        if (_slots[row, col] == tile) // Nếu tìm thấy Tile trong mảng
                        {
                            _slots[row, col].SetCurrent(new Vector2Int(-1, -1));
                            _slots[row, col] = null; // Gán phần tử đó bằng null
                            break; // Thoát khỏi vòng lặp
                        }
                    }
                }
            }
        }

        public void RemoveAllTilesFromSlots()
        {
            for (int i = 0; i < _slots.GetLength(0); i++)
            {
                for (int j = 0; j < _slots.GetLength(1); j++)
                {
                    var tile = _slots[i, j];
                    if (tile != null)
                    {
                        tile.SetCurrent(new Vector2Int(-1, -1));
                        _slots[i, j] = null;
                    }
                }
            }
            // _slots = new Tile[rows, columns]; 
        }

        public void StickTileToBoard(Tile tile, int row, int column)
        {
            // if (_slots[row, column] != null)
            // {
            //     Debug.Log("Slot is not empty!");
            //     ArrangeSinglePiecesBelowImage(tile);
            //     return;
            // }
            _slots[row, column] = tile;
            tile.SetCurrent(new Vector2Int(row, column));
            tile.transform.position = TileWorldPosition(row, column);
        }

        #endregion

        public bool IsCompleted()
        {
            foreach (var slot in _slots)
            {
                if (slot == null || !slot.IsCorrect())
                {
                    return false;
                }
            }
            return true;
        }

    }
}