using UnityEngine;

public class EnemyTurretController : MonoBehaviour
{
    [Header("Turret Settings")]
    [SerializeField] private float maxRotationAngle = 70f;
    [SerializeField] private float rotationSpeed = 180f; // градусов в секунду
    [SerializeField] private float aiUpdateRate = 0.2f; // частота обновления AI

    [Header("References")]
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;

    private Camera mainCamera;
    private EnemyAIController enemyAI;
    private float lastAIUpdate;
    private Vector2 lastTargetDirection;

    private void Start()
    {
        mainCamera = Camera.main;
        enemyAI = GetComponentInParent<EnemyAIController>();

        if (enemyTransform == null)
            enemyTransform = transform.parent; // Set to parent transform if not assigned

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (Time.time - lastAIUpdate >= aiUpdateRate)
        {
            UpdateAITarget();
            lastAIUpdate = Time.time;
        }

        HandleTurretRotation();
    }

    private void UpdateAITarget()
    {
        if (target == null) return;

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        lastTargetDirection = directionToTarget;
    }

    private void HandleTurretRotation()
    {
        if (enemyTransform == null || lastTargetDirection == Vector2.zero) return;

        Vector2 enemyForward = enemyTransform.right;
        float angleToTarget = Vector2.SignedAngle(enemyForward, lastTargetDirection);

        // Ограничиваем угол поворота башни
        float clampedAngle = Mathf.Clamp(angleToTarget, -maxRotationAngle, maxRotationAngle);

        // Если цель вне угла обзора башни, просто поворачиваем башню на максимальный угол
        if (Mathf.Abs(angleToTarget) > maxRotationAngle)
        {
            clampedAngle = Mathf.Sign(angleToTarget) * maxRotationAngle;
        }

        // Поворачиваем башню
        float targetRotation = enemyTransform.eulerAngles.z + clampedAngle;
        float currentRotation = transform.eulerAngles.z;

        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    public bool IsAimedAtTarget()
    {
        if (enemyTransform == null || lastTargetDirection == Vector2.zero) return false;

        Vector2 enemyForward = enemyTransform.right;
        float angleToTarget = Vector2.SignedAngle(enemyForward, lastTargetDirection);

        return Mathf.Abs(angleToTarget) <= maxRotationAngle;
    }

    public Vector2 GetFireDirection()
    {
        return transform.right;
    }

    public Transform GetFirePoint()
    {
        return firePoint != null ? firePoint : transform;
    }
}