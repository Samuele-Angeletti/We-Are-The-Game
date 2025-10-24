using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D nonTriggerArea;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Camera mainCamera;

    private Player player;

    /// <summary>
    /// Permette di muovere la camera se il player può muoversi ma non lo sta facendo e il mouse è al di fuori dell'area non trigger
    /// </summary>
    private bool CanMoveCamera
    {
        get
        {
            bool result = player.MovementInput == Vector2.zero && player.CanMove == true && !nonTriggerArea.OverlapPoint(mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            if (result)
            {
                // fai partire la camera dalla posizione del player
                if (!wasMoving && cinemachineCamera.Target.TrackingTarget == player.transform) transform.position = player.transform.position;
                cinemachineCamera.Target.TrackingTarget = transform;
            }

            return result;
        }
    }

    private bool canMove;
    private bool wasMoving = false;

    [Header("Settings")]
    //public GameEventsStats stats;               // contiene Stamina, StaminaCap, Medicines, ecc.
    [Tooltip("Velocita' di movimento in units/s")]
    public float moveSpeed = 4f;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        canMove = CanMoveCamera;
        wasMoving = canMove;
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // calcola la velocity desiderata
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 center = mainCamera.ScreenToWorldPoint(new(Screen.width / 2, Screen.height / 2));


        Vector2 desired = (worldMousePos - center);
        if (desired.sqrMagnitude > 0.001f)
        {
            desired = desired.normalized * moveSpeed;
        }
        else
        {
            desired = Vector2.zero;
        }

        // Imposta la velocit� (uso linearVelocity come richiesto)
        rb.linearVelocity = desired;
    }
}
