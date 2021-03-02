using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine;

/// <summary>
/// A stick control displayed on screen and moved around by touch or other pointer
/// input.
/// </summary>
public class VirtualJoystickController : OnScreenControl
{
    /// <summary>
    /// ��ƽ�� �����̰� �ִ� �����ΰ�?
    /// </summary>
    public bool IsControlling { get; private set; }


    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    /// <summary>
    /// ���� �е� UI Canvas.
    /// </summary>
    private Canvas UICanvas;

    /// <summary>
    /// ���̽�ƽ ��ü(�ڵ��� �߽��� �Ǵ� �κ�) RectTransform ���۷���.
    /// </summary>
    [SerializeField]
    private RectTransform joystickBaseTransform;

    /// <summary>
    /// ���̽�ƽ �ڵ�(�����̴� ��ƽ �κ�) RectTransform ���۷���.
    /// </summary>
    [SerializeField]
    private RectTransform joystickHandleTransform;

    /// <summary>
    /// ��ƽ�� �ִ�� ������ �� �ִ� �ݰ�(RectTransform ����).
    /// </summary>
    [SerializeField]
    private float movementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    /// <summary>
    /// ȭ�� ��ġ InputAction.
    /// </summary>
    private InputAction touchAction;

    /// <summary>
    /// ȭ�� �� InputAction.
    /// </summary>
    private InputAction tapAction;

    private RectTransform rootTransform;

    private Vector2 basePointerPosition;

    private Vector3 basePosition;

    private readonly static string InputActionAssetPath = "InputSystem/InputSettings";

    public Vector2 StickValue;


    private void Awake()
    {
        UICanvas = GetComponentInParent<Canvas>();
        rootTransform = (RectTransform)transform;

        //Load actions from InputActionAssset.
        var inputActionAsset = Resources.Load(InputActionAssetPath) as InputActionAsset;
        if (inputActionAsset == null)
        {
            Debug.LogError($"ResourceNotFoundError::Failed to load asset at '{InputActionAssetPath}'");
            return;
        }

        //InputAction ���ε�.
        var uiActionMap = inputActionAsset.FindActionMap("UI");

        touchAction = uiActionMap.FindAction("Touch");
        touchAction.performed += OnDrag;

        tapAction = uiActionMap.FindAction("Tap");
        tapAction.performed += OnPointerDown;
        tapAction.canceled += OnPointerUp;

        EnableInput();
    }


    private void Start()
    {
        IsControlling = false;
        SetVisibility(false);
    }


    public void EnableInput()
    {
        touchAction.Enable();
        tapAction.Enable();
    }


    public void DisableInput()
    {
        touchAction.Disable();
        tapAction.Disable();
    }


    private void Update()
    {
        if (IsControlling)
        {
            SendValueToControl(StickValue);
        }
        else
        {
            SendValueToControl(Vector2.zero);
        }
    }


    public void SetVisibility(bool value)
    {
        joystickBaseTransform.gameObject.SetActive(value);
        joystickHandleTransform.gameObject.SetActive(value);
    }


    /// <summary>
    /// ȭ���� Tap���� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="obj"></param>
    public void OnPointerDown(InputAction.CallbackContext obj)
    {
        Vector2 screenPos = touchAction.ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, screenPos, UICanvas.worldCamera, out basePointerPosition);
        
        //���̽�ƽ ��� ���� ȭ�鿵���� ��� ���� ��ġ�� ��쿡�� �۵����� �ʴ´�.
        bool withinArea = rootTransform.rect.Contains(basePointerPosition);
        if (!withinArea) return;

        basePosition = basePointerPosition;

        joystickBaseTransform.anchoredPosition = basePosition;
        joystickHandleTransform.anchoredPosition = basePosition;

        SetVisibility(true);
        IsControlling = true;

    }


    /// <summary>
    /// ��ƽ�� ��Ʈ���ϴ� ���� ȣ��˴ϴ�.
    /// </summary>
    public void OnDrag(InputAction.CallbackContext obj)
    {
        if (!IsControlling) return;
        Vector2 screenPos = obj.ReadValue<Vector2>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, screenPos, UICanvas.worldCamera, out var newPointerPosition);
        Vector2 direction = newPointerPosition - basePointerPosition;

        direction = Vector2.ClampMagnitude(direction, movementRange);

        joystickHandleTransform.anchoredPosition = basePointerPosition + (Vector2)direction;

        StickValue = new Vector2(direction.x / movementRange, direction.y / movementRange);
    }


    /// <summary>
    /// ���� ���� ��ƽ ��Ʈ���� ������ �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="obj"></param>
    public void OnPointerUp(InputAction.CallbackContext obj)
    {
        if (!IsControlling) return;

        joystickHandleTransform.anchoredPosition = basePointerPosition;
        StickValue = Vector2.zero;

        SetVisibility(false);
        
        IsControlling = false;
    }

}
