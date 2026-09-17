using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Salto y Física")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallMultiplier = 2.5f; // Cae más rápido para evitar sensación de flotar
    [SerializeField] private float lowJumpMultiplier = 2f; // Salto corto si sueltas rápido el botón

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Controles Móviles")]
    [SerializeField] private FixedJoystick joystick;

    private Rigidbody2D rb;
    private Animator anim; // Referencia al Animator
    private bool isGrounded;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Obtener el componente Animator
    }

    private void Update()
    {
        // 1. Lectura del Joystick táctil o Teclado
        if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.1f)
        {
            horizontalInput = joystick.Horizontal;
        }
        else
        {
            horizontalInput = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    horizontalInput = -1f;
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    horizontalInput = 1f;
            }
        }

        // 2. Detección del suelo
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 3. Salto desde teclado
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }

        // 4. Girar el personaje horizontalmente
        if (horizontalInput > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

        // 5. Enviar la velocidad al Animator para cambiar entre Idle y Run
        if (anim != null)
        {
            anim.SetFloat("horizontal", Mathf.Abs(horizontalInput));
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento horizontal
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // --- FÍSICA MEJORADA DE SALTO Y CAÍDA ---
        if (rb.linearVelocity.y < 0)
        {
            // Aumenta la velocidad de caída (evita el efecto de flotar hacia abajo)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !(Keyboard.current != null && Keyboard.current.spaceKey.isPressed))
        {
            // Permite saltos cortitos si no mantienes presionado
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}