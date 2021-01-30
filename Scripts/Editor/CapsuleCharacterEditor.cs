using UnityEngine;
using UnityEditor;

namespace ModularFirstPerson
{
    // This editor exists solely to provide the frame bounds so that pressing F
    // in the viewport correctly frames the character.
    [CustomEditor(typeof(CapsuleBody)), CanEditMultipleObjects]
    public class CapsuleCharacterEditor : Editor
    {
        private bool HasFrameBounds() => true;

        private Bounds OnGetFrameBounds()
        {
            var bounds = (targets[0] as CapsuleBody).bounds;
            for (int i = 1; i < targets.Length; i++)
            {
                bounds.Encapsulate((targets[i] as CapsuleBody).bounds);
            }
            return bounds;
        }
    }
}
