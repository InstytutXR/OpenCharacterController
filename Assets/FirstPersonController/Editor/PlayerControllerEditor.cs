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
        private static readonly List<Type> AbilityTypes = new List<Type>();
        
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

        static PlayerControllerEditor()
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAbility));

            AbilityTypes.AddRange(
                assembly
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && typeof(PlayerAbility).IsAssignableFrom(type))
                    .OrderBy(t => t.Name)
            );
        }

        private void OnEnable()
        {
            _list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("_abilities"), 
                true, 
                true,
                true, 
                true
            );

            _headerStyle = new GUIStyle();
            _headerStyle.alignment = TextAnchor.MiddleLeft;
            _headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? ProSkinTextColor : PersonalSkinTextColor;
            _headerStyle.fontStyle = FontStyle.Bold;

            _list.drawHeaderCallback += OnDrawAbilitiesHeader;
            _list.drawElementCallback += OnDrawAbilityListElement;
            _list.elementHeightCallback += GetAbilityListElementHeight;
            _list.onAddDropdownCallback += OnAbilityAddDropdown;
        }

        private void OnDisable()
        {
            _list.drawElementCallback -= OnDrawAbilityListElement;
            _list.elementHeightCallback -= GetAbilityListElementHeight;
            _list.drawHeaderCallback -= OnDrawAbilitiesHeader;
            _list.onAddDropdownCallback -= OnAbilityAddDropdown;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawAbilitiesHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Abilities");
        }

        private void OnDrawAbilityListElement(Rect rect, int index, bool isActive, bool isFocused)
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
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private float GetAbilityListElementHeight(int index)
        {
            var length = _list.serializedProperty.arraySize;

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

            var i = 0;
            while (iteratorProp.NextVisible(true) && !EqualContents(endProp, iteratorProp))
            {
                var multiplier = i == 0 ? AdditionalSpaceMultiplier : 1.0f;
                height += GetDefaultSpaceBetweenElements() * multiplier;
                i++;
            }

            return height;
        }

        private void OnAbilityAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();

            foreach (var abilityType in AbilityTypes)
            {
                var itemName = AbilityName(abilityType.Name);
                menu.AddItem(new GUIContent(itemName), false, OnAddAbilityType, abilityType);
            }

            menu.ShowAsContext();
        }

        private void OnAddAbilityType(object obj)
        {
            var settingsType = (Type)obj;

            var last = _list.serializedProperty.arraySize;
            _list.serializedProperty.InsertArrayElementAtIndex(last);

            var lastProp = _list.serializedProperty.GetArrayElementAtIndex(last);
            lastProp.managedReferenceValue = Activator.CreateInstance(settingsType);

            serializedObject.ApplyModifiedProperties();
        }

        private static float GetDefaultSpaceBetweenElements()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private static bool EqualContents(SerializedProperty a, SerializedProperty b)
        {
            return SerializedProperty.EqualContents(a, b);
        }
    }
}
