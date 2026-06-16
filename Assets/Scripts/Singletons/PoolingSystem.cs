using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    public static PoolingSystem Instance;

    Dictionary<string, Queue<Bullet>> stored;
    public List<Bullet> activeBullets = new List<Bullet>(); // ✅ Lista de projéteis ativos
    GameObject bulletHolder;

    void Awake()
    {
        Instance = this;
        stored = new Dictionary<string, Queue<Bullet>>();
        bulletHolder = GameObject.Find("BulletHolder");
    }

    public Bullet GetBullet(Bullet bulletPrefab)
    {
        string bulletKey = bulletPrefab.name;
        Bullet toReturn;

        if (!stored.ContainsKey(bulletKey))
        {
            toReturn = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletHolder.transform);
            Queue<Bullet> newQueue = new Queue<Bullet>();
            stored.Add(bulletKey, newQueue);
        }
        else
        {
            Queue<Bullet> currentQueue = stored[bulletKey];
            if (currentQueue.Count == 0)
            {
                toReturn = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletHolder.transform);
            }
            else
            {
                toReturn = currentQueue.Dequeue();
            }
        }

        toReturn.name = bulletKey;
        toReturn.gameObject.SetActive(true);
        activeBullets.Add(toReturn); // ✅ registra como ativo
        return toReturn;
    }

    public void StoreBullet(Bullet toStore)
    {
        stored[toStore.name].Enqueue(toStore);
        activeBullets.Remove(toStore); // ✅ remove da lista de ativos
        toStore.gameObject.SetActive(false);
    }

    public Dictionary<string, Queue<Bullet>> Stored => stored;
}
