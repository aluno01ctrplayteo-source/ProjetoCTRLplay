using UnityEngine;

public class StressShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 500f; // tiros por segundo

    private float timer;
    private int shotsThisSecond;
    private float secondTimer;

    public static int shotsPerSecond;

    void Update()
    {
        timer += Time.deltaTime;
        secondTimer += Time.deltaTime;

        while (timer >= 1f / fireRate)
        {
            Shoot();
            timer -= 1f / fireRate;
        }

        if (secondTimer >= 1f)
        {
            shotsPerSecond = shotsThisSecond;
            shotsThisSecond = 0;
            secondTimer = 0f;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.AddComponent<Rigidbody>();
        Destroy(bullet, 2f);

        shotsThisSecond++;
    }
}
