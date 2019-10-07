﻿using System;
using UnityEngine;

public class Player : SimpleStateMachine
{
    [SerializeField] public Pickable pickable;
    [SerializeField] public Picker picker;
    [SerializeField] public AirMovement airMovement;
    [SerializeField] public GroundMovement groundMovement;
    [SerializeField] public InputHandler inputHandler;

    private enum PlayerStates { Idle, Running, Flying, Captured }

    private void Start()
    {
        pickable.OnPicked.AddListener(() => currentState = PlayerStates.Captured);
        pickable.OnThrowed.AddListener(() => currentState = PlayerStates.Flying);
        pickable.OnHit.AddListener(OnHit);
        
        currentState = PlayerStates.Idle;
    }

    private void OnHit(Pickable pickable, GameObject other) => Land();

    private void Land()
    {
        currentState = PlayerStates.Idle;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
    
    public void OnColliderEnter(Type type)
    {
        Debug.LogWarning("OnColliderEnter : " + type.Name);
    }

    protected override void EarlyGlobalSuperUpdate()
    {
        if (inputHandler.ThrowButton)
            picker.Throw();

        if (Input.GetKeyDown(KeyCode.R))
        {
            var bombs = FindObjectsOfType<Pickable>();
            foreach (var b in bombs)
                Physics.IgnoreCollision(GetComponent<Collider>(), b.Collider, false);
        }
    }

    protected override void LateGlobalSuperUpdate()
    {

    }

    private void Running_FixedUpdate()
    {
        if (inputHandler.Direction != Vector3.zero) //Comment this to enable always moving mechanic
            groundMovement.Move(inputHandler.Direction);
        else
            currentState = PlayerStates.Idle;
    }


    private void Idle_EnterState()
    {
        pickable.SetIdle();
        pickable.IsPickBlocked = false;
        picker.Unavaiable = false;
    }

    private void Idle_Update()
    {
        if (inputHandler.Direction != Vector3.zero)
            currentState = PlayerStates.Running;
    }

    private void Idle_ExitState()
    {
        pickable.IsPickBlocked = true;
    }

    private void Captured_EnterState()
    {
        picker.Unavaiable = true;
    }

    private void Captured_Update()
    {
        if (inputHandler.ThrowButton)
        {
            pickable.GetRelease();
            currentState = PlayerStates.Idle;
        }
    }

    private void Captured_ExitState()
    {
    }

    private void Flying_Update()
    {
        if (inputHandler.ThrowButton) 
            Land();
    }

    private void OnDrawGizmos()
    {
        var from = transform.position + Vector3.up;
        var to = from + transform.forward * 1.5f;
        Gizmos.DrawLine(from, to);
    }

}
