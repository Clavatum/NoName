using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Arrow prefab to instantiate
    [SerializeField] private Transform arrowSpawnPoint; // Position and direction to launch from
    [SerializeField] private float launchForce = 1000f; // Speed of the arrow when launched
    [SerializeField] private float noHitLifetime = 5f; // Time before destruction if no collision
    [SerializeField] private float stuckLifetime = 3f; // Time to destroy the arrow after "sticking"

    public DamageTrigger damageTrigger;

    private GameObject currentArrow; // Reference to the instantiated arrow
    private Rigidbody arrowRigidbody;
    private bool isStuck = false;

    // Called by an animation event to make the arrow visible
    public void ShowArrow()
    {
        if (currentArrow != null)
        {
            //currentArrow.SetActive(true);
        }
    }

    // Called by an animation event to launch the arrow
    public void LaunchArrow()
    {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        arrowRigidbody = currentArrow.GetComponent<Rigidbody>();

        // Ensure the arrow starts invisible and unaffected by gravity
        //currentArrow.SetActive(false);
        arrowRigidbody.useGravity = false;

        // Apply launch force
        arrowRigidbody.AddForce(arrowSpawnPoint.forward * launchForce);
        damageTrigger.EnableCollider();
        Debug.Log("launched");

        // Schedule destruction if no collision occurs
        Invoke(nameof(DestroyArrow), noHitLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Return if already stuck or if colliding with Player/Soldier tags
        if (isStuck || other.CompareTag("Player") || other.CompareTag("Soldier"))
        {
            return;
        }

        // "Stick" the arrow to the hit object and disable movement
        isStuck = true;
        arrowRigidbody.isKinematic = true;
        arrowRigidbody.velocity = Vector3.zero;
        arrowRigidbody.angularVelocity = Vector3.zero;

        // Destroy arrow after a delay to simulate "stuck" effect
        Invoke(nameof(DestroyArrow), stuckLifetime);
    }

    private void DestroyArrow()
    {
        if (currentArrow != null)
        {
            Destroy(currentArrow);
        }
    }
}
