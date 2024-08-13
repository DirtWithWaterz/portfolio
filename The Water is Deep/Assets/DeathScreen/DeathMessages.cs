using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathMessages : MonoBehaviour {
    [SerializeField] float timeBtwnChars;
    [SerializeField] float timeBtwnWords;

    int i = 0;

    public string[] stringArray;

    public bool playerDied = false;
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text message;
    public List<string> deathMessages = new List<string>();
    [SerializeField]AudioSource as1;
    [SerializeField]AudioSource as2;

    void Start() {
        deathMessages.Add(new string("THE WATER IS DEEP"));
        deathMessages.Add(new string("DEEP DOWN DEEP DARK"));
        deathMessages.Add(new string("THE ABYSS IS CALLING"));
        deathMessages.Add(new string("GET OUT OF MY HEAD"));
        deathMessages.Add(new string("YOU NEED TO GO DEEPER"));
        deathMessages.Add(new string("SPEED UP SLOW POKE"));
        deathMessages.Add(new string("STOP HIDING"));
        deathMessages.Add(new string("GET THEM BEFORE HE DOES"));
        deathMessages.Add(new string("WE'RE ALL MONSTERS"));
        deathMessages.Add(new string("HE'S STARVING"));
        deathMessages.Add(new string("WHO'S THE REAL MONSTER"));
        deathMessages.Add(new string("MEMENTO MORI"));
        deathMessages.Add(new string("NOWHERE IS SAFE"));
        deathMessages.Add(new string("YOU CAN'T RUN FROM ME"));
        canvas.enabled = false;
    }

    void Update() {
        if(playerDied){
            canvas.enabled = true;
            as1.Play();
            as2.Play();
            int ranDeathM = Random.Range(0, deathMessages.Count -1);
            message.text = deathMessages[ranDeathM];
            StartCoroutine(TypewriteText());
            playerDied = false;
        }
    }

    void EndCheck(){
        if (i <= stringArray.Length - 1)
        {
            message.text = stringArray[i];
            StartCoroutine(TypewriteText());
        }
    }

    IEnumerator TypewriteText(){
        message.ForceMeshUpdate();
        int totalVisibleCharacters = message.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            message.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                Invoke("EndCheck", timeBtwnWords);
                break;
            }

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
        yield return new WaitForSeconds(23);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
