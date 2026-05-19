using TMPro;
using UnityEngine;

public class PauseMenuView : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject resumeButtonObject;
    public GameObject restartButtonObject;
    public GameObject exitButtonObject;

    [Header("Texts")]
    public TMP_Text exitButtonText;
    public string exitToMenuText = "В главное меню";
    public string quitGameText = "Выйти из игры";
}
