using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Jigsaw_Puzzle.Scripts
{
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
        public void CreateGenes(T[] genes)
        {
            if (Genes.Length != genes.Length)
            {
                Debug.LogError("Genes length is not equal to the length of the given array");
                return;
            }
            Genes = genes;
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
                child.Genes[i] = Random.value > 0.5f ? Genes[i] : otherParent.Genes[i];
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

        //public void Mutate(float mutationRate)
        //{
        //    // Tạo một mảng mới và sao chép dữ liệu từ Genes
        //    T[] newGenes = new T[Genes.Length];
        //    Array.Copy(Genes, newGenes, Genes.Length);

        //    // Áp dụng đột biến bằng cách hoán đổi các phần tử ngẫu nhiên
        //    for (int i = 0; i < newGenes.Length; i++)
        //    {
        //        if (Random.value < mutationRate)
        //        {
        //            int randomIndex = Random.Range(0, newGenes.Length);
        //            (newGenes[i], newGenes[randomIndex]) = (newGenes[randomIndex], newGenes[i]);
        //        }
        //    }

        //    // Cập nhật từng phần tử của Genes từ newGenes
        //    for (int i = 0; i < Genes.Length; i++)
        //    {
        //        Genes[i] = newGenes[i];
        //    }
        //}


    }

}
