using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanelController : MonoBehaviour
{
   public delegate void StartGameDelegate();

   public event StartGameDelegate OnStartButtonClick;

   public void OnClickStartButton()
   {
      OnStartButtonClick?.Invoke();
   }
}
