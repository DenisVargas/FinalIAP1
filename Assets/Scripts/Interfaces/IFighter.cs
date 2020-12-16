using UnityEngine;

public interface IFighter<TDataInput, TDataOutput>
{
    bool IsAlive { get; }
    Transform transform {get;}
    GameObject gameObject { get; }
    bool enabled { get; }

    TDataOutput Hit(TDataInput hitData);
    TDataInput GetCombatStats();
    void OnHiConnected(TDataOutput hitResult);
}

public struct HitData
{
    public int Damage;
}

public struct HitResult
{
    public bool targetEliminated;
    public bool Conected;
}
