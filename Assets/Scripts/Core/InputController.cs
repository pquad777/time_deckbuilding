using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public event Action<int> OnCardKeyPressed;
    public event Action OnCancelPressed;
    public bool Enabled { get; private set; }
    public int? ChosenIndex {get; private set;}
    public void Enable(bool value)
    {
        Enabled = value;
    }
    public void SetChoice(int idx) => ChosenIndex = idx;
    
    public void ClearChoice() => ChosenIndex = null;

    private void Update()
    {
        if (!Enabled) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnCancelPressed?.Invoke();
            Debug.Log($"Cancel selected card");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            OnCardKeyPressed?.Invoke(3);
            Debug.Log($"q pressed");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnCardKeyPressed?.Invoke(2);
            Debug.Log($"w pressed");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnCardKeyPressed?.Invoke(1);
            Debug.Log($"e pressed");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnCardKeyPressed?.Invoke(0);
            Debug.Log($"r pressed");
        }
    }
}
