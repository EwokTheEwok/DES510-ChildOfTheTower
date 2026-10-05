using UnityEngine;
using UnityEngine.InputSystem;

public class BrickAnimHandler : MonoBehaviour
{
    [SerializeField] private Animator _brickAnimator;

    public BrickBehaviorTest BrickBehavior;

    public void AnimationHandler()
    {

        _brickAnimator = BrickBehavior._currentBrick.GetComponentInChildren<Animator>();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("YPos");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }

        else if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("YNeg");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }

        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("XNeg");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }

        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("XPos");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }

        else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("ZPos");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }

        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            _brickAnimator.SetTrigger("ZNeg");
            _brickAnimator.Update(0f);
            _brickAnimator.SetTrigger("Default");
        }
    }
}
