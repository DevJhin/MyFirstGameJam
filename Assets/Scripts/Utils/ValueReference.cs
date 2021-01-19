using Sirenix.OdinInspector;
using UnityEngine;


[InlineProperty]
public abstract class ValueReference<TValue, TAsset> where TAsset : ValueAsset<TValue>
{
    [HorizontalGroup("Reference", MaxWidth = 100)]
    [ValueDropdown("valueList")]
    [HideLabel]
    [SerializeField]
    protected bool useValue = true;

    [ShowIf("useValue", Animate = false)]
    [HideLabel]
    [SerializeField]
   // [VerticalGroup("Reference/Hidden")]
    protected TValue _value;


    [HideIf("useValue", Animate = false)]
    [HorizontalGroup("Reference")]
    [OnValueChanged("UpdateAsset")]
    [HideLabel]
    [SerializeField]
    protected TAsset assetReference;

    [ShowIf("@assetReference != null && useValue == false")]
    [LabelWidth(100)]
    [SerializeField]
    protected bool editAsset = false;

    [ShowIf("@assetReference != null && useValue == false")]
    [EnableIf("editAsset")]
    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [SerializeField]
    protected TAsset _assetReference;


    private static ValueDropdownList<bool> valueList = new ValueDropdownList<bool>()
    {
        { "Value", true},
        { "Reference", false}
    };

    public TValue value
    {
        get
        {
            if (assetReference == null || useValue)
            {
                return _value;
            }
            else
            {
                return assetReference.Value;
            }
        }


    }

    protected void UpdateAsset()
    {
        _assetReference = assetReference;
    }

    public static implicit operator TValue(ValueReference<TValue, TAsset> valueRef)
    {
        return valueRef.value;
    }

}