using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private TextMeshProUGUI tipText;
    [TextArea] public string[] tips;

    private void Start()
    {
        tipText.text = tips[Random.Range(0, tips.Length)];
        StartCoroutine(LoadAsync(SceneLoader.SceneToLoad));
    }

    private void Update()
    {

        Color darkGreen = new Color32(98, 140, 59, 255);     // #628C3B
        Color lightGreen = new Color32(165, 201, 74, 255);   // #A5C94A


        progressBar.color = Color.Lerp(darkGreen, lightGreen, Mathf.PingPong(Time.time * 0.5f, 1));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.fillAmount = progress;
            percentText.text = Mathf.RoundToInt(progress * 100) + "%";

            if (op.progress >= 0.9f)
            {
                progressBar.fillAmount = 1f;
                percentText.text = "100%";
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
