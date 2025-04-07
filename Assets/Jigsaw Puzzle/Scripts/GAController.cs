using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Jigsaw_Puzzle.Scripts
{
    public class GAController : GameController
    {
        [SerializeField] private TMP_Text _generationText;
        [SerializeField] private TMP_Text _bestFitnessText;

        [Header("GA settings")] [SerializeField]
        private int populationSize = 200;

        [SerializeField] private float mutationRate = 0.05f;
        [SerializeField] private float _speed = 1;

        private GeneticAlgorithm<TileData> _ga;
        private float _timeCounter;

        private void Start()
        {
            Invoke(nameof(StartAlgorithm), 0.5f);
        }

        void StartAlgorithm()
        {
            // CloneOriginTiles();

            _ga = new GeneticAlgorithm<TileData>(populationSize, _boardGen.rows * _boardGen.columns, mutationRate,
                GetRandomTile, EvaluateFitness, GetRandomTileArrangement);
        }
        
        void DisplayGenes(List<TileData> genes)
        {
            _boardGen.RemoveAllTilesFromSlots();

            int columns = _boardGen.columns; // Số cột của bảng

            for (int i = 0; i < genes.Count; i++)
            {
                int row = i / columns; // Xác định hàng
                int col = i % columns; // Xác định cột

                _boardGen.GetTiles()[i].CopyData(genes[i]);
                _boardGen.StickTileToBoard(_boardGen.GetTiles()[i], row, col);
            }

            // DebugGeneIndex(genes);
        }

        void DebugGeneIndex(List<TileData> genes)
        {
            int columns = _boardGen.columns;
            string indexstr = "";
            for (int i = 0; i < genes.Count; i++)
            {
                indexstr += genes[i].index + ", ";
            }
            Debug.Log(indexstr);
        }


        private void Update()
        {
            _timeCounter += Time.deltaTime * _speed;
            if (_timeCounter >= 2)
            {
                _timeCounter = 0;
                if (_ga != null)
                {
                    _ga.NewGeneration();
                    // Debug.Log("Gen "+ _ga.Generation + ": " + _ga.BestFitness);
                    _generationText.text = _ga.Generation.ToString();
                    _bestFitnessText.text = _ga.BestFitness.ToString("F2");
                    DisplayGenes(_ga.BestGenes.ToList());
                    // if (Mathf.Approximately(_ga.BestFitness, 1))
                    // {
                    //     // Debug.Log("Result: " + _ga.Generation + " - " + _ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
                    //     enabled = false;
                    //     CheckComplete();
                    // }
                }
            }
        }

        private float EvaluateFitness(int index)
        {
            float score = 0;
            DNA<TileData> dna = _ga.Population[index];

            for (int i = 0; i < dna.Genes.Length; i++)
            {
                var row = i / _boardGen.columns;
                var col = i % _boardGen.columns;
                dna.Genes[i].current = new Vector2Int(row, col);

                // Tính toán số pixel khớp với các ô bên cạnh
                if (col < _boardGen.columns - 1) // Kiểm tra ô bên phải
                {
                    score += CalculateMatchPixel(dna.Genes[i].sprite, dna.Genes[i + 1].sprite, MatchDirection.Right);
                }
                if (col > 0) // Kiểm tra ô bên trái
                {
                    score += CalculateMatchPixel(dna.Genes[i].sprite, dna.Genes[i - 1].sprite, MatchDirection.Left);
                }
                if (row < _boardGen.rows - 1) // Kiểm tra ô bên dưới
                {
                    score += CalculateMatchPixel(dna.Genes[i].sprite, dna.Genes[i + _boardGen.columns].sprite, MatchDirection.Bottom);
                }
                if (row > 0) // Kiểm tra ô bên trên
                {
                    score += CalculateMatchPixel(dna.Genes[i].sprite, dna.Genes[i - _boardGen.columns].sprite, MatchDirection.Top);
                }
            }
            return score;
        }

        public TileData GetRandomTile()
        {
            var randomData = new TileData();
            randomData.Copy(_boardGen.GetRandomTile().data);
            return randomData;
        }

        // Tạo 1 cách sắp xếp ngẫu nhiên các ô
        public TileData[] GetRandomTileArrangement()
        {
            var tileDatas = _boardGen.GetTiles().Select(x => x.data).ToList();
            return tileDatas.OrderBy(x => Random.value).ToArray();
        }
        public int CalculateMatchPixel(Sprite sprite, Sprite other, MatchDirection direction)
        {
            var tex1 = sprite.texture;
            var tex2 = other.texture;
            int width1 = tex1.width;
            int height1 = tex1.height;
            int width2 = tex2.width;
            int height2 = tex2.height;

            int matchCount = 0;

            switch (direction)
            {
                case MatchDirection.Right:
                    for (int y = 0; y < height1; y++)
                    {
                        Color pixel1 = tex1.GetPixel(width1 - 1, y); // Cạnh phải của tex1
                        Color pixel2 = tex2.GetPixel(0, y);         // Cạnh trái của tex2
                        if (ArePixelsMatching(pixel1, pixel2)) matchCount++;
                    }
                    break;

                case MatchDirection.Left:
                    for (int y = 0; y < height1; y++)
                    {
                        Color pixel1 = tex1.GetPixel(0, y);         // Cạnh trái của tex1
                        Color pixel2 = tex2.GetPixel(width2 - 1, y); // Cạnh phải của tex2
                        if (ArePixelsMatching(pixel1, pixel2)) matchCount++;
                    }
                    break;

                case MatchDirection.Top:
                    for (int x = 0; x < width1; x++)
                    {
                        Color pixel1 = tex1.GetPixel(x, height1 - 1); // Cạnh trên của tex1
                        Color pixel2 = tex2.GetPixel(x, 0);           // Cạnh dưới của tex2
                        if (ArePixelsMatching(pixel1, pixel2)) matchCount++;
                    }
                    break;

                case MatchDirection.Bottom:
                    for (int x = 0; x < width1; x++)
                    {
                        Color pixel1 = tex1.GetPixel(x, 0);           // Cạnh dưới của tex1
                        Color pixel2 = tex2.GetPixel(x, height2 - 1); // Cạnh trên của tex2
                        if (ArePixelsMatching(pixel1, pixel2)) matchCount++;
                    }
                    break;
            }
            return matchCount;
        }

        bool ArePixelsMatching(Color pixel1, Color pixel2)
        {
            return (Mathf.Abs(pixel1.r - pixel2.r) < 0.1f &&
                    Mathf.Abs(pixel1.g - pixel2.g) < 0.1f &&
                    Mathf.Abs(pixel1.b - pixel2.b) < 0.1f);
        }

    }
    public enum MatchDirection
    {
        Right,
        Left,
        Top,
        Bottom
    }
}