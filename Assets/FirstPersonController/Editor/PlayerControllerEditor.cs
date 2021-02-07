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
    [CustomPropertyDrawer(typeof(PlayerAbilities))]
    public class PlayerAbilitiesPropertyDrawer : PropertyDrawer
    {
        private static readonly List<Type> AbilityTypes = new List<Type>();

        static PlayerAbilitiesPropertyDrawer()
        {
            var assembly = Assembly.GetAssembly(typeof(PlayerAbility));

            AbilityTypes.AddRange(
                from t in assembly.GetTypes()
                where t.IsClass && !t.IsAbstract && typeof(PlayerAbility).IsAssignableFrom(t)
                orderby t.Name
                select t
            );
        }

        private const float AdditionalSpaceMultiplier = 1.0f;

        private const float HeightHeader = 20.0f;
        private const float ShrinkHeaderWidth = 15.0f;
        private const float XShiftHeaders = 15.0f;

        private ReorderableList _list;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("_abilities");
            var list = GetList(listProperty);

            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(
                    height,
                    EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i))
                );
            }

            list.elementHeight = height;
            list.DoList(rect);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("_abilities");
            return GetList(listProperty).GetHeight();
        }

        private ReorderableList GetList(SerializedProperty serializedProperty)
        {
            if (_list == null)
            {
                _list = new ReorderableList(
                    serializedProperty.serializedObject,
                    serializedProperty,
                    true,
                    true,
                    true,
                    true
                );

                _list.drawHeaderCallback += OnDrawAbilitiesHeader;
                _list.drawElementCallback += OnDrawAbilityListElement;
                _list.elementHeightCallback += GetAbilityListElementHeight;
                _list.onAddDropdownCallback += OnAbilityAddDropdown;
            }

            return _list;
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

            var foldoutRect = rect;
            foldoutRect.height = HeightHeader;
            foldoutRect.x += XShiftHeaders;
            foldoutRect.width -= ShrinkHeaderWidth;

            iteratorProp.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(
                foldoutRect,
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
            var settingsType = (Type) obj;

            var last = _list.serializedProperty.arraySize;
            _list.serializedProperty.InsertArrayElementAtIndex(last);

            var lastProp = _list.serializedProperty.GetArrayElementAtIndex(last);
            lastProp.managedReferenceValue = Activator.CreateInstance(settingsType);
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