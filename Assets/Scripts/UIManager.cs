using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private InputActions _inputActions;
    private Camera _camera;
    private CameraController _cameraController;

    public GameObject textObject;
    public GameObject imageObject;
    public GameObject dungeongenerator_obj;
    public GameObject instantiator_obj;

    private DungeonGenerator _dungeonGenerator;
    private Instantiator _instantiator;



    private TextMeshProUGUI _generationText;
    private float _generationStartTime;
    private bool _wasGeneratingLastFrame;

    public Button defaultValues;

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

    public TMP_InputField roomProb_input;
    public Button roomProbUp;
    public Button roomProbDown;

    public Button generateButton;
    public Button refreshButton;

    public Button instantiateButton;

    [SerializeField] private float mouseScrollFactor;

    private Color _generateDefaultColor;
    private Color _refreshDefaultColor;
    private Color _instantiateDefaultColor;

    private Color32 greyOutColor = new Color32(100, 100, 100, 255);

    private Vector3 cameraDefaultPosition;
    private float cameraDefaultSize;

    private Vector3 _dragOrigin; // Stores initial click position in world space
    private bool _isDragging = false;
    private Vector2 _dragOriginScreen;

    private void Awake()
    {
        _inputActions = new InputActions();

#if UNITY_EDITOR
        UnityEditor.EditorWindow.focusedWindow.SendEvent(new Event { type = EventType.Ignore });
#endif


    }
    private void OnEnable()
    {
        _inputActions.General.Enable();
        _inputActions.General.ZoomScreen.performed += OnZoom;

        _inputActions.General.MoveCamera.started += OnMoveCamera;
        _inputActions.General.MoveCamera.performed += OnMoveCamera;
        _inputActions.General.MoveCamera.canceled += OnMoveCamera;

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

        roomProb_input.onEndEdit.AddListener(OnUpdateRoomProb);
        roomProbUp.onClick.AddListener(OnRoomProbUp);
        roomProbDown.onClick.AddListener(OnRoomProbDown);

        defaultValues.onClick.AddListener(OnDefaultValues);
        generateButton.onClick.AddListener(OnGenerate);
        refreshButton.onClick.AddListener(OnRefresh);

        instantiateButton.onClick.AddListener(OnInstantiate);
    }
    private void OnDisable()
    {
        _inputActions.General.Disable();

        _inputActions.General.ZoomScreen.performed -= OnZoom;

        _inputActions.General.MoveCamera.started -= OnMoveCamera;
        _inputActions.General.MoveCamera.performed -= OnMoveCamera;
        _inputActions.General.MoveCamera.canceled -= OnMoveCamera;

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

        roomProb_input.onEndEdit.RemoveListener(OnUpdateRoomProb);
        roomProbUp.onClick.RemoveListener(OnRoomProbUp);
        roomProbDown.onClick.RemoveListener(OnRoomProbDown);

        defaultValues.onClick.RemoveListener(OnDefaultValues);
        generateButton.onClick.RemoveListener(OnGenerate);
        refreshButton.onClick.RemoveListener(OnRefresh);

        instantiateButton.onClick.RemoveListener(OnInstantiate);
    }

    void Start()
    {
        _camera = Camera.main;
        cameraDefaultPosition = _camera.transform.position;
        cameraDefaultSize = _camera.orthographicSize;
        _cameraController = _camera.GetComponent<CameraController>();

        _dungeonGenerator = dungeongenerator_obj.GetComponent<DungeonGenerator>();
        _instantiator = instantiator_obj.GetComponent<Instantiator>();

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

        roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();

        _generateDefaultColor = generateButton.image.color;
        _refreshDefaultColor = refreshButton.image.color;
        _instantiateDefaultColor = instantiateButton.image.color;

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
            if (!_instantiator.isInstantiated)
            {
                instantiateButton.image.color = _instantiateDefaultColor;

            }
            else
            {
                instantiateButton.image.color = greyOutColor;

            }

            refreshButton.image.color = _refreshDefaultColor;

        }
        else
        {
            refreshButton.image.color = greyOutColor;
            instantiateButton.image.color = greyOutColor;
        }

        if (_isDragging)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            float sensitivity = 0.5f * (_camera.orthographicSize / cameraDefaultSize);

            Vector2 delta = (mousePos - _dragOriginScreen) * sensitivity; // Sensitivity factor

            // Convert screen delta to world movement
            _camera.transform.position -= new Vector3(delta.x, 0, delta.y);
            _dragOriginScreen = mousePos; // Update for next frame
        }

    }


    private void OnZoom(InputAction.CallbackContext context)
    {
        if (_dungeonGenerator.currentlyGenerating || !_dungeonGenerator.gen2_complete)
            return;

        // Read the scroll delta (y-axis for vertical scroll)
        float scrollDeltaY = context.ReadValue<Vector2>().y;

        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - (_camera.orthographicSize * scrollDeltaY * mouseScrollFactor), 0, 5000);
    }

    private void OnMoveCamera(InputAction.CallbackContext context)
    {
        if (_dungeonGenerator.currentlyGenerating || !_dungeonGenerator.gen2_complete)
            return;

        if (context.started)
        {
            _dragOriginScreen = Mouse.current.position.ReadValue();
            _isDragging = true;
        }
        else if (context.canceled)
        {
            _isDragging = false;
        }


    }
    private void OnDefaultValues()
    {
        _dungeonGenerator.SetDefaultValues();
        mainPathCount_inputField.text = _dungeonGenerator.mainPath_nodeCount.ToString();
        roomWidthMax_input.text = _dungeonGenerator.max_roomWidth.ToString();
        roomHeightMax_input.text = _dungeonGenerator.max_roomHeight.ToString();

        hallLengthMax_input.text = _dungeonGenerator.hall_maxLength.ToString();
        hallLengthMin_input.text = _dungeonGenerator.hall_minLength.ToString();

        roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();


    }

    private void OnGenerate()
    {
        _dungeonGenerator.GenerateFullLayout();
    }

    private void OnRefresh()
    {
        _dungeonGenerator.Refresh();
        _instantiator.DestroyFloorAndWalls();
        _cameraController.ResetCameraViewSize();
        _camera.transform.position = cameraDefaultPosition;
    }

    private void OnInstantiate()
    {
        if (_dungeonGenerator.gen2_complete && !_dungeonGenerator.currentlyGenerating && !_instantiator.isInstantiated)
        {
            _instantiator.InstantiateDungeon(_dungeonGenerator.croppedMatrix);
        }
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

    private void OnUpdateRoomProb(string _)
    {
        if (float.TryParse(roomProb_input.text, out float newValue))
        {
            _dungeonGenerator.SetRoomProb(newValue);
            roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();

        }
        else
        {
            roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();
        }
    }

    private void OnRoomProbUp()
    {
        _dungeonGenerator.RoomProbUp();
        roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();

    }

    private void OnRoomProbDown()
    {
        _dungeonGenerator.RoomProbDown();
        roomProb_input.text = (_dungeonGenerator.room_prob * 100).ToString();
    }
}
