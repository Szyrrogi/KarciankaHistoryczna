using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardEngine.Enums;
using CardEngine;
public class HandCard : MonoBehaviour
{

    private Card _card;
    public void Init(Card data)
    {
        _card = data;
    }
    protected virtual void Start()
    {
    }
    protected void Update()
    {
        
    }

    protected virtual void AfterPut()
    {

    }


}
