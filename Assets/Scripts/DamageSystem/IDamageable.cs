using UnityEngine;

/// <summary>
/// Interfaz que determina toda entidad que puede recibir daño.
/// </summary>
/// <typeparam name="input">Tipo de dato que resume las variables de daño</typeparam>
/// <typeparam name="output">Tipo de dato que resume el resultado del daño aplicado sobre dicho objeto</typeparam>
public interface IDamageable<input, output>
{
    Transform transform { get; }

    bool IsAlive { get; }

    output getHit(input inputDamage); //Se llama cuando esta entidad recibe daño.
}
/// <summary>
/// Interfaz que determina toda entidad que puede generar daño y que sea parte de un sistema cerrado (Agresor vs Damageable)
/// </summary>
/// <typeparam name="input">Tipo de dato que resume las variables de daño</typeparam>
/// <typeparam name="output">Tipo de dato que resume el resultado del daño aplicado sobre dicho objeto</typeparam>
public interface IAgressor<input, output>
{
    Transform transform { get; }

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