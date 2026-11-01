using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Visuals")]
    [SerializeField] private bool defaultFacingRight = true;
    [SerializeField] private Sprite idleSprite;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private WorldInputGate inputGate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputGate = GetComponent<WorldInputGate>();
        ShowIdle();
    }

    private void Update()
    {
        if (inputGate != null && inputGate.IsBlocked)
        {
            moveInput = Vector2.zero;
            ShowIdle();
            return;
        }

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
        bool isMoving = moveInput.magnitude > 0.1f;
        SetWalking(isMoving);
        HandleSpriteFlipping();
    }

    private void FixedUpdate()
    {
        if (inputGate != null && inputGate.IsBlocked) { rb.linearVelocity = Vector2.zero; return; }
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // The Animator only runs while walking; standing still shows a static sprite.
    private void SetWalking(bool walking)
    {
        if (animator != null) animator.enabled = walking;
        if (walking) return;
        ShowIdle();
    }

    private void ShowIdle()
    {
        if (spriteRenderer == null) return;
        if (idleSprite != null) spriteRenderer.sprite = idleSprite;
    }

    private void HandleSpriteFlipping()
    {
        if (spriteRenderer == null) return;

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
