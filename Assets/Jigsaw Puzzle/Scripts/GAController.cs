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
            for (int i = 0; i < _boardGen.GetTiles().Count; i++)
            {
                _boardGen.GetTiles()[i].CopyData(genes[i]);
                _boardGen.StickTileToBoard(_boardGen.GetTiles()[i], _boardGen.GetTiles()[i].data.current.x, _boardGen.GetTiles()[i].data.current.y);
            }
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
                    if (Mathf.Approximately(_ga.BestFitness, 1))
                    {
                        // Debug.Log("Result: " + _ga.Generation + " - " + _ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
                        enabled = false;
                        CheckComplete();
                    }
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
                if (dna.Genes[i].IsCorrect())
                {
                    score += 1;
                }
            }

            score /= _boardGen.rows * _boardGen.columns * 1.0f;
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
    }
}