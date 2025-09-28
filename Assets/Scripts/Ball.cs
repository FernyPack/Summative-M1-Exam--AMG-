using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallController : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody rb;
    public Text scoreText;
    public Text livesText;
    public int score = 0;
    public int lives = 3;
    private bool gameOver = false;
    private Renderer rend;
    public float maxScale = 2f;
    private Vector3 startPos;
    private float startSpeed;
    private Vector3 startScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        startPos = transform.position;
        startSpeed = speed;
        startScale = transform.localScale;
        rb.linearVelocity = new Vector3(Random.Range(-1f,1f),0,1).normalized * speed;
        UpdateUI();
    }

    void FixedUpdate()
    {
        if(gameOver) return;
        if(rb.linearVelocity.magnitude < 2f)
            rb.linearVelocity = rb.linearVelocity.normalized * 2f;

        Vector3 pos = transform.position;
        float halfSize = transform.localScale.x / 2f;
        pos.x = Mathf.Clamp(pos.x, -8f + halfSize, 8f - halfSize);
        pos.z = Mathf.Clamp(pos.z, -10f + halfSize, 15f - halfSize);
        transform.position = pos;
    }

    void OnCollisionEnter(Collision col)
    {
        if(gameOver) return;

        if(col.gameObject.tag == "Paddle")
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity, col.contacts[0].normal).normalized;
            reflectDir.x += Random.Range(-0.1f, 0.1f);
            rb.linearVelocity = reflectDir.normalized * speed;
            score++;
            speed += 0.5f;
            StartCoroutine(GrowBallSafe(1.05f));
            rend.material.color = new Color(Random.value, Random.value, Random.value);
            UpdateUI();
        }
        else if(col.gameObject.tag == "Wall")
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity, col.contacts[0].normal).normalized;
            rb.linearVelocity = reflectDir.normalized * speed;
        }
        else if(col.gameObject.tag == "Floor")
        {
            lives--;
            if(lives <= 0)
            {
                lives = 0;
                rb.linearVelocity = Vector3.zero;
                gameOver = true;
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
            else
            {
                ResetBall();
            }
            UpdateUI();
        }
    }

    void ResetBall()
    {
        transform.position = startPos;
        transform.localScale = startScale;
        speed = startSpeed;
        rb.linearVelocity = new Vector3(Random.Range(-1f,1f),0,1).normalized * speed;
        rend.material.color = Color.white;
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;
    }

    IEnumerator GrowBallSafe(float factor)
    {
        Vector3 targetScale = transform.localScale * factor;
        if(targetScale.x > maxScale) targetScale = Vector3.one * maxScale;
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;
        float duration = 0.2f;
        while(elapsed < duration)
        {
            Vector3 newScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
            float halfSize = newScale.x / 2f;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -8f + halfSize, 8f - halfSize);
            pos.z = Mathf.Clamp(pos.z, -10f + halfSize, 15f - halfSize);
            transform.position = pos;
            transform.localScale = newScale;
            elapsed += Time.fixedDeltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
