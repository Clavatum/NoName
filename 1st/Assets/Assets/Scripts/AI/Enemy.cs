using UnityEngine;

public class Enemy : BaseUnit
{
    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Soldier"); // Hedef asker olmalý
    }

    // Düþmanlara özgü davranýþlarý burada ekleyebiliriz
}
