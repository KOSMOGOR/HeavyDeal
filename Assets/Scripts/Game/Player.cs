using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    int currentOxygenTank = 20;
    int oxygenTanks = 2;
    List<CardInstance> deck = new();
    List<CardInstance> hand = new();
    List<CardInstance> discard = new();

    void Start() {
        
    }

    void Update() {
        
    }
}
