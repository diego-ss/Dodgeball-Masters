using UnityEngine;

public abstract class PowerUpEffect : ScriptableObject
{
    public abstract void Aplicar(GameObject target);
    public abstract void Destruir();
}
