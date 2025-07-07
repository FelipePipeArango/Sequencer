using UnityEngine;
using TMPro;

public class PickUpNumber : MonoBehaviour
{
    [Header("DONT TOUCH")]
    [SerializeField] TextMeshProUGUI m_TextMeshPro;

    [Header("MUST ASSIGN IN SCENE")]
    [SerializeField] int assignedNumberValue;
    [SerializeField] NumberItem assignedNumberHUD;

    void Start()
    {
        m_TextMeshPro.text = assignedNumberValue.ToString();
        assignedNumberHUD.value = assignedNumberValue;
    }

    public void PickedUp()
    {
        assignedNumberHUD.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
