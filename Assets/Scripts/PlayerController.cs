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
    private WorldInputGate inputGate;
    private bool directionalAnimator;
    private BattleUnit unit;
    private Vector2 lastDirection = Vector2.down;
    public Vector2 MovementInput => moveInput;
    public Vector2 LastDirection => lastDirection;
    public float MovementSpeed => Mathf.Clamp(moveSpeed+(unit!=null?unit.MovementAdjustment:0),0,
        unit!=null && unit.UsesPrimaryAttributes?Mathf.Max(0,unit.Attributes.rules.maximumMovementSpeed):float.MaxValue);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputGate = GetComponent<WorldInputGate>();
        unit = GetComponent<BattleUnit>();
        directionalAnimator = animator != null &&
            HasFloat("MoveX") && HasFloat("MoveY") && HasFloat("LastMoveX") && HasFloat("LastMoveY");
        UpdateAnimation();
    }

    private void Update()
    {
        if (inputGate != null && inputGate.IsBlocked)
        {
            moveInput = Vector2.zero;
            UpdateAnimation();
            return;
        }
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
        if (moveInput.sqrMagnitude > 0f) lastDirection = moveInput;

        // 2. Управление анимацией
        UpdateAnimation();

        // 3. Зеркалирование по A/D (влево/вправо)
        if (!directionalAnimator) HandleSpriteFlipping();
    }

    private bool HasFloat(string parameter)
    {
        foreach (var entry in animator.parameters)
            if (entry.name == parameter && entry.type == AnimatorControllerParameterType.Float) return true;
        return false;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0f);
        if (!directionalAnimator) return;
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("LastMoveX", lastDirection.x);
        animator.SetFloat("LastMoveY", lastDirection.y);
        // Directional clips own the facing; horizontal flipping would corrupt real diagonal art.
        spriteRenderer.flipX = false;
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (inputGate != null && inputGate.IsBlocked) { rb.linearVelocity = Vector2.zero; return; }
        // Физическое перемещение
        rb.MovePosition(rb.position + moveInput * MovementSpeed * Time.fixedDeltaTime);
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
