using UnityEngine;

public class Enemymanger : MonoBehaviour
{
    public ZombieEnemy enemyPrefab;
    public float spawnInterval = 1f;
    public float timer = 0;
    public int enemyCount = 0;
    public int maxEnemyCount = 5;

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("OnTriggerEnter: " + other.gameObject.name);
            timer += Time.deltaTime;
            if (timer >= spawnInterval && enemyCount < maxEnemyCount)
            {
                EnemyPool.Instance.GetObj(enemyPrefab);
                timer = 0;
                enemyCount++;
            }
        }
    }
}
