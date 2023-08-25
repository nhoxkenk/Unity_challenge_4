using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBody;
    private GameObject focalPoint;
    public float speed = 5.0f;
    public bool isPowerUp = false;
    public float powerUpStrength = 15f;
    public GameObject powerUpIndicator;

    public PowerUpType currentPowerUpType = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountDown;

    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    public bool smashing = false;

    float floorY;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        rigidBody.AddForce(focalPoint.transform.forward * verticalInput * speed);
        powerUpIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerUpType == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            launchRockets();
        }

        if (currentPowerUpType == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space))
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            isPowerUp = true;
            currentPowerUpType = other.gameObject.GetComponent<PowerUp>().type;
            Debug.Log(currentPowerUpType.ToString());
            powerUpIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);

            //nếu như đang trong trạng thái power up mà va chạm với 1 power up khác.
            if (powerupCountDown != null)
            {
                StopCoroutine(powerupCountDown);
            }
            powerupCountDown =  StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        isPowerUp = false;
        currentPowerUpType = PowerUpType.None;
        powerUpIndicator.gameObject.SetActive(false);
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        floorY = transform.position.y;
        float jumpTime = Time.time + hangTime;

        while(Time.time < jumpTime)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, smashSpeed);
            yield return null;
        }

        while(transform.position.y > floorY)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        foreach (var enemy in enemies)
        {
            //float distance = (transform.position - enemy.transform.position).magnitude;
            if(enemy != null)
            {
                enemy.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        
        smashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if(collision.gameObject.CompareTag("Enemy") && currentPowerUpType == PowerUpType.Pushback)
        {
            Rigidbody enemyRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRigidBody.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + "with power up set to " + currentPowerUpType.ToString());
        }
        
    }

    void launchRockets()
    {
        foreach(var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
