using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    TowerInputActions towerInputActions;

    [Header("Spell Settings")]
    public GameObject spellIndicatorPrefab; // The 2D circle sprite prefab
    public GameObject spellVFXPrefab;      // VFX to be instantiated on spell cast
    public float spellRadius = 5f;         // Area of effect radius
    public float spellDamage = 50f;        // Damage to enemies
    public float spellCooldown = 5f;       // Cooldown duration
    public float spellCost = 20f;          // Gold cost to use the spell

    [Header("References")]
    public Camera mapCamera;               // Camera used for the raycast
    public LayerMask groundLayer;          // Layer mask to ensure raycast hits only the ground
    public Button spellActivateButton;     // UI Button to activate spell indicator

    private GameObject spellIndicator;     // The active spell indicator
    private bool isSpellActive = false;    // Is the spell indicator active?
    public float cooldownTimer = 0f;       // Cooldown timer for the spell

    private GameStatsManager statsManager;

    void Awake()
    {
        towerInputActions = new TowerInputActions();
        towerInputActions.Enable();

        if (spellActivateButton != null)
        {
            spellActivateButton.onClick.AddListener(ActivateSpellIndicator); // Attach button click listener
        }
    }

    void Start()
    {
        statsManager = GameStatsManager.Instance;

        // Instantiate the spell indicator and deactivate it
        spellIndicator = Instantiate(spellIndicatorPrefab);
        spellIndicator.SetActive(false);
    }

    void Update()
    {
        HandleCooldown();

        if (isSpellActive)
        {
            MoveSpellIndicator();

            if (towerInputActions.Actions.MouseLeft.triggered)
            {
                CastSpell();
            }
        }
    }

    /// <summary>
    /// Activates the spell indicator if enough gold is available and no cooldown is active.
    /// </summary>
    void ActivateSpellIndicator()
    {
        if (cooldownTimer > 0)
        {
            Debug.Log("Spell is on cooldown!");
            return;
        }

        if (statsManager.totalGold >= spellCost)
        {
            isSpellActive = true;
            spellIndicator.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to use the spell!");
        }
    }

    /// <summary>
    /// Moves the spell indicator to follow the mouse cursor, restricted to the ground layer.
    /// </summary>
    void MoveSpellIndicator()
    {
        Ray ray = mapCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Restrict position to the ground layer
            spellIndicator.transform.position = new Vector3(hit.point.x, 2.25f, hit.point.z);
            spellIndicator.transform.localScale = new Vector3(spellRadius, 1, spellRadius);
        }
    }

    /// <summary>
    /// Casts the spell, applies damage to enemies, instantiates VFX, and starts cooldown.
    /// </summary>
    void CastSpell()
    {
        Vector3 spellPosition = spellIndicator.transform.position;

        // Instantiate VFX at the selected position and destroy it after 2 seconds
        if (spellVFXPrefab != null)
        {
            GameObject vfxInstance = Instantiate(spellVFXPrefab, spellPosition, Quaternion.identity);
            Destroy(vfxInstance, 2f); // Destroy the VFX after 2 seconds
        }

        // Cast the spell and deal damage to enemies within the area
        Collider[] hitColliders = Physics.OverlapSphere(spellPosition, spellRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                var health = hitCollider.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(spellDamage, "Player");
                }
            }
        }

        // Deduct gold and start cooldown
        statsManager.SpendGold(spellCost);
        cooldownTimer = spellCooldown;

        // Deactivate the spell indicator
        spellIndicator.SetActive(false);
        isSpellActive = false;

        Debug.Log("Spell cast successfully!");
    }


    void HandleCooldown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
