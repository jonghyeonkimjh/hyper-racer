using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public delegate void MoveButtonDelegate();
    public event MoveButtonDelegate OnMoveButtonDown;
    private bool _isDown = false;
    

    private void Update()
    {
        // Button은 단순히 눌리냐 안눌리냐 같은 단순한 기능 만 넣을것 그 이상은 넣치 말것
        if (_isDown)
        {
            OnMoveButtonDown?.Invoke();
        }
    }

    public void ButtonDown()
    {
        _isDown = true;
    }

    public void ButtonUp()
    {
        _isDown = false;
    }
}
