// Editor code here heavily inspired by the work of Cristian Qiu Félez
// and their repo: https://github.com/CristianQiu/Unity-Editor-PolymorphicReorderableList

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FirstPersonController
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        private const float AdditionalSpaceMultiplier = 1.0f;

        private const float HeightHeader = 20.0f;
        private const float MarginReorderIcon = 20.0f;
        private const float ShrinkHeaderWidth = 15.0f;
        private const float XShiftHeaders = 15.0f;

        private static readonly Color ProSkinTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        private static readonly Color PersonalSkinTextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

        private static readonly Color ProSkinSelectionBgColor = new Color(44.0f / 255.0f, 93.0f / 255.0f, 135.0f / 255.0f, 1.0f);
        private static readonly Color PersonalSkinSelectionBgColor = new Color(58.0f / 255.0f, 114.0f / 255.0f, 176.0f / 255.0f, 1.0f);

        private ReorderableList _list;
        private GUIStyle _headerStyle;

        private void OnEnable()
        {
            _list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("_abilities"), 
                true, 
                true,
                true, 
                false
            );

            _headerStyle = new GUIStyle();
            _headerStyle.alignment = TextAnchor.MiddleLeft;
            _headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? ProSkinTextColor : PersonalSkinTextColor;
            _headerStyle.fontStyle = FontStyle.Bold;

            _list.drawHeaderCallback += OnDrawReorderListHeader;
            _list.drawElementCallback += OnDrawReorderListElement;
            _list.drawElementBackgroundCallback += OnDrawReorderListBg;
            _list.elementHeightCallback += OnReorderListElementHeight;
            _list.onAddDropdownCallback += OnReorderListAddDropdown;
        }

        private void OnDisable()
        {
            _list.drawElementCallback -= OnDrawReorderListElement;
            _list.elementHeightCallback -= OnReorderListElementHeight;
            _list.drawElementBackgroundCallback -= OnDrawReorderListBg;
            _list.drawHeaderCallback -= OnDrawReorderListHeader;
            _list.onAddDropdownCallback -= OnReorderListAddDropdown;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawReorderListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Abilities");
        }

        private void OnDrawReorderListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            int length = _list.serializedProperty.arraySize;

            if (length <= 0)
            {
                return;
            }

            var iteratorProp = _list.serializedProperty.GetArrayElementAtIndex(index);
            var elementName = AbilityName(iteratorProp.managedReferenceFullTypename);

            Rect labelfoldRect = rect;
            labelfoldRect.height = HeightHeader;
            labelfoldRect.x += XShiftHeaders;
            labelfoldRect.width -= ShrinkHeaderWidth;

            iteratorProp.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(
                labelfoldRect,
                iteratorProp.isExpanded,
                elementName
            );

            if (iteratorProp.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    var endProp = iteratorProp.GetEndProperty();

                    int i = 0;
                    while (iteratorProp.NextVisible(true) && !EqualContents(endProp, iteratorProp))
                    {
                        float multiplier = i == 0 ? AdditionalSpaceMultiplier : 1.0f;
                        rect.y += GetDefaultSpaceBetweenElements() * multiplier;
                        rect.height = EditorGUIUtility.singleLineHeight;

                        EditorGUI.PropertyField(rect, iteratorProp, true);

                        i++;
                    }
                }
            }

            EditorGUI.EndFoldoutHeaderGroup();
        }

        private static string AbilityName(string name)
        {
            name = name.Substring(name.LastIndexOf('.') + 1);
            name = name.Substring(0, name.IndexOf("Ability"));
            return name;
        }

        private void OnDrawReorderListBg(Rect rect, int index, bool isActive, bool isFocused)
        {
            //if (isFocused && isActive)
            //{
            //    float height = OnReorderListElementHeight(index);

            //    var prop = _list.serializedProperty.GetArrayElementAtIndex(index);

            //    if (!prop.isExpanded)
            //    {
            //        height -= EditorGUIUtility.standardVerticalSpacing;
            //    }

            //    Rect copyRect = rect;
            //    copyRect.width = MarginReorderIcon;
            //    copyRect.height = height;

            //    // draw two rects indepently to avoid overlapping the header label
            //    Color color = EditorGUIUtility.isProSkin ? ProSkinSelectionBgColor : PersonalSkinSelectionBgColor;
            //    EditorGUI.DrawRect(rect, color);

            //    float offset = 2.0f;
            //    rect.x += MarginReorderIcon;
            //    rect.width -= (MarginReorderIcon + offset);

            //    rect.height = height - HeightHeader + offset;
            //    rect.y += HeightHeader - offset;

            //    EditorGUI.DrawRect(rect, color);
            //}
        }

        private float OnReorderListElementHeight(int index)
        {
            int length = _list.serializedProperty.arraySize;

            if (length <= 0)
            {
                return 0.0f;
            }

            var iteratorProp = _list.serializedProperty.GetArrayElementAtIndex(index);
            var endProp = iteratorProp.GetEndProperty();

            var height = GetDefaultSpaceBetweenElements();

            if (!iteratorProp.isExpanded)
            {
                return height;
            }

            int i = 0;
            while (iteratorProp.NextVisible(true) && !EqualContents(endProp, iteratorProp))
            {
                float multiplier = i == 0 ? AdditionalSpaceMultiplier : 1.0f;
                height += GetDefaultSpaceBetweenElements() * multiplier;
                i++;
            }

            return height;
        }

        private void OnReorderListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var showTypes = GetNonAbstractTypesSubclassOf<PlayerAbility>();

            for (int i = 0; i < showTypes.Count; i++)
            {
                var type = showTypes[i];
                var itemName = AbilityName(showTypes[i].Name);
                menu.AddItem(new GUIContent(itemName), true, OnAddItemFromDropdown, type);
            }

            menu.ShowAsContext();
        }

        private void OnAddItemFromDropdown(object obj)
        {
            var settingsType = (Type)obj;

            int last = _list.serializedProperty.arraySize;
            _list.serializedProperty.InsertArrayElementAtIndex(last);

            var lastProp = _list.serializedProperty.GetArrayElementAtIndex(last);
            lastProp.managedReferenceValue = Activator.CreateInstance(settingsType);

            serializedObject.ApplyModifiedProperties();
        }

        private float GetDefaultSpaceBetweenElements()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private bool EqualContents(SerializedProperty a, SerializedProperty b)
        {
            return SerializedProperty.EqualContents(a, b);
        }

        private List<Type> GetNonAbstractTypesSubclassOf<T>(bool sorted = true) where T : class
        {
            var assembly = Assembly.GetAssembly(typeof(T));

            var types = 
                assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(T).IsAssignableFrom(type))
                .ToList();

            if (sorted)
            {
                types.Sort(CompareTypesNames);
            }

            return types;
        }

        private int CompareTypesNames(Type a, Type b)
        {
            return a.Name.CompareTo(b.Name);
        }
    }
}
