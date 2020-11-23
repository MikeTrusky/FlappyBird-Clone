using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Sliders")]
    public SpriteRenderer[] sliders;
    public float sliderWidth;

    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        sliderWidth = sliders[0].gameObject.GetComponent<Renderer>().bounds.size.x * 2;
        sliders[1].transform.position = new Vector3(sliders[1].transform.position.x + (sliderWidth / 2), sliders[1].transform.position.y, 0);
    }

    void Update()
    {
        foreach (SpriteRenderer slideBar in sliders)
        {
            slideBar.transform.position += -transform.right * gm.slideSpeed * Time.deltaTime;
            if (slideBar.transform.position.x < -3.2)
                slideBar.transform.position = new Vector3(slideBar.transform.position.x + sliderWidth, slideBar.transform.position.y, 0);
        }
    }
}
