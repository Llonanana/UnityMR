    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageOption : MonoBehaviour
{
    public LunarcomSpeechRecognizer speechRecognizer;
    public APIRequestController apiRequestController;
    public NPCRequestManager npcRequestManager;
    public SpeechToTextManager speechToTextManager;

    public string Language1 = "en-US";
    public string Language1Alt = "en";
    public string Language2 = "zh-TW";
    public string Language2Alt = "zh";
    public string Language3 = "ja-JP";
    public string Language3Alt = "ja";
    public string Language4 = "de-DE";
    public string Language4Alt = "de";
    // Start is called before the first frame update
    void Start()
    {
        if (speechRecognizer == null)
        {
            Debug.LogError("Speech Recognizer reference is not set!");
        }
    }

    //English
    public void English()
    {
        if (speechRecognizer != null)
        {
            speechRecognizer.fromLanguage = Language1;
            apiRequestController.language = Language1;
            npcRequestManager.language = Language1Alt;
            speechToTextManager.fromLanguage = Language1;
            Debug.Log("Language changed to: " + Language1);
        }
    }

    public void Chinese_TW()
    {
        if (speechRecognizer != null)
        {
            speechRecognizer.fromLanguage = Language2;
            apiRequestController.language = Language2;
            npcRequestManager.language = Language2Alt;
            speechToTextManager.fromLanguage = Language2;
            Debug.Log("Language changed to: " + Language2);
        }
    }

    public void Japanese()
    {
    if (speechRecognizer != null)
    {
            speechRecognizer.fromLanguage = Language3;
            apiRequestController.language = Language3;
            npcRequestManager.language = Language3Alt;
            speechToTextManager.fromLanguage = Language3;
            Debug.Log("Language changed to: " + Language3);
        }
    }

    public void German()
    {
        if (speechRecognizer != null)
        {
            speechRecognizer.fromLanguage = Language4;
            apiRequestController.language = Language4;
            npcRequestManager.language = Language4Alt;
            speechToTextManager.fromLanguage = Language4;
            Debug.Log("Language changed to: " + Language4);
        }
    }

}
