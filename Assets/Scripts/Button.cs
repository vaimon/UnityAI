using System;
using System.Collections;
using System.Collections.Generic;

using Unity.MLAgents;

using UnityEngine;

public enum ButtonStates {
    Unpressed,
    Pressed
}

public class Button : ButtonActivator
{
    MeshRenderer mesh;
    ButtonStates state = ButtonStates.Unpressed;
    [SerializeField]
    Material pressedMaterial;
    [SerializeField]
    Material unpressedMaterial;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = unpressedMaterial;
    }

    int collisionsCount = 0;

    void OnCollisionEnter(Collision collision) {
        collisionsCount++;
        if (state == ButtonStates.Unpressed) {
            var agent = collision.gameObject.GetComponent<Agent>();
            agent.AddReward(0.1f);
            tag = "pressedButton";
            state = ButtonStates.Pressed;
            mesh.material = pressedMaterial;
            onActivate.Invoke();
        }
    }
    void OnCollisionExit(Collision collision) {
        collisionsCount--;
        if (collisionsCount <= 0 && state == ButtonStates.Pressed) {
            tag = "button";
            var agent = collision.gameObject.GetComponent<Agent>();
            agent.AddReward(-0.1f);
            state = ButtonStates.Unpressed;
            mesh.material = unpressedMaterial;
            onDeactivate.Invoke();
        }
    }
}
