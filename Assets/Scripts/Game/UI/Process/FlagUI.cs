using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagUI : MonoBehaviour
{
    public void RiseUp()
    {
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 22f, 1f);
    }

    public void RiseDown()
    {
        LeanTween.moveLocalY(gameObject, transform.localPosition.y - 22f, 1f);
    }
}
