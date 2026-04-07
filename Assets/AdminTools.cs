using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminTools : MonoBehaviour
{
    [SerializeField] private Transform oasis1;
    [SerializeField] private Transform oasis2;
    [SerializeField] private Transform oasis3;
    [SerializeField] private Transform oasis4;
    [SerializeField] private Transform oasis5;

    private void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            gameObject.transform.position = oasis1.transform.position;
        }

        if (Input.GetKey(KeyCode.F2))
        {
            gameObject.transform.position = oasis2.transform.position;
        }

        if (Input.GetKey(KeyCode.F3))
        {
            gameObject.transform.position = oasis3.transform.position;
        }

        if (Input.GetKey(KeyCode.F4))
        {
            gameObject.transform.position = oasis4.transform.position;
        }

        if (Input.GetKey(KeyCode.F5))
        {
            gameObject.transform.position = oasis5.transform.position;
        }

        if (Input.GetKey(KeyCode.F8))
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}
