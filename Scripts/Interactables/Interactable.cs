using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool usedUp = false;
    protected bool isBeingInteracted = false;
    public abstract void Interact(HKUnit unit);
}
