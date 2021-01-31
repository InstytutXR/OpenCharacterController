#if ENABLE_INPUT_SYSTEM

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

namespace ModularFirstPerson
{
    [CustomPropertyDrawer(typeof(PlayerInputActionReference))]
    public class PlayerInputActionReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var obj = property.serializedObject.targetObject as MonoBehaviour;
            var playerInput = obj.GetComponentInParent<PlayerInput>();
            var actions = playerInput.actions;

            var options = actions.actionMaps.SelectMany(map => 
                map.actions.Select(action => new { name = $"{map.name}/{action.name}", action.id })
            ).ToList();

            var optionNames = options.Select(o => o.name).ToArray();

            var guidAsStringProp = property.FindPropertyRelative("guidAsString");

            int currentIndex = -1;
            if (Guid.TryParse(guidAsStringProp.stringValue, out var guid))
            {
                currentIndex = options.FindIndex(o => o.id == guid);
            }

            var newIndex = EditorGUI.Popup(position, currentIndex, optionNames);

            if (newIndex != currentIndex)
            {
                guidAsStringProp.stringValue = options[newIndex].id.ToString();
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif // #if ENABLE_INPUT_SYSTEM
