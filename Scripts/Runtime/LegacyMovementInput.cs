using ModularFirstPerson;
using UnityEngine;

public class LegacyMovementInput : MovementInput
{
    public override Vector2 GetMovementInput()
    {
        var moveInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        if (moveInput.sqrMagnitude > 1)
        {
            moveInput.Normalize();
        }
        return moveInput;
    }
}
