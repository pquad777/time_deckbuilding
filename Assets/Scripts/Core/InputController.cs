using UnityEngine;

public class InputController : MonoBehaviour
{   
    public bool Enabled { get; private set; }
    public int? ChosenIndex {get; private set;}
    public void Enable(bool value)
    {
        Enabled = value;
    }
    
    public void ClearChoice() => ChosenIndex = null;

    private void Update()
    {
        if (!Enabled) return;
        if (Input.GetKeyDown(KeyCode.Space)) {ChosenIndex = null; return;}

        if (Input.GetKeyDown(KeyCode.Q)) ChosenIndex = 0;
        if (Input.GetKeyDown(KeyCode.W)) ChosenIndex = 1;
        if (Input.GetKeyDown(KeyCode.E)) ChosenIndex = 2;
        if (Input.GetKeyDown(KeyCode.R)) ChosenIndex = 3;
    }
}
