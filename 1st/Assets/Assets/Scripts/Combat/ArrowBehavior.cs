using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] private AudioSource archerAudioSource;
    [SerializeField] private AudioClip arrowSound;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float launchForce = 1000f;
    [SerializeField] private float noHitLifetime = 5f;

    private SoldierAI soldierAI;
    private GameObject currentArrow;

    private void Start()
    {
        soldierAI = GetComponent<SoldierAI>();
    }

    public void LaunchArrow()
    {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody arrowRigidbody = currentArrow.GetComponent<Rigidbody>();

        arrowRigidbody.useGravity = false;

        arrowRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        arrowRigidbody.AddForce(arrowSpawnPoint.forward * launchForce, ForceMode.Impulse);

        Destroy(currentArrow, noHitLifetime);
        Debug.Log("Arrow launched");
    }

    public void PlayArrowSound()
    {
        archerAudioSource.PlayOneShot(arrowSound);
    }

    public void SetIsAttackingTrue()
    {
        soldierAI.isAttacking = true;
    }

    public void SetIsAttackingFalse()
    {
        soldierAI.isAttacking = false;
    }
}
