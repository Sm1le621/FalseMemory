using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Чорний екран для переходу між сценами.
// При перезапуску гри / виході в меню переживає завантаження сцени ТІЛЬКИ
// сам чорний екран - ніякий ігровий стан не переноситься.
public sealed class BattleScreenFade : MonoBehaviour
{
    // Чорна картинка на весь екран
    public Image panel;

    // Завантажити сцену і потім плавно прибрати чорний екран
    public void LoadAndReveal(string scene, float duration)
    {
        // Відчіпляємо від батька і кажемо Unity не видаляти цей об'єкт при зміні сцени
        transform.SetParent(null, false);
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Load(scene, duration));
    }

    private IEnumerator Load(string scene, float duration)
    {
        AsyncOperation operation = null;
        try
        {
            operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, this);
        }

        // Чекаємо, поки сцена завантажиться
        if (operation != null)
        {
            yield return operation;
        }

        // Плавно робимо екран прозорим
        yield return Fade(panel, 0f, duration);

        // Більше не потрібен
        Destroy(gameObject);
    }

    // Плавно змінює прозорість картинки до alpha за duration секунд
    public static IEnumerator Fade(Image image, float alpha, float duration)
    {
        if (image == null)
        {
            yield break;
        }

        alpha = Mathf.Clamp01(alpha);
        duration = Mathf.Clamp(duration, 0f, 2f);

        Color color = image.color;
        float start = color.a;

        // unscaledDeltaTime - працює навіть якщо Time.timeScale = 0
        for (float t = 0; t < duration; t = t + Time.unscaledDeltaTime)
        {
            // Картинку могли видалити поки ми чекали
            if (image == null)
            {
                yield break;
            }

            // SmoothStep робить зміну плавною на початку і в кінці
            float progress = Mathf.SmoothStep(0f, 1f, t / duration);
            color.a = Mathf.Lerp(start, alpha, progress);
            image.color = color;
            yield return null;
        }

        // В кінці точно ставимо потрібну прозорість
        if (image != null)
        {
            color.a = alpha;
            image.color = color;
        }
    }
}
