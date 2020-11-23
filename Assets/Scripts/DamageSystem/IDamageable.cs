using UnityEngine;

public interface IDamageable<input, output>
{
    Transform transform { get; }

    bool IsAlive { get; }

    output getHit(input inputDamage); //Se llama cuando esta entidad recibe daño.
    void onHit(output hitResult); //Se llama cuando la unidad hace daño a otra entidad.
}

public struct Damage
{
    public int damageAmmount;
}

public struct HitResult
{
    public bool killed;
}