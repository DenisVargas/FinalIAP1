using UnityEngine;

public static class VectorExtentions
{
    /// <summary>
    /// Devuelve un Vector3 cuyos 3 parámetros son enteros aleatorios, entre un mínimo [inclusivo] y un máximo [exclusivo].
    /// </summary>
    /// <param name="Vector">Vector original</param>
    /// <param name="min">Rango mínimo [inclusivo]</param>
    /// <param name="max">Rango máximo [exclusivo]</param>
    /// <returns></returns>
    public static Vector3 RandomVector3(this Vector3 Vector, int min = 0, int max = 2)
    {
        Vector = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        return Vector;
    }
    /// <summary>
    /// Devuelve un Vector3 cuyos 3 parámetros flotantes son aleatorios, entre un mínimo [inclusivo] y un máximo [inclusivo].
    /// </summary>
    /// <param name="Vector">Vector original</param>
    /// <param name="min">Rango mínimo [inclusivo]</param>
    /// <param name="max">Rango máximo [inclusivo]</param>
    /// <returns></returns>
    public static Vector3 RandomVector3(this Vector3 Vector, float min = 0f, float max = 1f)
    {
        Vector = new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        return Vector;
    }
    public static Vector3 DirTo(this Vector3 origin, Vector3 targetPosition)
    {
        return (targetPosition - origin).normalized;
    }
    /// <summary>
    /// Mutiplica cada paramámetro por un escalar y devuelve el resultado.
    /// </summary>
    /// <param name="original">Vector original</param>
    /// <param name="scale">Escalar flotante</param>
    /// <returns></returns>
    public static Vector3 ScaleTo(this Vector3 original, float scale)
    {
        return new Vector3(original.x * scale, original.y * scale, original.z * scale);
    }
    /// <summary>
    /// Multiplica cada compomente por el componente equivalente en el Vector escala dado como parámetro.
    /// </summary>
    /// <param name="original">Vector original</param>
    /// <param name="Scale">Vector de escala</param>
    /// <returns></returns>
    public static Vector3 ScaleTo(this Vector3 original, Vector3 Scale)
    {
        return new Vector3(original.x * Scale.x, original.y * Scale.y, original.z * Scale.z);
    }
    /// <summary>
    /// Sobrescribe el valor del componente Y.
    /// </summary>
    /// <param name="vector">Vector original.</param>
    /// <param name="value">Valor final del componente Y.</param>
    /// <returns>El vector original con su componente Y modificado.</returns>
    public static Vector3 YComponent(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, value, vector.z);
    }
    /// <summary>
    /// Sobrescribe el valor del componente X.
    /// </summary>
    /// <param name="vector">Vector original</param>
    /// <param name="value">Valor final del componente X.</param>
    /// <returns>El vector original con su componente X modificado.</returns>
    public static Vector3 XComponent(this Vector3 vector, float value)
    {
        return new Vector3(value, vector.y, vector.z);
    }
    /// <summary>
    /// Sobrescribe el valor del componente Z.
    /// </summary>
    /// <param name="vector">Vector original.</param>
    /// <param name="value">Valor final del componente Z.</param>
    /// <returns>El vector original con su componente z modificado.</returns>
    public static Vector3 ZComponent(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y, value);
    }
}
