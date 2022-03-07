using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanel : MonoBehaviour
{
    public List<GameObject> panels;

    public void ShowPanel(int index)
    {
        panels.ForEach(x => x.SetActive(false));
        panels[index].SetActive(true);
    }
}
