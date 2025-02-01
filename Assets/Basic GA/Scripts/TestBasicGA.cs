using System;
using System.Linq;
using BasicGA;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestBasicGA : MonoBehaviour
{
    [Header("Genetic Algorithm Settings")] 
    [SerializeField] protected string targetString = "To be, or not to be";
    protected const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|!#$%&/()=?[]{}+-*'_ ";
    [SerializeField] protected int populationSize = 200;
    [SerializeField] protected float mutationRate = 0.05f;
    
    protected GeneticAlgorithm<char> _ga;

    void Start()
    {
        // targetText.text = targetString;
        _ga = new GeneticAlgorithm<char>(populationSize, targetString.Length, mutationRate, GetRandomCharacter, FitnessFunction);
    }

    protected virtual void Update()
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
