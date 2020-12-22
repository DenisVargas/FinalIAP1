using IA.PathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] GameObject ZombieLeaderPrefab = null;
    [SerializeField] GameObject[] NormalZombiePrefabs = new GameObject[3];

    [Header("initialSpawn")]
    [SerializeField] bool spawnAtStart = false;
    [SerializeField] int minNormalZombies = 1;
    [SerializeField] int maxNormalZombies = 3;

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

    // Start is called before the first frame update
    void Start()
    {
        if (spawnAtStart && ZombieLeaderPrefab != null && NormalZombiePrefabs.Length > 0)
                SpawnWithSettings(maxNormalZombies, minNormalZombies);
    }

    public void SpawnWithSettings(int maxAmmount, int minAmmount)
    {
        int cantidad = Random.Range(minAmmount, maxAmmount + 1);

        List<Zombie> zombies = new List<Zombie>();
        for (int i = 0; i < cantidad; i++)
        {
            int prefab = Random.Range(0, NormalZombiePrefabs.Length);
            Zombie zombi = Instantiate(NormalZombiePrefabs[prefab]).GetComponent<Zombie>();
            zombies.Add(zombi);

            zombi.transform.position = transform.position + GetrandomPositionInCircle(MinSpawnRange, SpawnRange);
            LevelManager.ins.TrackZombie(zombi);
        }

        var liderDePelotón = Instantiate(ZombieLeaderPrefab).GetComponent<Zombie>();
        LevelManager.ins.TrackZombie(liderDePelotón);

        //Creo los grupos!
        foreach (var zombi in zombies)
        {
            List<Transform> group = new List<Transform>();
            group.Add(liderDePelotón.transform);
            for (int i = 0; i < zombies.Count; i++)
            {
                var zombieRef = zombies[i];
                if (zombieRef == zombi)
                    continue;
                else group.Add(zombieRef.transform);
            }

            zombi.SetGroup(group);
            zombi.isCaptain = false;
            zombi.SetLeader(liderDePelotón.transform);
            zombi.FeedState(CommonState.followLeader);
        }

        liderDePelotón.isCaptain = true;
        liderDePelotón.transform.position = transform.position;
        if (LevelManager.ins && LevelManager.ins.humansAlive())
            liderDePelotón.SetLookUpTargetLocation(LevelManager.ins.GetMiddlePointBetweenHumans());
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
