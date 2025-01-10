using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticAlgorithm<T>
{
    public List<DNA<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float MutationRate { get; set; }
    
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }
    
    private List<DNA<T>> _newPopulation = new List<DNA<T>>();
    private float _fitnessSum = 0;

    public GeneticAlgorithm(int populationSize, int dnaSize, float mutationRate, Func<T> getRandomGene, Func<int, float> fitnessFunction)
    {
        Generation = 1;
        MutationRate = mutationRate;
        Population = new List<DNA<T>>(populationSize);
        BestGenes = new T[dnaSize];
        for (int i = 0; i < populationSize; i++)
        {
            var dna = new DNA<T>(dnaSize, getRandomGene, fitnessFunction);
            dna.CreateRandomGenes();
            Population.Add(dna);
        }
    }

    public void NewGeneration()
    {
        if (Population.Count <= 0)
        {
            Debug.Log("Population is empty, cannot create new generation");
            return;
        }
        CalculateFitness();
        _newPopulation.Clear();
        for (int i = 0; i < Population.Count; i++)
        {
            DNA<T> parent1 = ChooseParent();
            DNA<T> parent2 = ChooseParent();
            var child = parent1.Crossover(parent2);
            child.Mutate(MutationRate);
            _newPopulation.Add(child);
        }
        Population = new List<DNA<T>>(_newPopulation);
        Generation++;
        Debug.Log("Gen "+ Generation + ": " + BestFitness);
        Debug.Log(BestGenes.Aggregate("", (current, t) => current + (t + " ")));
    }

    public void CalculateFitness()
    {
        _fitnessSum = 0;
        DNA<T> best = Population[0];
        for (int i = 0; i < Population.Count; i++)
        {
            _fitnessSum += Population[i].CalculateFitness(i);
            if (Population[i].Fitness > best.Fitness)
            {
                best = Population[i];
            }
        }
        BestFitness = best.Fitness;
        best.Genes.CopyTo(BestGenes, 0);
    }
    private DNA<T> ChooseParent()
    {
        var randomValue = Random.value * _fitnessSum;
        for (int i = 0; i < Population.Count; i++)
        {
            if (randomValue < Population[i].Fitness)
            {
                return Population[i];
            }
            randomValue -= Population[i].Fitness;
        }
        return null;
    }
    
}