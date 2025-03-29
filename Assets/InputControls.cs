//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/InputControls.inputactions
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

public partial class @InputControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
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
                    ""id"": ""3fdeb085-1a24-4bfc-ac09-ed0989bf7391"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Refresh"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
    }

    ~@InputControls()
    {
        UnityEngine.Debug.Assert(!m_General.enabled, "This will cause a leak and performance issues, InputControls.General.Disable() has not been called.");
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
    public struct GeneralActions
    {
        private @InputControls m_Wrapper;
        public GeneralActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @NodeSize => m_Wrapper.m_General_NodeSize;
        public InputAction @Refresh => m_Wrapper.m_General_Refresh;
        public InputAction @CreateNextNode => m_Wrapper.m_General_CreateNextNode;
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
    }
}
