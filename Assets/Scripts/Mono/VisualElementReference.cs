using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class VisualElementReference<T> 
	where T : VisualElement
{
	[SerializeField]
	private UIDocument uiDocument;
	public UIDocument UIDocument
	{
		get => uiDocument;
		set
		{
			uiDocument = value;
			cachedElement = null;
		}
	}

	[SerializeField]
	private string elementPath;
	public string ElementPath
	{
		get => elementPath;
		set
		{
			elementPath = value;
			cachedElement = null;
		}
	}

	private T cachedElement;

	public T VisualElement
	{
		get
		{
			if (uiDocument == null)
				return null;

			if (cachedElement != null)
				return cachedElement;

			return GetElement(uiDocument.rootVisualElement);
		}
	}

	private T GetElement(VisualElement root)
	{
		if (root == null || string.IsNullOrEmpty(elementPath))
			return null;

		var pathParts = elementPath.Split('/');
		var element = root;
		for (int i = 0; i < pathParts.Length; i++)
		{
			string part = pathParts[i];
			if (string.IsNullOrEmpty(part) || element == null)
				return null;

			var children = element.Children();
			element = children.FirstOrDefault(x => x.name == part);
		}
		return element as T;
	}
}

[System.Serializable]
public class VisualElementReference : VisualElementReference<VisualElement>
{
	public T GetElement<T>() where T : VisualElement => VisualElement as T;
}