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

    public static PartyManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        foreach (Character c in members)
        {
            c.charInit(VFXManager.instance);
            c.MagicSkills.Add(new Magic(0, "Fireball", 10f, 30, 3f, 1f, 0, 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChar.Count > 0)
            {
                selectChar[0].IsMagicMode = true;
                selectChar[0].CurMagicCast = selectChar[0].MagicSkills[0];
            }
        }
    }
}
