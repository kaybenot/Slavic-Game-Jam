using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(VisualElementReference))]
public class VisualElementReferenceDrawer : PropertyDrawer
{
	private const string DocumentFieldName = "uiDocument";
	private const string ElementPathFieldName = "elementPath";

	private VisualElement rootVisualElement;
	private string[] popupOptions = null;

	private static readonly GUIContent ElementPathPropertyLabel = new GUIContent("Element Path");
	private static readonly GUIContent DocumentPropertyLabel = new GUIContent("UI Document");


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = EditorGUIUtility.singleLineHeight;
		if (property.isExpanded == false)
			return height;

		var documentProperty = property.FindPropertyRelative(DocumentFieldName);
		height += EditorGUIUtility.standardVerticalSpacing;
		height += EditorGUI.GetPropertyHeight(documentProperty);
		if (documentProperty.objectReferenceValue != null)
		{
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ElementPathFieldName));
		}

		return height;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var labelRect = position;
		labelRect.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label);

		var documentProperty = property.FindPropertyRelative(DocumentFieldName);
		var elementPathProperty = property.FindPropertyRelative(ElementPathFieldName);

		Debug.Log(documentProperty.objectReferenceValue);
		if (documentProperty.objectReferenceValue is UIDocument uiDocument)  
		{
			if (rootVisualElement == null || rootVisualElement.visualTreeAssetSource != uiDocument.visualTreeAsset)
			{
				rootVisualElement = uiDocument.rootVisualElement;
				var optionsList = ListPool<string>.Get();
				optionsList.Clear();
				optionsList.Add("Select Element");
				PopulateListWithChildrenNames(rootVisualElement, optionsList);
				popupOptions = optionsList.ToArray();
				ListPool<string>.Release(optionsList);
			}
		}
		if (property.isExpanded == false)
			DrawNotExpanded(position, documentProperty, elementPathProperty);
		else
			DrawExpanded(position, documentProperty, elementPathProperty);
	}

	private static void PopulateListWithChildrenNames(VisualElement parent, List<string> namesList, ReadOnlySpan<char> parentPath = default)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			var child = parent[i];
			if (child.name == null || string.IsNullOrEmpty(child.name))
				continue;

			var childPath = parentPath.IsEmpty ? child.name : $"{parentPath.ToString()}/{child.name}";
			namesList.Add(childPath);
			if (child.childCount > 0)
			{
				PopulateListWithChildrenNames(child, namesList, childPath.AsSpan());
			}
		}
	}

	private void DrawNotExpanded(Rect position, SerializedProperty documentProperty, SerializedProperty elementPathProperty)
	{
		var documentPropertyRect = position;
		documentPropertyRect.height = EditorGUIUtility.singleLineHeight;
		documentPropertyRect.xMin += EditorGUIUtility.labelWidth + 2;

		var uiDocument = documentProperty.objectReferenceValue as UIDocument;
		if (uiDocument != null)
		{
			float propertyWidth = documentPropertyRect.width / 2;
			documentPropertyRect.xMax -= (propertyWidth + 1);
		}

		EditorGUI.PropertyField(documentPropertyRect, documentProperty, GUIContent.none);
		if (uiDocument == null)
			return;

		var elementPathPropertyRect = documentPropertyRect;
		elementPathPropertyRect.xMin += documentPropertyRect.width + 1;
		elementPathPropertyRect.width = documentPropertyRect.width - 1;
		DrawPathDropdown(elementPathPropertyRect, elementPathProperty, GUIContent.none);
	}

	private void DrawExpanded(Rect position, SerializedProperty documentProperty, SerializedProperty elementPathProperty)
	{
		EditorGUI.indentLevel++;
		Draw(position, documentProperty, elementPathProperty);
		EditorGUI.indentLevel--;

		void Draw(Rect position, SerializedProperty documentProperty, SerializedProperty elementPathProperty)
		{
			var documentPropertyRect = position;
			documentPropertyRect.yMin += EditorGUIUtility.singleLineHeight + 1;
			documentPropertyRect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(documentPropertyRect, documentProperty, DocumentPropertyLabel);

			var uiDocument = documentProperty.objectReferenceValue as UIDocument;
			if (uiDocument == null)
				return;

			var elementPathPropertyRect = documentPropertyRect;
			elementPathPropertyRect.yMin += EditorGUIUtility.singleLineHeight + 1;
			elementPathPropertyRect.height = EditorGUIUtility.singleLineHeight;

			DrawPathDropdown(elementPathPropertyRect, elementPathProperty, ElementPathPropertyLabel);
		}
	}

	private void DrawPathDropdown(Rect position, SerializedProperty elementPathProperty, GUIContent label)
	{
		if (popupOptions == null)
			return;

		string elementPath = elementPathProperty.stringValue;
		int oldIndex = Array.FindIndex(popupOptions, x => x == elementPath);
		if (oldIndex < 0)
			oldIndex = 0;

		int newIndex = EditorGUI.Popup(position, label.text, oldIndex, popupOptions);
		if (newIndex != oldIndex)
		{
			elementPathProperty.stringValue = newIndex == 0 ? string.Empty : popupOptions[newIndex];
			elementPathProperty.serializedObject.ApplyModifiedProperties();
		}
	}
}
