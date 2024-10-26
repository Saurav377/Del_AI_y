using UnityEngine;
using UnityEngine.UI; // For UI elements
using TMPro; // For TextMesh Pro elements
using UnityEngine.SceneManagement; // For scene management

public class TextChanger : MonoBehaviour
{
    public TMP_Text displayText; // Reference to the TextMesh Pro text element
    public Button changeTextButton; // Reference to the button
    public string[] texts = { }; // Array of texts
    private int currentIndex = 0; // Index to track the current text

    void Start()
    {
        // Set initial text
        if (texts.Length > 0)
        {
            displayText.text = texts[currentIndex];

            // Add listener to button
            changeTextButton.onClick.AddListener(ChangeText);
        }
    }

    void ChangeText()
    {
        // Increment index and check if we reached the end of the array
        currentIndex++;

        if (currentIndex < texts.Length)
        {
            // Update the displayed text
            displayText.text = texts[currentIndex];
        }
        else
        {
            // Load the main scene when the last text is displayed
            LoadMainScene();
        }
    }

    void LoadMainScene()
    {
        // Replace "MainScene" with the name of your main scene
        SceneManager.LoadScene("main");
    }
}
