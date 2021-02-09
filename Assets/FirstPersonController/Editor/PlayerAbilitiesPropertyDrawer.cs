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

        private ReorderableList _list;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative("_abilities");
            var list = GetList(listProperty);
            list.elementHeight = GetPropertyHeight(property, label);
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

                _list.drawHeaderCallback += OnDrawHeader;
                _list.drawElementCallback += OnDrawElement;
                _list.elementHeightCallback += GetElementHeight;
                _list.onAddDropdownCallback += OnAddDropdown;
            }

            return _list;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Abilities");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var length = _list.serializedProperty.arraySize;
            if (length <= 0)
            {
                return;
            }

            var elementProp = _list.serializedProperty.GetArrayElementAtIndex(index);
            var elementName = AbilityName(elementProp.managedReferenceFullTypename);
            EditorGUI.PropertyField(
                new Rect(
                    rect.x + 15,
                    rect.y,
                    rect.width - 15,
                    rect.height
                ), 
                elementProp, 
                new GUIContent(elementName), 
                true
            );
        }

        private static string AbilityName(string name)
        {
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private float GetElementHeight(int index)
        {
            var length = _list.serializedProperty.arraySize;
            if (length <= 0)
            {
                return 0.0f;
            }

            var elementProp = _list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(elementProp);
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();

            foreach (var abilityType in AbilityTypes)
            {
                var itemName = AbilityName(abilityType.Name);
                menu.AddItem(new GUIContent(itemName), false, OnAddAbility, abilityType);
            }

            menu.ShowAsContext();
        }

        private void OnAddAbility(object obj)
        {
            var settingsType = (Type)obj;

            var last = _list.serializedProperty.arraySize;
            _list.serializedProperty.InsertArrayElementAtIndex(last);

            var lastProp = _list.serializedProperty.GetArrayElementAtIndex(last);
            lastProp.managedReferenceValue = Activator.CreateInstance(settingsType);

            _list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}