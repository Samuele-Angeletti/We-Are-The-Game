using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private GameInput inputActions;
    new private Rigidbody2D rigidbody;
    [Header("Data")]
    public int medicine;
    public float stamina;
    public int medicineCap;
    public float staminaCap;
    public float moveSpeed;
    [Header("State")]
    public bool occupied;
    public City lastVisitedCity;
    [Header("Input")]
    public Vector2 movementInput;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        inputActions = new GameInput();
        inputActions.Enable();

        inputActions.ActionMap.Movement.performed += (callback) => movementInput = callback.ReadValue<Vector2>();
        inputActions.ActionMap.Movement.canceled += (callback) => movementInput = Vector2.zero;
    }

    void Update()
    {
        if (occupied) return;

        rigidbody.linearVelocity = (movementInput * moveSpeed).normalized;
        if (movementInput.magnitude > 0)
        {
            stamina -= Time.deltaTime;
        }
    }
}
