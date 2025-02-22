using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }

    [SerializeField]
    private List<Character> selectChar = new List<Character>();
    public List<Character> SelectChar { get { return selectChar; } }

    public static PartyManager instacnce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instacnce = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
