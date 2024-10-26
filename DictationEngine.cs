using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;

public class DictationEngine : MonoBehaviour
{
    protected DictationRecognizer dictationRecognizer;

    public AudioClip[] greeting;
    public AudioClip[] rejectwork,reject, confusion, motivation, stare;
    public AudioClip name, love, parent, tv, music;
    public bool coolDown, talking, typing, walking;
    public Animator anim;
    public float idleTime;
    public GameObject tvScreen;
    public Transform typePos, mainPos;

    void Start()
    {
        StartDictationEngine();
    }

    private void DictationRecognizer_OnDictationHypothesis(string text)
    {
        Debug.Log("Dictation hypothesis: " + text);
    }

    void Update()
    {
        idleTime += Time.deltaTime;
        if (idleTime > 10)
        {
            if (!coolDown)
            {
                idleTime = 0f;
                int idlejob = Random.Range(0, 4);
                switch (idlejob)
                {
                    case 0:
                        int audiono = Random.Range(0, 3);
                        GetComponent<AudioSource>().PlayOneShot(stare[audiono]);
                        StartCoroutine(CoolDownTime(14f));
                        break;
                    case 1:
                        anim.SetTrigger("tv");
                        tvScreen.SetActive(true);
                        StartCoroutine(TVScreen());
                        StartCoroutine(CoolDownTime(5f));
                        break;
                    case 2:
                        anim.SetTrigger("dance");
                        GetComponent<AudioSource>().PlayOneShot(music);
                        StartCoroutine(CoolDownTime(5f));
                        break;
                    case 3:
                        anim.SetTrigger("bored");
                        StartCoroutine(CoolDownTime(2.44f));
                        break;
                    case 4:
                        anim.SetTrigger("pushup");
                        StartCoroutine(CoolDownTime(5f));
                        break;
                }
            }
            
        }
        if (typing)
        {
            if (transform.position != typePos.position)
            {
                // Use Lerp for smooth movement
                transform.position = Vector3.Lerp(transform.position, typePos.position, Time.deltaTime * 5f);
            }
            else
            {
                StartCoroutine(Typing()); // Changed to TypingCoroutine for clarity
            }
        }

        if (walking)
        {
            if (transform.position != mainPos.position)
            {
                // Use Lerp for smooth movement
                transform.position = Vector3.Lerp(transform.position, mainPos.position, Time.deltaTime * 5f);
            }
        }

    }

    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
    {
        switch (completionCause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
                CloseDictationEngine();
                StartDictationEngine();
                break;
            case DictationCompletionCause.UnknownError:
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                CloseDictationEngine();
                break;
        }
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation result: " + text);
        CheckText(text); // Call CheckText here to process the final result text
    }

    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
    }

    private void OnApplicationQuit()
    {
        CloseDictationEngine();
    }

    private void StartDictationEngine()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
        dictationRecognizer.Start();
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
        }
    }

    void CheckText(string text)
    {
        string lowerText = text.ToLower();

        if (!coolDown)
        {
            // Check for sad phrases
            if (lowerText.Contains("i'm sad") || lowerText.Contains("i am sad") || lowerText.Contains("bad day") ||
                lowerText.Contains("feeling down") || lowerText.Contains("feeling blue") || lowerText.Contains("not happy"))
            {
                idleTime = 0f;
                int audiono = Random.Range(0, 3);
                GetComponent<AudioSource>().PlayOneShot(motivation[audiono]);
                anim.SetTrigger("argue2");
                StartCoroutine(CoolDownTime(14f));
            }

            // Check for name inquiries
            else if (lowerText.Contains("your name") || lowerText.Contains("what's your name") || lowerText.Contains("who are you"))
            {
                idleTime = 0f;
                GetComponent<AudioSource>().PlayOneShot(name);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            // Check for parents inquiries
            else if (lowerText.Contains("your parents") || lowerText.Contains("your parent") || lowerText.Contains("who made you") || lowerText.Contains("who created you") || lowerText.Contains("your creator") || lowerText.Contains("your creators"))
            {
                idleTime = 0f;
                GetComponent<AudioSource>().PlayOneShot(parent);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            // Check for requests involving "could you" or "can you"
            else if (lowerText.Contains("could you do") || lowerText.Contains("can you do") || lowerText.Contains("would you do") || lowerText.Contains("do something"))
            {
                idleTime = 0f;
                int audiono = Random.Range(0, 4);
                GetComponent<AudioSource>().PlayOneShot(rejectwork[audiono]);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            // Check for general requests involving "could you"
            else if (lowerText.Contains("could you") || lowerText.Contains("can you") || lowerText.Contains("would you") || lowerText.Contains("dance") || lowerText.Contains("sing"))
            {
                idleTime = 0f;
                int audiono = Random.Range(0, 2);
                GetComponent<AudioSource>().PlayOneShot(reject[audiono]);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            // Check for greetings
            else if (lowerText.Contains("hey delay") || lowerText.Contains("hey dile") || lowerText.Contains("hey diley") || lowerText.Contains("hey dilay") ||
                     lowerText.Contains("hello delay") || lowerText.Contains("hi delay") || lowerText.Contains("hey there delay") || lowerText.Contains("hi") || lowerText.Contains("hello"))
            {
                idleTime = 0f;
                Debug.Log("Hello");
                int audiono = Random.Range(0, 4);
                GetComponent<AudioSource>().PlayOneShot(greeting[audiono]);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            else if (lowerText.Contains("love you"))
            {
                idleTime = 0f;
                GetComponent<AudioSource>().PlayOneShot(love);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            else if (lowerText.Contains("tv"))
            {
                idleTime = 0f;
                GetComponent<AudioSource>().PlayOneShot(tv);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }

            // Handle confusion response
            else
            {
                idleTime = 0f;
                int audiono = Random.Range(0, 2);
                GetComponent<AudioSource>().PlayOneShot(confusion[audiono]);
                anim.SetTrigger("talk");
                StartCoroutine(CoolDownTime(3f));
            }
        }
    }




    IEnumerator CoolDownTime(float sec)
    {
        coolDown = true;
        yield return new WaitForSeconds(sec);
        coolDown = false;
    }

    IEnumerator TVScreen()
    {
        yield return new WaitForSeconds(4f);
        tvScreen.gameObject.SetActive(false);
    }

    IEnumerator Typing()
    {
        typing = false;
        anim.SetTrigger("typing");
        StartCoroutine(CoolDownTime(6f));
        yield return new WaitForSeconds(5f);
        walking = true;
    }

}
