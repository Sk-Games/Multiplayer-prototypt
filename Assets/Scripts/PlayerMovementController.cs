using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField touchField;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbodyFirstPersonController.joystickInputAxis.x = joystick.Horizontal;
        rigidbodyFirstPersonController.joystickInputAxis.y = joystick.Vertical;
        rigidbodyFirstPersonController.mouseLook.lookInputAxis = touchField.TouchDist;
    }
}
