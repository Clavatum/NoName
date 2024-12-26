using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    TowerInputActions towerInputActions;

    [Header("Spell Settings")]
    public GameObject spellIndicatorPrefab;
    public float spellRadius = 5f;
    public float spellDamage = 50f;
    public float spellCooldown = 5f;
    public float spellCost = 20f;

    [Header("Meteor Settings")]
    public GameObject meteorPrefab;
    public float meteorSpawnDelay = 0f; // set a value after get an meteor vfx

    [Header("References")]
    public Camera mapCamera;
    public LayerMask groundLayer;
    public Button spellActivateButton;

    private GameObject spellIndicator;
    private bool isSpellActive = false;
    public float cooldownTimer = 0f;

    private GameStatsManager statsManager;

    void Awake()
    {
        towerInputActions = new TowerInputActions();
        towerInputActions.Enable();

        if (spellActivateButton != null)
        {
            spellActivateButton.onClick.AddListener(ActivateSpellIndicator);
        }
    }

    void Start()
    {
        statsManager = GameStatsManager.Instance;

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
            Debug.Log("Spell indicator activated.");
        }
        else
        {
            Debug.Log("Not enough gold to use the spell!");
        }
    }

    void MoveSpellIndicator()
    {
        Ray ray = mapCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            spellIndicator.transform.position = new Vector3(hit.point.x, 2.25f, hit.point.z);
            spellIndicator.transform.localScale = new Vector3(spellRadius, 1, spellRadius);
        }
    }

    void CastSpell()
    {
        Vector3 spellPosition = spellIndicator.transform.position;

        Debug.Log($"Casting spell at position: {spellPosition}");

        if (meteorPrefab != null)
        {
            Debug.Log("Spawning meteor...");
            StartCoroutine(SpawnMeteorWithDelay(spellPosition, meteorSpawnDelay));
        }
        else
        {
            Debug.LogError("Meteor prefab is not assigned in the inspector!");
        }

        Collider[] hitColliders = Physics.OverlapSphere(spellPosition, spellRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                var health = hitCollider.GetComponent<HealthSystem>();
                if (health != null)
                {
                    Debug.Log($"Damaging enemy: {hitCollider.name}");
                    health.TakeDamage(spellDamage, "Player");
                }
            }
        }

        statsManager.SpendGold(spellCost);
        cooldownTimer = spellCooldown;

        spellIndicator.SetActive(false);
        isSpellActive = false;

        Debug.Log("Spell cast successfully!");
    }

    IEnumerator SpawnMeteorWithDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 meteorSpawnPosition = position + new Vector3(0, 0, 0);
        Debug.Log($"Spawning meteor at position: {meteorSpawnPosition}");

        GameObject meteorInstance = Instantiate(meteorPrefab, meteorSpawnPosition, Quaternion.identity);

        if (meteorInstance != null)
        {
            Debug.Log("Meteor spawned successfully.");
            Destroy(meteorInstance, 2f);
        }
        else
        {
            Debug.LogError("Failed to spawn meteor.");
        }
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
