//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""General"",
            ""id"": ""6feb5628-6e7b-4948-9323-803e1a717f07"",
            ""actions"": [
                {
                    ""name"": ""NodeSize"",
                    ""type"": ""Button"",
                    ""id"": ""1e57045a-e781-4e62-b0e1-a2dd76418906"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Refresh"",
                    ""type"": ""Button"",
                    ""id"": ""cb931bcf-0ccf-4f47-86f3-02c6e7dfdaeb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CreateNextNode"",
                    ""type"": ""Button"",
                    ""id"": ""d8ae39bb-fd6c-4ae9-b7de-ac3f9b003809"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GenerateFullLayout"",
                    ""type"": ""Button"",
                    ""id"": ""07451ccf-b71d-415a-a753-96d2d5a0b41e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ZoomScreen"",
                    ""type"": ""Value"",
                    ""id"": ""7c92e578-4aba-4f68-b885-cfab70a2b0ab"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""7a0a78fe-8251-4e3c-903a-ac34db9ca619"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MovePlayer"",
                    ""type"": ""Value"",
                    ""id"": ""d45f8bb7-44cf-422b-b1b8-99f9b982153b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseLook"",
                    ""type"": ""Value"",
                    ""id"": ""3519e791-4491-421b-9c5c-dcb1b7c9858d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""f57c6f4f-1e0b-47d7-aae5-0fd83a2d86a2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ReturnToOverhead"",
                    ""type"": ""Button"",
                    ""id"": ""da5ec2e2-91db-423b-b190-034ea84d4ea8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""bf808e87-8191-4004-af4f-59cbd51e76cd"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NodeSize"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""f8e26571-eae7-4b7e-87c3-28d589f46445"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NodeSize"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ef877544-0be9-488f-a226-827e22ee254e"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NodeSize"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""330f8229-2e37-4b8b-9dab-f7e3e011eb67"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CreateNextNode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c3145af-0fd7-4be4-8bda-0f78cc248e89"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GenerateFullLayout"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4558b01c-59f1-4fb0-bf9e-af3d2ec6dc52"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomScreen"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""687d5baf-7618-44e0-a42c-59cfb1316eca"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1e4f3b29-abf3-404b-9ae7-6b3d5dfcd039"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cf658f81-4fc3-4f77-9ffc-a52504308f2c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""67c1cd53-d934-4fb9-a84c-47a5f000b286"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ecbdea24-8f30-4a64-a05c-cca1abd9a59d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f0eb07c9-2359-4848-b08a-be371027bf20"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovePlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""63570e48-5832-4584-a524-c1ab63bc7eac"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e8a7659-8c2f-4a2d-a527-2655d2f19638"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3680c233-afaf-4c58-b9db-0f00b75e9e0f"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ReturnToOverhead"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // General
        m_General = asset.FindActionMap("General", throwIfNotFound: true);
        m_General_NodeSize = m_General.FindAction("NodeSize", throwIfNotFound: true);
        m_General_Refresh = m_General.FindAction("Refresh", throwIfNotFound: true);
        m_General_CreateNextNode = m_General.FindAction("CreateNextNode", throwIfNotFound: true);
        m_General_GenerateFullLayout = m_General.FindAction("GenerateFullLayout", throwIfNotFound: true);
        m_General_ZoomScreen = m_General.FindAction("ZoomScreen", throwIfNotFound: true);
        m_General_MoveCamera = m_General.FindAction("MoveCamera", throwIfNotFound: true);
        m_General_MovePlayer = m_General.FindAction("MovePlayer", throwIfNotFound: true);
        m_General_MouseLook = m_General.FindAction("MouseLook", throwIfNotFound: true);
        m_General_Jump = m_General.FindAction("Jump", throwIfNotFound: true);
        m_General_ReturnToOverhead = m_General.FindAction("ReturnToOverhead", throwIfNotFound: true);
    }

    ~@InputActions()
    {
        UnityEngine.Debug.Assert(!m_General.enabled, "This will cause a leak and performance issues, InputActions.General.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // General
    private readonly InputActionMap m_General;
    private List<IGeneralActions> m_GeneralActionsCallbackInterfaces = new List<IGeneralActions>();
    private readonly InputAction m_General_NodeSize;
    private readonly InputAction m_General_Refresh;
    private readonly InputAction m_General_CreateNextNode;
    private readonly InputAction m_General_GenerateFullLayout;
    private readonly InputAction m_General_ZoomScreen;
    private readonly InputAction m_General_MoveCamera;
    private readonly InputAction m_General_MovePlayer;
    private readonly InputAction m_General_MouseLook;
    private readonly InputAction m_General_Jump;
    private readonly InputAction m_General_ReturnToOverhead;
    public struct GeneralActions
    {
        private @InputActions m_Wrapper;
        public GeneralActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @NodeSize => m_Wrapper.m_General_NodeSize;
        public InputAction @Refresh => m_Wrapper.m_General_Refresh;
        public InputAction @CreateNextNode => m_Wrapper.m_General_CreateNextNode;
        public InputAction @GenerateFullLayout => m_Wrapper.m_General_GenerateFullLayout;
        public InputAction @ZoomScreen => m_Wrapper.m_General_ZoomScreen;
        public InputAction @MoveCamera => m_Wrapper.m_General_MoveCamera;
        public InputAction @MovePlayer => m_Wrapper.m_General_MovePlayer;
        public InputAction @MouseLook => m_Wrapper.m_General_MouseLook;
        public InputAction @Jump => m_Wrapper.m_General_Jump;
        public InputAction @ReturnToOverhead => m_Wrapper.m_General_ReturnToOverhead;
        public InputActionMap Get() { return m_Wrapper.m_General; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
        public void AddCallbacks(IGeneralActions instance)
        {
            if (instance == null || m_Wrapper.m_GeneralActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GeneralActionsCallbackInterfaces.Add(instance);
            @NodeSize.started += instance.OnNodeSize;
            @NodeSize.performed += instance.OnNodeSize;
            @NodeSize.canceled += instance.OnNodeSize;
            @Refresh.started += instance.OnRefresh;
            @Refresh.performed += instance.OnRefresh;
            @Refresh.canceled += instance.OnRefresh;
            @CreateNextNode.started += instance.OnCreateNextNode;
            @CreateNextNode.performed += instance.OnCreateNextNode;
            @CreateNextNode.canceled += instance.OnCreateNextNode;
            @GenerateFullLayout.started += instance.OnGenerateFullLayout;
            @GenerateFullLayout.performed += instance.OnGenerateFullLayout;
            @GenerateFullLayout.canceled += instance.OnGenerateFullLayout;
            @ZoomScreen.started += instance.OnZoomScreen;
            @ZoomScreen.performed += instance.OnZoomScreen;
            @ZoomScreen.canceled += instance.OnZoomScreen;
            @MoveCamera.started += instance.OnMoveCamera;
            @MoveCamera.performed += instance.OnMoveCamera;
            @MoveCamera.canceled += instance.OnMoveCamera;
            @MovePlayer.started += instance.OnMovePlayer;
            @MovePlayer.performed += instance.OnMovePlayer;
            @MovePlayer.canceled += instance.OnMovePlayer;
            @MouseLook.started += instance.OnMouseLook;
            @MouseLook.performed += instance.OnMouseLook;
            @MouseLook.canceled += instance.OnMouseLook;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @ReturnToOverhead.started += instance.OnReturnToOverhead;
            @ReturnToOverhead.performed += instance.OnReturnToOverhead;
            @ReturnToOverhead.canceled += instance.OnReturnToOverhead;
        }

        private void UnregisterCallbacks(IGeneralActions instance)
        {
            @NodeSize.started -= instance.OnNodeSize;
            @NodeSize.performed -= instance.OnNodeSize;
            @NodeSize.canceled -= instance.OnNodeSize;
            @Refresh.started -= instance.OnRefresh;
            @Refresh.performed -= instance.OnRefresh;
            @Refresh.canceled -= instance.OnRefresh;
            @CreateNextNode.started -= instance.OnCreateNextNode;
            @CreateNextNode.performed -= instance.OnCreateNextNode;
            @CreateNextNode.canceled -= instance.OnCreateNextNode;
            @GenerateFullLayout.started -= instance.OnGenerateFullLayout;
            @GenerateFullLayout.performed -= instance.OnGenerateFullLayout;
            @GenerateFullLayout.canceled -= instance.OnGenerateFullLayout;
            @ZoomScreen.started -= instance.OnZoomScreen;
            @ZoomScreen.performed -= instance.OnZoomScreen;
            @ZoomScreen.canceled -= instance.OnZoomScreen;
            @MoveCamera.started -= instance.OnMoveCamera;
            @MoveCamera.performed -= instance.OnMoveCamera;
            @MoveCamera.canceled -= instance.OnMoveCamera;
            @MovePlayer.started -= instance.OnMovePlayer;
            @MovePlayer.performed -= instance.OnMovePlayer;
            @MovePlayer.canceled -= instance.OnMovePlayer;
            @MouseLook.started -= instance.OnMouseLook;
            @MouseLook.performed -= instance.OnMouseLook;
            @MouseLook.canceled -= instance.OnMouseLook;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @ReturnToOverhead.started -= instance.OnReturnToOverhead;
            @ReturnToOverhead.performed -= instance.OnReturnToOverhead;
            @ReturnToOverhead.canceled -= instance.OnReturnToOverhead;
        }

        public void RemoveCallbacks(IGeneralActions instance)
        {
            if (m_Wrapper.m_GeneralActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGeneralActions instance)
        {
            foreach (var item in m_Wrapper.m_GeneralActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GeneralActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GeneralActions @General => new GeneralActions(this);
    public interface IGeneralActions
    {
        void OnNodeSize(InputAction.CallbackContext context);
        void OnRefresh(InputAction.CallbackContext context);
        void OnCreateNextNode(InputAction.CallbackContext context);
        void OnGenerateFullLayout(InputAction.CallbackContext context);
        void OnZoomScreen(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnMovePlayer(InputAction.CallbackContext context);
        void OnMouseLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnReturnToOverhead(InputAction.CallbackContext context);
    }
}
