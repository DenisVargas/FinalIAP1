using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] HumanPrefabs = new GameObject[3];

    [Header("initialSpawn")]
    [SerializeField] bool spawnAtStart = false;
    [Range(1,20)]
    [SerializeField] int Ammount = 1;

    [SerializeField] float SpawnRange = 4f;
    [SerializeField] float MinSpawnRange = 0f;
    [SerializeField] Color debug_spawnMaxRangeColor = Color.yellow;
    [SerializeField] Color debug_spawnMinRangeColor = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = debug_spawnMaxRangeColor;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, SpawnRange);

        Gizmos.color = debug_spawnMinRangeColor;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, MinSpawnRange);
    }

    private void Start()
    {
        if (spawnAtStart)
            Spawn(Ammount);
    }

    public void Spawn(int Ammount)
    {
        for (int i = 0; i < Ammount; i++)
        {
            int prefabIndex = Random.Range(0, HumanPrefabs.Length);
            var human = Instantiate(HumanPrefabs[prefabIndex]).GetComponent<Human>();

            human.transform.position = transform.position + GetrandomPositionInCircle(MinSpawnRange, SpawnRange);
            //Acá tenemos acceso a cada humanito.
            LevelManager.ins.TrackHuman(human);
        }
    }

    public void SpawnRandomAmmount(int min, int max)
    {
        int _ammount = Random.Range(min, max + 1);
        Spawn(_ammount);
    }

    private Vector3 GetrandomPositionInCircle(float minRadius, float maxRadius)
    {
        //Calculo posicion.
        float distanceFactor = Random.Range(0f, 1f);
        float factorToMinDistance = (maxRadius - minRadius) / maxRadius;
        float angle = Mathf.Deg2Rad * (Random.Range(0f, 360f));
        Vector2 circlePoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 dirToCircle = (circlePoint - Vector2.zero);
        Vector2 pointToMinFactor = Vector2.Lerp(Vector2.zero, circlePoint, factorToMinDistance);
        Vector2 randomPointInCircle = Vector2.Lerp(pointToMinFactor, circlePoint, distanceFactor);

        Vector3 dir = new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y);
        dir *= maxRadius;
        return dir;
    }
}
