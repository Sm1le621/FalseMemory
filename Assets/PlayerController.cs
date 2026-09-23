using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Считываем WASD
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;
        }

        moveInput = new Vector2(moveX, moveY).normalized;

        // Передаем состояние движения в Animator
        bool isMoving = moveInput.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}