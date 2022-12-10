using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthUI : MonoBehaviour
{

    public GameObject loginUI;
    public GameObject registerUI;

    public static AuthUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != null)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    public void LoginScreen()
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }

    public void RegisterScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

}
