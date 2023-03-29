using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public abstract class Singletone<T> : MonoBehaviour where T : Component
{
    #region Fields

    private static T instance;

    #endregion

    #region Properties

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    return null;
                }
            }
            return instance;
        }
    }

    #endregion

    #region Methods

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
