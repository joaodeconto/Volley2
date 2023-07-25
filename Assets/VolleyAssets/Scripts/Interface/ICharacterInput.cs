using UnityEngine;
public interface ICharacterInput
{
    Vector2 GetMoveInput();
    bool GetJumpInput();
    float GetStrikeInput();
}