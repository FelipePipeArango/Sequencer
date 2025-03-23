using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbleConnecting : MonoBehaviour
{
    [SerializeField] GameObject cardHolder;

    GameObject[] cables;
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

            Vector2 newPos = cables[slotDistance - 1].GetComponent<RectTransform>().anchoredPosition;
            cables[slotDistance - 1].GetComponent<RectTransform>().anchoredPosition = new Vector2(UIdistance + 30f, newPos.y);
            cables[slotDistance - 1].SetActive(true);
        }
    }

    public void EndCable()
    {
        cables[slotDistance - 1].SetActive(false);
    }
}
