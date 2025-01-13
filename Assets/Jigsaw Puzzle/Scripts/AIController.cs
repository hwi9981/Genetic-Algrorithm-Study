using System;
using System.Collections;
using System.Collections.Generic;
using Jigsaw_Puzzle.Scripts;
using UnityEngine;

namespace Jigsaw_Puzzle.Scripts
{
    public class AIController : GameController
    {
        [SerializeField] private int populationSize = 200;
        [SerializeField] private float mutationRate = 0.05f;
        private GeneticAlgorithm<Tile> _ga;

        private void Start()
        {
            Invoke(nameof(StartAlgorithm), 0.5f);
        }

        void StartAlgorithm()
        {
            _ga = new GeneticAlgorithm<Tile>(populationSize, _boardGen.rows * _boardGen.columns, mutationRate,
                GetRandomTile, EvaluateFitness);
        }
        private float EvaluateFitness(int index)
        {
            float score = 0;
            DNA<Tile> dna = _ga.Population[index];
            for (int i = 0; i < dna.Genes.Length; i++)
            {
                var row = i / _boardGen.columns;
                var col = i % _boardGen.columns;
                dna.Genes[i].Current = new Vector2Int(row, col);
                if (dna.Genes[i].IsCorrect())
                {
                    score += 1;
                }else
                {
                    score -= 0.1f; //improve fitness, higher score means more character correct
                }
            }
            score /= _boardGen.rows * _boardGen.columns * 1.0f;
            return score;
        }

        public Tile GetRandomTile()
        {
            return _boardGen.GetRandomTile();
        }
        private void Update()
        {
            if (_ga != null)
            {
                _ga.NewGeneration();
                Debug.Log("Gen "+ _ga.Generation + ": " + _ga.BestFitness);
                // Debug.Log(_ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
                if (Mathf.Approximately(_ga.BestFitness, 1))
                {
                    // Debug.Log("Result: " + _ga.Generation + " - " + _ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
                    enabled = false;
                    // var row = index / _boardGen.columns;
                    // var col = index % _boardGen.columns;
                    // _boardGen.SetTile(row, col, _ga.BestGenes[index]);
                    for (int i = 0; i < _ga.BestGenes.Length; i++)
                    {
                        var row = i / _boardGen.columns;
                        var col = i % _boardGen.columns;
                        _boardGen.StickTileToBoard(_ga.BestGenes[i], row, col);
                    }
                }
            }
           
            
        }
    }

}
