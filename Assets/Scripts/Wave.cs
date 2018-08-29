using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave  {

    public int WaveCapacity;

    public List<string> Enemies;

    public Wave(int waveCapacity, List<string> enemies)
    {
        WaveCapacity = waveCapacity;
        Enemies = enemies;
    }
    public Wave(){
        
    }
}
