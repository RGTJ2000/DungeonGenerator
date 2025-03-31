using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject textObject;
    public GameObject imageObject;
    public GameObject dungeongenerator_obj;

    private DungeonGenerator _dungeonGenerator;
    private TextMeshProUGUI _generationText;
    private float _generationStartTime;
    private bool _wasGeneratingLastFrame;

    void Start()
    {
        _dungeonGenerator = dungeongenerator_obj.GetComponent<DungeonGenerator>();

        if (textObject != null)
        {
            _generationText = textObject.GetComponent<TextMeshProUGUI>();
            if (_generationText != null)
            {
                _generationText.text = "";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on textObject!");
            }
        }
    }

    void Update()
    {

        if (_dungeonGenerator.currentlyGenerating)
        {
            if (!_wasGeneratingLastFrame)
            {
                _generationStartTime = Time.time;
            }

            if (textObject != null) textObject.SetActive(true);
            if (imageObject != null) imageObject.SetActive(true);

            if (_generationText != null)
            {
                float elapsedTime = Time.time - _generationStartTime;
                _generationText.text = $"Generating. Please hold.\nElapsed time: {elapsedTime:F2}s";
            }
        }
        else
        {
            if (textObject != null) textObject.SetActive(false);
            if (imageObject != null) imageObject.SetActive(false);
            if (_generationText != null) _generationText.text = "";
        }

        _wasGeneratingLastFrame = _dungeonGenerator.currentlyGenerating;
    }
}