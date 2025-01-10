using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DNA<T>
{
    public T[] Genes { get; private set; }
    public float Fitness { get; private set; }

    private Func<T> _getRandomGene;
    private Func<int, float> _fitnessFunction; 
    public DNA(int size, Func<T> getRandomGene, Func<int, float> fitnessFunction)
    {
        Genes = new T[size];
        _getRandomGene = getRandomGene;
        _fitnessFunction = fitnessFunction;
    }

    public void CreateRandomGenes()
    {
        if (_getRandomGene == null)
        {
            Debug.LogError("getRandomGene is null");
            return;
        }
        for (int i = 0; i < Genes.Length; i++)
        {
            Genes[i] = _getRandomGene();
        }
    }
    public float CalculateFitness(int index)
    {
        Fitness = _fitnessFunction(index);
        return Fitness;
    }

    public DNA<T> Crossover(DNA<T> otherParent)
    {
        DNA<T> child = new DNA<T>(Genes.Length, _getRandomGene, _fitnessFunction);
        for (int i = 0; i < Genes.Length; i++)
        {
            child.Genes[i] = Random.value > 0.5f ?  Genes[i] : otherParent.Genes[i];
        }
        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                Genes[i] = _getRandomGene();
            }
        }
    }
}
