using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FirstPersonController
{
    [CustomPropertyDrawer(typeof(PlayerAbilities))]
    public class PlayerAbilitiesPropertyDrawer : PropertyDrawer
    {
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
            }

            return _list;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Abilities");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index <= _list.serializedProperty.arraySize)
            {
                var elementProp = _list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, elementProp);
            }
        }

        private float GetElementHeight(int index)
        {
            if (index <= _list.serializedProperty.arraySize)
            {
                var elementProp = _list.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(elementProp);
            }
            else
            {
                return 0;
            }
        }
    }
}