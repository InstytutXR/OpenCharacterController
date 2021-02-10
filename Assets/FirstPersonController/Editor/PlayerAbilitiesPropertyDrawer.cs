using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FirstPersonController
{
    [CustomPropertyDrawer(typeof(PlayerAbilities))]
    public class PlayerAbilitiesPropertyDrawer : PropertyDrawer
    {
        private ReorderableList _list;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) =>
            GetList(property).DoList(rect);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            GetList(property).GetHeight();

        private ReorderableList GetList(SerializedProperty property) =>
            _list ?? (_list = CreateList(property));

        private static ReorderableList CreateList(SerializedProperty property)
        {
            var abilitiesProp = property.FindPropertyRelative("_abilities");
            var list = new ReorderableList(
                abilitiesProp.serializedObject,
                abilitiesProp,
                true,
                true,
                true,
                true
            );

            list.drawHeaderCallback += rect =>
                EditorGUI.LabelField(rect, "Abilities");

            var spacing = EditorGUIUtility.standardVerticalSpacing;
            
            list.drawElementCallback += (rect, index, active, focused) =>
                EditorGUI.PropertyField(
                    new Rect(
                        rect.x,
                        rect.y + spacing / 2f,
                        rect.width,
                        rect.height - spacing
                    ),
                    list.serializedProperty.GetArrayElementAtIndex(index),
                    GUIContent.none
                );

            list.elementHeightCallback += index =>
                EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) +
                spacing;

            return list;
        }
    }
}