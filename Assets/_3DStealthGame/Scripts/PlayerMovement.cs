using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // ✅ NEW

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;
    public InputAction MoveAction;

    // ✅ NEW: Run input
    public InputAction RunAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    // ✅ NEW: Boost settings
    public float runMultiplier = 2f;
    public float boostDuration = 3f;
    public float rechargeTime = 5f;

    // ✅ NEW: Boost tracking
    private float boostTimer = 0f;
    private float rechargeTimer = 0f;
    private bool isBoosting = false;
    private bool isRecharging = false;

    // ✅ NEW: UI Icon
    public GameObject boostIcon;

    // ✅ NEW: Boost Bar
    public Image boostBarFill;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        RunAction.Enable();
        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        // ✅ Handle boost input
        if (RunAction.IsPressed() && !isRecharging && boostTimer < boostDuration)
        {
            isBoosting = true;
            boostTimer += Time.deltaTime;

            if (boostTimer >= boostDuration)
            {
                isBoosting = false;
                isRecharging = true;
            }
        }
        else
        {
            isBoosting = false;
        }

        // ✅ Handle recharge
        if (isRecharging)
        {
            rechargeTimer += Time.deltaTime;

            if (rechargeTimer >= rechargeTime)
            {
                rechargeTimer = 0f;
                boostTimer = 0f;
                isRecharging = false;
            }
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MoveRotation(m_Rotation);

        // ✅ Apply boost speed
        float currentSpeed = isBoosting ? walkSpeed * runMultiplier : walkSpeed;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);

        // ✅ UI icon logic
        if (boostIcon != null)
        {
            boostIcon.SetActive(!isRecharging);
        }

        // ✅ NEW: Update boost bar
        if (boostBarFill != null)
        {
            float fillValue;

            if (isRecharging)
            {
                // refill
                fillValue = rechargeTimer / rechargeTime;
            }
            else
            {
                // drain
                fillValue = 1f - (boostTimer / boostDuration);
            }

            boostBarFill.fillAmount = Mathf.Clamp01(fillValue);

            // ✅ OPTIONAL: color feedback
            if (isBoosting)
                boostBarFill.color = Color.green;
            else if (isRecharging)
                boostBarFill.color = Color.red;
            else
                boostBarFill.color = Color.white;
        }
    }
}