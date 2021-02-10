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
            GetList(property).DoList(rect);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetList(property).GetHeight();
        }

        private ReorderableList GetList(SerializedProperty property)
        {
            if (_list == null)
            {
                var serializedProperty = property.FindPropertyRelative("_abilities");
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
                _list.elementHeight = EditorGUIUtility.singleLineHeight;
            }

            return _list;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Abilities");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.PropertyField(
                rect, 
                _list.serializedProperty.GetArrayElementAtIndex(index)
            );
        }
    }
}