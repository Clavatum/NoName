using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab; // Atýlacak ok prefab'ý
    [SerializeField] private Transform characterTransform; // Karakterin pozisyon ve yön bilgisi
    [SerializeField] private float launchForce = 1000f; // Fýrlatma kuvveti
    private Rigidbody arrowRigidbody;
    private bool isLaunched = false;
    private bool isStuck = false;

    private void Awake()
    {
        arrowRigidbody = GetComponent<Rigidbody>();
    }

    public void LaunchArrow()
    {
        if (!isLaunched)
        {
            GameObject newArrow = Instantiate(arrowPrefab, characterTransform.position, characterTransform.rotation);
            Rigidbody newArrowRigidbody = newArrow.GetComponent<Rigidbody>();
            newArrowRigidbody.isKinematic = false;
            newArrowRigidbody.AddForce(characterTransform.forward * launchForce);

            ArrowBehavior arrowBehavior = newArrow.GetComponent<ArrowBehavior>();
            arrowBehavior.isLaunched = true;

            Destroy(newArrow, 5f); // Ok 5 saniye sonra yok olacak, çarpmýþ olsa da olmasa da
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLaunched && !isStuck)
        {
            isStuck = true;
            arrowRigidbody.isKinematic = true;
            arrowRigidbody.velocity = Vector3.zero;
            arrowRigidbody.angularVelocity = Vector3.zero;

            Destroy(gameObject, 3f); // Çarptýðýnda okun 3 saniye sonra yok olmasý saðlanýyor
        }
    }

    public void ActivateArrow()
    {
        arrowPrefab.SetActive(true);
    }

    public void DeactivateArrow()
    {
        arrowPrefab.SetActive(false);
    }
}
