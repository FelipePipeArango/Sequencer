using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbleConnecting : MonoBehaviour
{
    [SerializeField] GameObject cardHolder;

    GameObject[] cables;
    GameObject selectedCable;
    int slotDistance;

    private void Start()
    {
        cables = new GameObject[this.transform.childCount];

        for (int i = 0; i < cables.Length; i++)
        {
            cables[i] = this.transform.GetChild(i).gameObject;
        }
    }

    public void StartCable(int currentSlot, int pointedSlot)
    {
        Vector3 initialPosition;
        Vector3 pointedPosition;
        slotDistance = Mathf.Abs(pointedSlot - currentSlot);

        if (slotDistance > 0)
        {
            initialPosition = cardHolder.transform.GetChild(currentSlot - 1).GetComponent<RectTransform>().localPosition;
            pointedPosition = cardHolder.transform.GetChild(pointedSlot - 1).GetComponent<RectTransform>().localPosition;
            float UIdistance = (pointedPosition.x + initialPosition.x)/2;

            selectedCable = cables[slotDistance - 1];

            Vector2 newPos = selectedCable.GetComponent<RectTransform>().anchoredPosition;
            selectedCable.GetComponent<RectTransform>().anchoredPosition = new Vector2(UIdistance + 30f, newPos.y);
            selectedCable.SetActive(true);
            if (pointedSlot - currentSlot > 0)
            {
                selectedCable.transform.GetChild(0).GetComponent<Animator>().SetBool("StartsLeft", false);
            }
            else
            {
                selectedCable.transform.GetChild(0).GetComponent<Animator>().SetBool("StartsLeft", true);
            }
        }
    }

    public void EndCable()
    {
        if (slotDistance > 0)
        {
            selectedCable.transform.GetChild(0).GetComponent<Animator>().SetBool("HasEnded", true);
            cables[slotDistance - 1].SetActive(false); 
        }
    }
}
