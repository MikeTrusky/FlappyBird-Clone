using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Speed")]
    public float speed = 8.0f;

    [Header("Sound")]
    public AudioClip[] sounds;

    [Header("BirdSprites")]
    public List<Sprite> yellowBird;
    public List<Sprite> blueBird;
    public List<Sprite> redBird;

    private AudioSource birdAudioSource;
    private List<List<Sprite>> birds = new List<List<Sprite>>();
    private List<Sprite> bird = new List<Sprite>();
    private List<float> moveRanges = new List<float>() { 0.30f, 0.10f };
    private bool jump = false;
    private bool move = true;
    private GameManager gm;
    private Vector2 movement;
    private float flyingSpeed = 0.1f;
    private float desiredRot;
    private float rotSpeed = 30.0f;
    private float damping = 10.0f;
    private bool goUp = true;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            birds.Clear();
            bird.Clear();
            birds.Add(yellowBird);
            birds.Add(blueBird);
            birds.Add(redBird);
            bird = birds[Random.Range(0, 2)];
        }
        gm = FindObjectOfType<GameManager>();
        rb = this.GetComponent<Rigidbody2D>();
        birdAudioSource = this.GetComponent<AudioSource>();
        birdAudioSource.clip = sounds[0];
        rb.gravityScale = 0;
        StartCoroutine(ChangeSprites());
    }

    private void OnEnable()
    {
        desiredRot = transform.eulerAngles.z;
    }

    void Update()
    {
        movement = new Vector2(0, -speed * Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && move && SceneManager.GetActiveScene().name != "Menu")
        {
            gm.gameBegun = true;
            flyingSpeed = 0.04f;
            jump = true;
        }
        desiredRot += rb.velocity.y * rotSpeed * Time.deltaTime;

        desiredRot = desiredRot > -90.0f ? desiredRot : -90.0f;
        desiredRot = desiredRot < 45.0f ? desiredRot : 45.0f;
        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);
    }

    void FixedUpdate()
    {
        if (!gm.gameBegun)
            MoveOnStart(moveRanges);
        else
            moveCharacter(movement, 1, ForceMode2D.Force);
        if (jump)
        {
            rb.gravityScale = 1;
            Jump();
            jump = false;
        }
    }

    void moveCharacter(Vector2 direction, float divideAmount, ForceMode2D fm)
    {
        rb.AddForce(direction * speed / divideAmount, fm);
    }

    void Jump()
    {
        rb.velocity = Vector2.zero;
        moveCharacter(Vector2.up, 0.8f, ForceMode2D.Impulse);
        birdAudioSource.Play();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Death();
    }

    IEnumerator PlayDeathSound()
    {
        yield return new WaitForSeconds(sounds[1].length);
        birdAudioSource.clip = sounds[2];
        birdAudioSource.Play();
    }

    void Death()
    {
        gm.slideSpeed = 0.0f;
        if (PlayerPrefs.GetFloat("BestScore") < gm.points)
            PlayerPrefs.SetFloat("BestScore", gm.points);
        gm.dead = true;
        move = false;
        this.GetComponent<CircleCollider2D>().enabled = false;
        moveCharacter(Vector2.up, 0.6f, ForceMode2D.Impulse);
        birdAudioSource.clip = sounds[1];
        birdAudioSource.Play();
        StartCoroutine(PlayDeathSound());
    }

    IEnumerator ChangeSprites()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
            bird = yellowBird;

        for (int i = 0; i < bird.Count; ++i)
        {
            this.GetComponent<SpriteRenderer>().sprite = bird[i];
            yield return new WaitForSeconds(flyingSpeed);
        }
        for (int i = bird.Count - 1; i >= 0; --i)
        {
            this.GetComponent<SpriteRenderer>().sprite = bird[i];
            yield return new WaitForSeconds(flyingSpeed);
        }
        StartCoroutine(ChangeSprites());
    }

    void MoveOnStart(List<float> ranges)
    {
        if (this.transform.position.y < ranges[0] && goUp)
            this.transform.position += transform.up * 0.7f * Time.deltaTime;
        if (this.transform.position.y >= ranges[0])
            goUp = false;
        if (this.transform.position.y > ranges[1] && !goUp)
            this.transform.position += -transform.up * 0.7f * Time.deltaTime;
        if (this.transform.position.y <= ranges[1])
            goUp = true;
    }
}