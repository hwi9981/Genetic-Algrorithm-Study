using System;
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

    [Header("Other")] 
    [SerializeField] private int numCharsPerText = 15000;
    [SerializeField] private Text targetText;
    [SerializeField] private Text bestText;
    [SerializeField] private Text bestFitnessText;
    [SerializeField] private Text numGenerationsText;
    [SerializeField] private Transform populationTextParent;
    [SerializeField] private Text textPrefab;
    
    private GeneticAlgorithm<char> ge;

    void Start()
    {
        // targetText.text = targetString;
        ge = new GeneticAlgorithm<char>(populationSize, targetString.Length, mutationRate, GetRandomCharacter, FitnessFunction);
    }

    private void Update()
    {
        ge.NewGeneration();
        if (Mathf.Approximately(ge.BestFitness, 1))
        {
            Debug.Log(ge.Generation);
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
        DNA<char> dna = ge.Population[index];
        for (int i = 0; i < dna.Genes.Length; i++)
        {
            if (dna.Genes[i] == targetString[i])
            {
                score += 1;
            }
        }
        score /= targetString.Length;
        return score;
    }
}