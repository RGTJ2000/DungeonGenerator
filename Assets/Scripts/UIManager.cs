using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class UIManager : MonoBehaviour
{
    public GameObject textObject;
    public GameObject imageObject;
    public GameObject dungeongenerator_obj;

    private DungeonGenerator _dungeonGenerator;
    private TextMeshProUGUI _generationText;
    private float _generationStartTime;
    private bool _wasGeneratingLastFrame;

    public TMP_InputField mainPathCount_inputField;
    public Button nodeUpButton;
    public Button nodeDownButton;

    public TMP_InputField roomWidthMax_input;
    public Button roomWidthUp;
    public Button roomWidthDown;

    public TMP_InputField roomHeightMax_input;
    public Button roomHeightUp;
    public Button roomHeightDown;

    public TMP_InputField hallLengthMax_input;
    public Button hallLengthMaxUp;
    public Button hallLengthMaxDown;

    public TMP_InputField hallLengthMin_input;
    public Button hallLengthMinUp;
    public Button hallLengthMinDown;

    public Button generateButton;
    public Button refreshButton;

    private Color _generateDefaultColor;
    private Color _refreshDefaultColor;
    private Color32 greyOutColor = new Color32(100, 100, 100, 255);

    private void OnEnable()
    {
        mainPathCount_inputField.onEndEdit.AddListener(OnUpdateNodes);
        nodeUpButton.onClick.AddListener(OnNodeUp);
        nodeDownButton.onClick.AddListener(OnNodeDown);

        roomWidthMax_input.onEndEdit.AddListener(OnUpdateRoomWidthMax);
        roomWidthUp.onClick.AddListener(OnRoomWidthUp);
        roomWidthDown.onClick.AddListener(OnRoomWidthDown);

        roomHeightMax_input.onEndEdit.AddListener(OnUpdateRoomHeightMax);
        roomHeightUp.onClick.AddListener(OnRoomHeightUp);
        roomHeightDown.onClick.AddListener(OnRoomHeightDown);

        hallLengthMax_input.onEndEdit.AddListener(OnUpdateHallLengthMax);
        hallLengthMaxUp.onClick.AddListener(OnHallLengthMaxUp);
        hallLengthMaxDown.onClick.AddListener(OnHallLengthMaxDown);

        hallLengthMin_input.onEndEdit.AddListener(OnUpdateHallLengthMin);
        hallLengthMinUp.onClick.AddListener(OnHallLengthMinUp);
        hallLengthMinDown.onClick.AddListener(OnHallLengthMinDown);


        generateButton.onClick.AddListener(OnGenerate);
        refreshButton.onClick.AddListener(OnRefresh);
    }
    private void OnDisable()
    {
        mainPathCount_inputField.onEndEdit.RemoveListener(OnUpdateNodes);
        nodeUpButton.onClick.RemoveListener(OnNodeUp);
        nodeDownButton.onClick.RemoveListener(OnNodeDown);

        roomWidthMax_input.onEndEdit.RemoveListener(OnUpdateRoomWidthMax);
        roomWidthUp.onClick.RemoveListener(OnRoomWidthUp);
        roomWidthDown.onClick.RemoveListener(OnRoomWidthDown);

        roomHeightMax_input.onEndEdit.RemoveListener(OnUpdateRoomHeightMax);
        roomHeightUp.onClick.RemoveListener(OnRoomHeightUp);
        roomHeightDown.onClick.RemoveListener(OnRoomHeightDown);

        hallLengthMax_input.onEndEdit.RemoveListener(OnUpdateHallLengthMax);
        hallLengthMaxUp.onClick.RemoveListener(OnHallLengthMaxUp);
        hallLengthMaxDown.onClick.RemoveListener(OnHallLengthMaxDown);

        hallLengthMin_input.onEndEdit.RemoveListener(OnUpdateHallLengthMin);
        hallLengthMinUp.onClick.RemoveListener(OnHallLengthMinUp);
        hallLengthMinDown.onClick.RemoveListener(OnHallLengthMinDown);

        generateButton.onClick.RemoveListener(OnGenerate);
        refreshButton.onClick.RemoveListener(OnRefresh);
    }

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

        mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();
        roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();
        roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();

        hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();
        hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();

        _generateDefaultColor = generateButton.image.color;
        _refreshDefaultColor = refreshButton.image.color;

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

        //grey out buttons
        if (_dungeonGenerator.currentlyGenerating || _dungeonGenerator.gen2_complete)
        {
            generateButton.image.color = greyOutColor;
        }
        else
        {
            generateButton.image.color = _generateDefaultColor;
        }

        if (_dungeonGenerator.gen2_complete && !_dungeonGenerator.currentlyGenerating)
        {
            refreshButton.image.color = _refreshDefaultColor;
        } else
        {
            refreshButton.image.color = greyOutColor;
        }


    }

    private void OnGenerate()
    {
        _dungeonGenerator.GenerateFullLayout();
    }

    private void OnRefresh()
    {
        _dungeonGenerator.Refresh();
    }

    private void OnNodeUp()
    {
        _dungeonGenerator.MainPathNodesUp();
        mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();

    }
    private void OnNodeDown()
    {
        _dungeonGenerator.MainPathNodesDown();
        mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();

    }

    private void OnUpdateNodes(string _)
    {
        if (int.TryParse(mainPathCount_inputField.text, out int newNodeCount))
        {
            _dungeonGenerator.SetMainPathNodesCount(newNodeCount);
            mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();


        }
        else
        {
            mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();
        }
    }

    private void OnUpdateRoomWidthMax(string _)
    {
        if (int.TryParse(roomWidthMax_input.text, out int newMaxWidth))
        {
            _dungeonGenerator.SetRoomWidthMax(newMaxWidth);
            roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();


        }
        else
        {
            roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();
        }

    }

    private void OnRoomWidthUp()
    {
        _dungeonGenerator.RoomWidthUp();
        roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();

    }

    private void OnRoomWidthDown()
    {
        _dungeonGenerator.RoomWidthDown();
        roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();
    }

    private void OnUpdateRoomHeightMax(string _)
    {
        if (int.TryParse(roomHeightMax_input.text, out int newMaxHeight))
        {
            _dungeonGenerator.SetRoomHeightMax(newMaxHeight);
            roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();


        }
        else
        {
            roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();
        }

    }

    private void OnRoomHeightUp()
    {
        _dungeonGenerator.RoomHeightUp();
        roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();

    }

    private void OnRoomHeightDown()
    {
        _dungeonGenerator.RoomHeightDown();
        roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();
    }

    private void OnUpdateHallLengthMax(string _)
    {
        if (int.TryParse(hallLengthMax_input.text, out int newValue))
        {
            _dungeonGenerator.SetHallLengthMax(newValue);
            hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();


        }
        else
        {
            hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();
        }

    }

    private void OnHallLengthMaxUp()
    {
        _dungeonGenerator.HallLengthMaxUp();
        hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();
    }
    private void OnHallLengthMaxDown()
    {
        _dungeonGenerator.HallLengthMaxDown();
        hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();
    }

    private void OnUpdateHallLengthMin(string _)
    {
        if (int.TryParse(hallLengthMin_input.text, out int newValue))
        {
            _dungeonGenerator.SetHallLengthMin(newValue);
            hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();

        }
        else
        {
            hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();
        }

    }

    private void OnHallLengthMinUp()
    {
        _dungeonGenerator.HallLengthMinUp();
        hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();
    }
    private void OnHallLengthMinDown()
    {
        _dungeonGenerator.HallLengthMinDown();
        hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();
    }

}
