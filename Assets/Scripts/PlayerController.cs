using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))] // Скрипт сам добавит компонент, если его нет
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Visual Settings")]
    [SerializeField] private bool defaultFacingRight = true; // Спрайт по умолчанию смотрит вправо?

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 1. Считываем ввод WASD / Стрелки
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

        // 2. Управление анимацией
        bool isMoving = moveInput.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // 3. Зеркалирование по A/D (влево/вправо)
        HandleSpriteFlipping();
    }

    private void FixedUpdate()
    {
        // Физическое перемещение
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleSpriteFlipping()
    {
        if (spriteRenderer == null) return;

        // Меняем направление только при горизонтальном вводе (A/D или стрелки)
        if (moveInput.x < -0.01f)
        {
            SetFacingDirection(false);
        }
        else if (moveInput.x > 0.01f)
        {
            SetFacingDirection(true);
        }
    }

    private void SetFacingDirection(bool lookRight)
    {
        // Логика зависит от того, куда спрайт смотрит изначально
        if (defaultFacingRight)
        {
            // Спрайт изначально смотрит ВПРАВО.
            // Если нужно смотреть ВПРАВО, flipX = false. Если ВЛЕВО, flipX = true.
            spriteRenderer.flipX = !lookRight;
        }
        else
        {
            // Спрайт изначально смотрит ВЛЕВО.
            // Если нужно смотреть ВПРАВО, flipX = true. Если ВЛЕВО, flipX = false.
            spriteRenderer.flipX = lookRight;
        }
    }
}