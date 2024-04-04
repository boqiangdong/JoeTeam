using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventmanagement
{
    private static Eventmanagement instance;
    public static Eventmanagement Instance
    {
        get
        {
            if (null == instance)
                instance = new Eventmanagement();
            return instance;
        }
    }

    public float number;
}
