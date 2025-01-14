using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jigsaw_Puzzle.Scripts;
using TMPro;
using UnityEngine;

namespace Jigsaw_Puzzle.Scripts
{
    public class AIController : GameController
    {
        [SerializeField] private TMP_Text _generationText;
        [SerializeField] private TMP_Text _bestFitnessText;
        
        [Header("GA settings")]
        [SerializeField] private int populationSize = 200;
        [SerializeField] private float mutationRate = 0.05f;
        [SerializeField] private float _speed = 1f;
        
        private GeneticAlgorithm<TileData> _ga;
        private List<Tile> _bestTiles = new List<Tile>();
        private float _timeCounter;
        private void Start()
        {
            Invoke(nameof(StartAlgorithm), 0.5f);
        }
        void StartAlgorithm()
        {
            CloneOriginTiles();
            
            _ga = new GeneticAlgorithm<TileData>(populationSize, _boardGen.rows * _boardGen.columns, mutationRate,
                GetRandomTile, EvaluateFitness);
        }
        void CloneOriginTiles()
        {
            // _bestTiles = _boardGen.GetTiles();
            _bestTiles.Clear();
            // create clone to display the result is the best genes of GA, because sometime gene overlap with each other
            foreach (var tile in _boardGen.GetTiles())
            {
                tile.gameObject.SetActive(false);
                var clone = Instantiate(tile, tile.transform.parent);
                clone.CopyData(tile.data);
                clone.gameObject.SetActive(true);
                _bestTiles.Add(clone);
            }
        }

        void DisplayGenes(List<TileData> genes)
        {
            _boardGen.RemoveAllTilesFromSlots();
            for (int i = 0; i < _bestTiles.Count; i++)
            {
                _bestTiles[i].CopyData(genes[i]);
                _boardGen.StickTileToBoard(_bestTiles[i], _bestTiles[i].data.current.x, _bestTiles[i].data.current.y);
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
                else
                {
                    score -= 0.1f; //improve fitness, higher score means more character correct
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
        public override void ResetGame()
        {
            base.ResetGame();
            _boardGen.ArrangeAllPiecesBelowImage(_bestTiles);
        }
    }

}
