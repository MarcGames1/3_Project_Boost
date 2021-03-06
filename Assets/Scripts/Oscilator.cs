﻿using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period=2f;

    //todo remove from inspector
    [Range(0,1)][SerializeField]float movementFactor; // 0 for not moved 1 for fully moved

   Vector3 startingPos;
    void Start()
    {
        startingPos = transform.position;
    }

   
    void Update()
    {
        float cycles = Time.time / period; //grows continually from 0
        if(period <= Mathf.Epsilon) { return; }
        const float tau = Mathf.PI * 2; // ~6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1
        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
        
}
