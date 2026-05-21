using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cinematic : MonoBehaviour
{
    [SerializeField] List<Image> comicImages = new List<Image>();
    [SerializeField] float duracionDeLasImagenes = 5f;
    Coroutine coroutineCinematic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
        foreach (Image comicImage in comicImages)
        {
            Color colorComicImage = comicImage.color;
            colorComicImage.a = 0f;
            comicImage.color = colorComicImage;
        }
        StartCoroutine(StartComic(0, 1, duracionDeLasImagenes));
    }
    IEnumerator StartComic(float startValue, float endValue, float duration)
    {
        foreach (Image image in comicImages)
        {
            float time = 0.0f;
            Color actualImageComic = image.color;
            while (time < duration)
            {
                actualImageComic.a = Mathf.Lerp(startValue, endValue, time / duration);
                image.color = actualImageComic;
                time += Time.deltaTime;
                yield return null;
            }
            actualImageComic.a = endValue;
            image.color = actualImageComic;
            yield return new WaitForSeconds(2);
        }
    }
    public void SaltarCinematica()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
        UIMainMenu.instance.gameObject.SetActive(true);
    }
}
