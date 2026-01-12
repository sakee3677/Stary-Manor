using UnityEngine;

public class AnimationEventBridge : MonoBehaviour
{
    public void TriggerAbsorbEvent()//這是一個讓animationEvent可以從子物件觸發到父物件程式的功能
    {
        // 嘗試找到父物件上的吞食腳本
        var parasiteController = GetComponentInParent<Slimee>();
        if (parasiteController != null)
        {
          
            parasiteController.OnAbsorbAnimationEvent(); // 呼叫父物件函數
        }
    }
    public void stopeating()//這是一個讓animationEvent可以從子物件觸發到父物件程式的功能
    {
        // 嘗試找到父物件上的吞食腳本
        var parasiteController = GetComponentInParent<Slimee>();
        if (parasiteController != null)
        {
           
            parasiteController.stopEatingAnimation(); // 呼叫父物件函數
        }
    }
    public void ParaAnimationSlow()
    {
        var playereController = GetComponentInParent<PlayerController>();
        if (playereController != null)
        {
            playereController.ParanimationSlw();
        }
    }
    public void ParaAnimationEnd()
    {
        var playereController = GetComponentInParent<PlayerController>();
        if(playereController != null)
        {
            playereController.ParanimationEnd();
        }
    }
}
