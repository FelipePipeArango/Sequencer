using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbleConnecting : MonoBehaviour
{
    [SerializeField] GameObject cardHolder;

    GameObject[] cables;
    GameObject selectedCable;
    int absoluteSlotDistance;
    int slotDistance;
    RectTransform cableTransform;

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
        absoluteSlotDistance = Mathf.Abs(pointedSlot - currentSlot);
        slotDistance = pointedSlot - currentSlot;

        if (absoluteSlotDistance > 0)
        {
            initialPosition = cardHolder.transform.GetChild(currentSlot - 1).GetComponent<RectTransform>().localPosition;
            pointedPosition = cardHolder.transform.GetChild(pointedSlot - 1).GetComponent<RectTransform>().localPosition;
            float UIdistance = (pointedPosition.x + initialPosition.x)/2;

            selectedCable = cables[absoluteSlotDistance - 1];
            cableTransform = selectedCable.GetComponent<RectTransform>();
            selectedCable.transform.GetComponent<Animator>().SetBool("HasEnded", false);
            selectedCable.transform.GetComponent<Animator>().SetBool("Dropped", false);
            cableTransform.anchoredPosition = new Vector2(UIdistance + 30f, cableTransform.anchoredPosition.y);

            if (slotDistance > 0)
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartRight", true);
            }
            else
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartLeft", true);
            }
        }
    }

    public void CancelCable(bool hasDropped)
    {
        if (absoluteSlotDistance > 0 && hasDropped == false)
        {
            selectedCable.transform.GetComponent<Animator>().SetBool("HasEnded", true);

            if (slotDistance > 0)
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartRight", false);
            }
            else
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartLeft", false);
            }
        }
    }

    public void EndCable(bool hasDropped)
    {
        if (absoluteSlotDistance > 0 && hasDropped == true)
        {
            selectedCable.transform.GetComponent<Animator>().SetBool("Dropped", true);

            if (slotDistance > 0)
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartRight", false);
            }
            else
            {
                selectedCable.transform.GetComponent<Animator>().SetBool("StartLeft", false);
            }
        }
    }
}
