using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WelcomeTip : MonoBehaviour
{
    [SerializeField] private List<string> list = new List<string>();

    private void Awake()
    {
        int index = Random.Range(0, list.Count);
        GetComponent<TextMeshProUGUI>().text = "Tip: "+list[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
