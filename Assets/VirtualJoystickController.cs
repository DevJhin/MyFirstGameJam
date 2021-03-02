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
    /// 스틱을 움직이고 있는 상태인가?
    /// </summary>
    public bool IsControlling { get; private set; }


    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    /// <summary>
    /// 가상 패드 UI Canvas.
    /// </summary>
    private Canvas UICanvas;

    /// <summary>
    /// 조이스틱 몸체(핸들의 중심이 되는 부분) RectTransform 레퍼런스.
    /// </summary>
    [SerializeField]
    private RectTransform joystickBaseTransform;

    /// <summary>
    /// 조이스틱 핸들(움직이는 스틱 부분) RectTransform 레퍼런스.
    /// </summary>
    [SerializeField]
    private RectTransform joystickHandleTransform;

    /// <summary>
    /// 스틱이 최대로 움직일 수 있는 반경(RectTransform 기준).
    /// </summary>
    [SerializeField]
    private float movementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    /// <summary>
    /// 화면 터치 InputAction.
    /// </summary>
    private InputAction touchAction;

    /// <summary>
    /// 화면 탭 InputAction.
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

        //InputAction 바인딩.
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
    /// 화면을 Tap했을 때 호출됩니다.
    /// </summary>
    /// <param name="obj"></param>
    public void OnPointerDown(InputAction.CallbackContext obj)
    {
        Vector2 screenPos = touchAction.ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, screenPos, UICanvas.worldCamera, out basePointerPosition);
        
        //조이스틱 사용 가능 화면영역을 벗어난 곳을 터치한 경우에는 작동하지 않는다.
        bool withinArea = rootTransform.rect.Contains(basePointerPosition);
        if (!withinArea) return;

        basePosition = basePointerPosition;

        joystickBaseTransform.anchoredPosition = basePosition;
        joystickHandleTransform.anchoredPosition = basePosition;

        SetVisibility(true);
        IsControlling = true;

    }


    /// <summary>
    /// 스틱을 컨트롤하는 동안 호출됩니다.
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
    /// 손을 떼어 스틱 컨트롤을 멈췄을 때 호출됩니다.
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
