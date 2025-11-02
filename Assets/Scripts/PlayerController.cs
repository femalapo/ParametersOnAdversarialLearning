using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerController : MonoBehaviour
{
    public GameObject opponent;

    public GameObject arena;
    private float[] bounds;

    public GameObject bullet;
    public float bulletSpawnDistance = 1f;
    public float bulletSpeed = 15f; // Should be faster than moveSpeed to avoid self-collision
    public bool bulletActive = false;
    public GameObject currBullet = null;

    public float moveSpeed = 10.0f;
    public float rotationSpeed = 100.0f;

    private void Start()
    {
        bounds = arena.GetComponent<Arena>().getBounds();

        //print(bounds[0] + " " + bounds[1] + " " + bounds[2] + " " + bounds[3]);
    }

    public void Move(Vector2 moveValue)
    {
        transform.Translate(new Vector3(moveValue[0], 0f, moveValue[1]) * moveSpeed * Time.deltaTime, Space.World);

        // Keep within bounds
        float zVal = Mathf.Min(Mathf.Max(transform.position.z, bounds[0]), bounds[1]);
        float xVal = Mathf.Min(Mathf.Max(transform.position.x, bounds[2]), bounds[3]);
        transform.position = new Vector3(xVal, transform.position.y, zVal);
    }

    public void Rotate(float rotateValue)
    {
        transform.Rotate(0f, rotateValue * rotationSpeed * Time.deltaTime, 0f);
    }

    public void Shoot()
    {
        if(!bulletActive)
        {
            GameObject b = Instantiate(bullet, transform.position + (transform.forward * bulletSpawnDistance), transform.rotation);
            Bullet bScript = b.GetComponent<Bullet>();
            bScript.owner = gameObject;
            bScript.speed = bulletSpeed;
            bScript.opponent = opponent;

            bulletActive = true;
            currBullet = b;
        }
        
    }
}
