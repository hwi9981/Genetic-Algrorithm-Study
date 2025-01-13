using System;
using System.Linq;
using BasicGA;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestShakespeare : MonoBehaviour
{
    [Header("Genetic Algorithm Settings")] 
    [SerializeField] private string targetString = "To be, or not to be";
    [SerializeField] private string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|!#$%&/()=?[]{}+-*'_ ";
    [SerializeField] private int populationSize = 200;
    [SerializeField] private float mutationRate = 0.01f;
    
    private GeneticAlgorithm<char> _ga;

    void Start()
    {
        // targetText.text = targetString;
        _ga = new GeneticAlgorithm<char>(populationSize, targetString.Length, mutationRate, GetRandomCharacter, FitnessFunction);
    }

    private void Update()
    {
        _ga.NewGeneration();
        Debug.Log("Gen "+ _ga.Generation + ": " + _ga.BestFitness);
        Debug.Log(_ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
        if (Mathf.Approximately(_ga.BestFitness, 1))
        {
            Debug.Log("Result: " + _ga.Generation + " - " + _ga.BestGenes.Aggregate("", (current, t) => current + (t + " ")));
            enabled = false;
        }
    }

    private char GetRandomCharacter()
    {
        return validCharacters[Random.Range(0, validCharacters.Length)];
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        DNA<char> dna = _ga.Population[index];
        for (int i = 0; i < dna.Genes.Length; i++)
        {
            if (dna.Genes[i] == targetString[i])
            {
                score += 1;
            }else
            {
                score -= 0.1f; //improve fitness, higher score means more character correct
            }
        }
        score /= targetString.Length;
        return score;
    }
}
