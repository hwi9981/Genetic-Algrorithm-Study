using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestBasicGAWithCanvas : TestBasicGA
{
    [Header("UI")]
    [SerializeField] private TMP_Text _generationText;
    [SerializeField] private TMP_Text _bestFitnessText;
    [SerializeField] private TMP_Text _bestGenesText;

    [SerializeField] private TMP_Text _populationText;
    
    [SerializeField] private float _threshold = 0.25f;// update new generation every _threshold seconds to see what's happening
    private float _timeCounter = 0;
    protected override void Update()
    {
        _timeCounter += Time.deltaTime;
        if (_timeCounter >= _threshold)
        {
            _timeCounter = 0;
            base.Update();
            _generationText.text = _ga.Generation.ToString();
            _bestFitnessText.text = _ga.BestFitness.ToString();
            _bestGenesText.text = GenesToString(_ga.BestGenes);

            _populationText.text = "";
            foreach (var individual in _ga.Population)
            {
                _populationText.text += GenesToString(individual.Genes) + "\n";
            }
        }
    }

    string GenesToString(char[] genes)
    {
        string result = "";
        foreach (var gene in genes)
        {
            result += gene.ToString();
        }
        return result;
    }
}
