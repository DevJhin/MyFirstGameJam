using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 기본 요소의 기본 구성
/// </summary>
public abstract class UIBase : MonoBehaviour
{
	/// <summary>
	/// Bind를 통해 불러온 자식 오브젝트를 타입별로 저장합니다.
	/// </summary>
	private Dictionary<Type, UnityEngine.Object[]> childObjects = new Dictionary<Type, UnityEngine.Object[]>();

	private Canvas canvas;

	protected virtual void OnEnable()
	{
		canvas = GetComponentInParent<Canvas>();
	}

	protected virtual void Start()
	{

	}

	/// <summary>
	/// 이 캔버스의 Sorting Order를 설정합니다.
	/// * 옵션창 같이 팝업창 형태(나중에 뜨는 창이 위로 가게)로 뜨는 UI들이 많아지면 UI Manager를 통해 자동으로 설정되도록 할 수도 있을듯
	/// </summary>
	public void SetCanvasOrder(int order)
	{
		canvas.sortingOrder = order;
	}

	/// <summary>
	/// 캔버스에 소속된 하위 오브젝트를 불러와 저장합니다. 같은 타입에 대한 Bind는 1회만 호출해야 합니다.
	/// 오브젝트의 이름과 Enum 자료형 내의 이름이 일치해야 하며, 오브젝트에 T 타입의 컴포넌트가 있어야 합니다.
	/// </summary>
	/// <typeparam name="T">저장할 컴포넌트(또는 GameObject) 타입을 입력합니다</typeparam>
	/// <param name="type">저장할 오브젝트의 이름들을 담은 열거형 자료를 typeof(Enums)와 같이 입력합니다. Enum 값은 0부터 순차적으로 할당되어야 합니다.</param>
	protected void Bind<T>(Type type) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);

		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
		childObjects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			objects[i] = Utility.FindChild<T>(gameObject, names[i]);

			if (objects[i] == null)
				Debug.Log($"Failed to Bind : {names[i]} ({type.Name})");
		}
	}

	/// <summary>
	/// 저장된 T타입 컴포넌트를 반환합니다. Bind에서 입력한 컴포넌트 타입과 Enum값 내의 불러올 오브젝트명을 정수로 입력해야 합니다.
	/// </summary>
	protected T Get<T>(int index) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects;
		if (childObjects.TryGetValue(typeof(T), out objects))
			return objects[index] as T;
		else
			return null;
	}

	protected Button GetButton(int index) => Get<Button>(index);
}
