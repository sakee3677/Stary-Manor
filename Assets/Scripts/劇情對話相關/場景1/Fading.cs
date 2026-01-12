using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Fading : MonoBehaviour
{
    public Material fadeMaterial;
    public Material playerMaterial;
    

    public float fadeDuration = 3f;
    public float fadeDuration1 = 1f;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer player;
    public GameObject realplayerSprite;
    public GameObject realplayerShadow;
    public SpriteRenderer player1;
    public GameObject playershowup;
    public GameObject cageplayerlight;
    public bool Next;
    //public bool Debug;

    void Start()
    {
        realplayerSprite.SetActive(!true);
        realplayerShadow.SetActive(!true);
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();


        fadeMaterial = spriteRenderer.material;
          playerMaterial = player.material;
    }

   
    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
       
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeMaterial.SetFloat("_FadeAmount", alpha);
            yield return null;
        }
        Color playerColor = player.color;
        playerColor.a = 225;
        player.color = playerColor;

        

        Color plater1Color = player1.color;
        plater1Color.a = 0;
        player1.color = plater1Color;
        elapsedTime = 0f;
        cageplayerlight.SetActive(!true);
        while (elapsedTime < fadeDuration1)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(19f, 0f, elapsedTime / fadeDuration1);
            playerMaterial.SetFloat("_Glow", alpha);
           
            yield return null;
        }

        Debug.Log("Glow งนฆจ");
       
        playershowup.SetActive(false);
        realplayerSprite.SetActive(true);
        realplayerShadow.SetActive(true);
        DialogueManager.Instance.RegisterEventCallback();
    }
   
}