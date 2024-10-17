using UnityEngine;

public class Soldier : BaseUnit
{
    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Enemy"); // Hedef düþman olmalý
    }

    // Askerlere özgü davranýþlarý burada ekleyebiliriz
}
