using UnityEngine;

public class PlayerInput : MonoBehaviour 
{

    public float lookSensitivity = 1f;


    public bool Sprint { get; private set; }
    public bool Jump { get; set; }
    public bool Attack { get; set; }
    public bool ShieldHeld { get; private set; }
    public bool Interact { get; private set; }

    public float LookHorizontal
    {
        get { return GetLookAxis("Mouse X"); }
    }
    
    public float LookVertical
    {
        get { return GetLookAxis("Mouse Y"); }
    }

    public Vector3 Movement
    {
        get 
        {
            var movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            movement = Vector3.ClampMagnitude(movement, 1);
            return movement;
        }
    }

    private void Update() 
    {
        if (Input.GetButtonDown("Jump"))
            Jump = true;
        
        Sprint = Input.GetButton("Sprint");

        if (Input.GetButtonDown("Fire1"))
        {
            Attack = true;
        }

        ShieldHeld = Input.GetButton("Fire2");

        if (Input.GetButtonDown("Interact"))
            Interact = true;
    }

    float GetLookAxis(string axisName)
    {
        float value = Input.GetAxis(axisName);
        value *= lookSensitivity;
        value *= 0.01f;
        return value;
    }

}