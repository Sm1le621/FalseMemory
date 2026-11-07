using UnityEngine;
using UnityEngine.InputSystem;

// Керування гравцем: рух на WASD / стрілках, анімація ходьби і поворот спрайта.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    // Хеш параметра "isMoving" в Animator (так швидше, ніж щоразу передавати рядок)
    private static readonly int IsMovingParameter = Animator.StringToHash("isMoving");

    [Header("Movement")]
    // Швидкість гравця
    [SerializeField] private float moveSpeed = 4f;

    [Header("Visuals")]
    // Куди дивиться спрайт за замовчуванням (true - вправо)
    [SerializeField] private bool defaultFacingRight = true;
    // Спрайт, коли гравець стоїть на місці
    [SerializeField] private Sprite idleSprite;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    // Куди гравець хоче йти (від -1 до 1 по кожній осі)
    private Vector2 moveInput;
    // "Замок" на керування (наприклад, коли відкритий інвентар)
    private WorldInputGate inputGate;
    // Чи є в аніматорі параметр isMoving
    private bool hasIsMovingParameter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputGate = GetComponent<WorldInputGate>();
        hasIsMovingParameter = HasParameter(IsMovingParameter);
        ShowIdle();
    }

    private void Update()
    {
        // Якщо керування заблоковане (відкрите вікно) - стоїмо
        if (inputGate != null && inputGate.IsBlocked)
        {
            moveInput = Vector2.zero;
            SetWalking(false);
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        // Читаємо клавіатуру
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveX = moveX - 1f;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveX = moveX + 1f;
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                moveY = moveY - 1f;
            }
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                moveY = moveY + 1f;
            }
        }

        // normalized - щоб по діагоналі гравець не біг швидше
        moveInput = new Vector2(moveX, moveY).normalized;

        bool isMoving = false;
        if (moveInput.magnitude > 0.1f)
        {
            isMoving = true;
        }

        SetWalking(isMoving);
        HandleSpriteFlipping();
    }

    // Фізику рухаємо у FixedUpdate
    private void FixedUpdate()
    {
        if (inputGate != null && inputGate.IsBlocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // Аніматор працює тільки коли гравець іде.
    // Коли стоїть - показуємо звичайний статичний спрайт.
    // Параметр isMoving перемикає стани Idle/Walk/Run в Player.controller.
    private void SetWalking(bool walking)
    {
        if (animator != null)
        {
            if (hasIsMovingParameter)
            {
                animator.SetBool(IsMovingParameter, walking);
            }
            animator.enabled = walking;
        }

        if (walking)
        {
            return;
        }

        ShowIdle();
    }

    // Перевіряємо, чи є в аніматорі параметр з таким хешем
    private bool HasParameter(int parameterHash)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
            {
                return true;
            }
        }
        return false;
    }

    // Ставимо спрайт "стоїть"
    private void ShowIdle()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    // Повертаємо спрайт вліво/вправо залежно від напрямку руху
    private void HandleSpriteFlipping()
    {
        if (spriteRenderer == null)
        {
            return;
        }

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
        if (defaultFacingRight)
        {
            spriteRenderer.flipX = !lookRight;
        }
        else
        {
            spriteRenderer.flipX = lookRight;
        }
    }
}
