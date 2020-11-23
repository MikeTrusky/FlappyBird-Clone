using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    [Header("Points images")]
    public List<Sprite> images;
    public Image imagePrefab;
    public Transform startImagePosition;
    [Header("Begin")]
    public List<Image> beginImages;
    private bool begun = false;
    [Header("Summary")]
    public List<GameObject> summaryImages;
    public AudioSource showSummarySounds;
    public List<Transform> ScorePositions;
    public List<Transform> BestScorePositions;
    public Transform RecordPosition;
    public Image newRecordPrefab;
    [Header("Medals")]
    public List<Sprite> medalsImages;
    public Image medalPrefab;
    public Transform medalPosition;

    private float bestScorePoints;
    private Image image_units, image_tens, image_hundreds, image_thousands;
    private GameManager gm;

    void Start()
    {
        foreach (GameObject image in summaryImages)
        {
            image.SetActive(false);
        }
        gm = FindObjectOfType<GameManager>();
        image_units = Instantiate(imagePrefab, startImagePosition.position, Quaternion.identity, this.gameObject.transform);
        bestScorePoints = PlayerPrefs.GetFloat("BestScore");
    }

    void Update()
    {
        if (gm.dead)
        {
            StartCoroutine(ShowSummary());
            gm.dead = false;
        }
        if (gm.gameBegun)
        {
            foreach (Image image in beginImages)
            {
                image.gameObject.SetActive(false);
            }
            begun = true;
        }
        List<int> results = CountPointsImagesNumbers(gm.points);
        if (image_units != null)
            image_units.sprite = images[results[0]];
        //Points in range(10, 99)
        if (results[1] > 0 && image_units != null && image_tens == null)
        {
            image_units.rectTransform.transform.position = startImagePosition.position + new Vector3(12 * 3.2f, 0, 0);
            image_tens = Instantiate(imagePrefab, startImagePosition.position + new Vector3(-12 * 3.2f, 0, 0), Quaternion.identity, this.gameObject.transform);
        }
        if (image_tens != null)
            image_tens.sprite = images[results[1]];
        //Points in range(100, 999)
        if (results[2] > 0 && image_units != null && image_tens != null && image_hundreds == null)
        {
            image_units.rectTransform.transform.position = startImagePosition.position + new Vector3(24 * 3.2f, 0, 0);
            image_tens.rectTransform.transform.position = startImagePosition.position;
            image_hundreds = Instantiate(imagePrefab, startImagePosition.position + new Vector3(-24 * 3.2f, 0, 0), Quaternion.identity, this.gameObject.transform);
        }
        if (image_hundreds != null)
            image_hundreds.sprite = images[results[2]];
        //Points in range(1000, 9999)
        if (results[3] > 0 && image_units != null && image_tens != null && image_hundreds != null && image_thousands == null)
        {
            image_units.rectTransform.transform.position = startImagePosition.position + new Vector3(36 * 3.2f, 0, 0);
            image_tens.rectTransform.transform.position = startImagePosition.position + new Vector3(12 * 3.2f, 0, 0);
            image_hundreds.rectTransform.transform.position = startImagePosition.position + new Vector3(-12 * 3.2f, 0, 0);
            image_thousands = Instantiate(imagePrefab, startImagePosition.position + new Vector3(-36* 3.2f, 0, 0), Quaternion.identity, this.gameObject.transform);
        }
        if (image_thousands != null)
            image_thousands.sprite = images[results[3]];
    }

    List<int> CountPointsImagesNumbers(int points)
    {
        //Kolekcja przechowująca 4 wartości: 4 cyfry: jedności, dziesiątki, setki, tysiące
        List<int> returnList;
        int units = 0, tens = 0, hundreds = 0, thousands = 0;
        thousands = Mathf.FloorToInt(points / 1000) % 10;
        hundreds = Mathf.FloorToInt(points / 100) % 10;
        tens = Mathf.FloorToInt(points / 10) % 10;
        units = points % 10;
        returnList = new List<int> { units, tens, hundreds, thousands };
        return returnList;
    }

    IEnumerator ShowSummary()
    {
        image_units.gameObject.SetActive(false);
        if (image_tens != null)
            image_tens.gameObject.SetActive(false);
        if (image_hundreds != null)
            image_hundreds.gameObject.SetActive(false);
        if (image_thousands != null)
            image_thousands.gameObject.SetActive(false);

        summaryImages[0].SetActive(true);
        showSummarySounds.Play();
        yield return new WaitForSeconds(showSummarySounds.clip.length - 1.0f);

        TransformPointsImageInSummary(image_units, ScorePositions[0]);
        if (image_tens != null)
            TransformPointsImageInSummary(image_tens, ScorePositions[1]);
        if (image_hundreds != null)
            TransformPointsImageInSummary(image_hundreds, ScorePositions[2]);
        if (image_thousands != null)
            TransformPointsImageInSummary(image_thousands, ScorePositions[3]);

        List<int> bestScore = CountPointsImagesNumbers((int)PlayerPrefs.GetFloat("BestScore"));
        Debug.Log("best score: " + bestScore[0]);
        Debug.Log("best score:" + bestScore[1]);

        image_units = Instantiate(imagePrefab, BestScorePositions[0].position, Quaternion.identity, summaryImages[1].transform);
        image_units.sprite = images[bestScore[0]];
        TransformPointsImageInSummary(image_units, BestScorePositions[0]);
        //Points in range(10, 99)
        if (bestScore[1] > 0)
        {
            image_tens = Instantiate(imagePrefab, BestScorePositions[1].position, Quaternion.identity, this.gameObject.transform);
            image_tens.sprite = images[bestScore[1]];
            TransformPointsImageInSummary(image_tens, BestScorePositions[1]);
            Debug.Log("tens");
        }
        //Points in range(100, 999)
        if (bestScore[2] > 0)
        {
            image_hundreds = Instantiate(imagePrefab, BestScorePositions[2].position, Quaternion.identity, this.gameObject.transform);
            image_hundreds.sprite = images[bestScore[2]];
            TransformPointsImageInSummary(image_hundreds, BestScorePositions[2]);
        }
        //Points in range(1000, 9999)
        if (bestScore[3] > 0)
        {
            image_thousands = Instantiate(imagePrefab, BestScorePositions[3].position, Quaternion.identity, this.gameObject.transform);
            image_thousands.sprite = images[bestScore[3]];
            TransformPointsImageInSummary(image_thousands, BestScorePositions[3]);
        }

        if (bestScorePoints < gm.points)
            Instantiate(newRecordPrefab, RecordPosition.position, Quaternion.identity, summaryImages[1].transform);

        if (gm.points >= 10 && gm.points < 20)
        {
            Image medal = Instantiate(medalPrefab, medalPosition.position, Quaternion.identity, summaryImages[1].transform);
            medal.sprite = medalsImages[0];
        }
        if (gm.points >= 20 && gm.points < 30)
        {
            Image medal = Instantiate(medalPrefab, medalPosition.position, Quaternion.identity, summaryImages[1].transform);
            medal.sprite = medalsImages[1];
        }
        if (gm.points >= 30 && gm.points < 40)
        {
            Image medal = Instantiate(medalPrefab, medalPosition.position, Quaternion.identity, summaryImages[1].transform);
            medal.sprite = medalsImages[2];
        }
        if (gm.points >= 40)
        {
            Image medal = Instantiate(medalPrefab, medalPosition.position, Quaternion.identity, summaryImages[1].transform);
            medal.sprite = medalsImages[3];
        }

        summaryImages[1].SetActive(true);
        showSummarySounds.Play();
        yield return new WaitForSeconds(showSummarySounds.clip.length - 1.0f);
        summaryImages[2].SetActive(true);
        summaryImages[3].SetActive(true);
    }

    void TransformPointsImageInSummary(Image image, Transform newTransform)
    {
        image.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
        image.gameObject.SetActive(true);
        Instantiate(image, newTransform.position, Quaternion.identity, summaryImages[1].transform);
        Destroy(image.gameObject);
    }
}
