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
            c.charInit(VFXManager.instance, UIManager.instance);
        }

        SelectSingleHero(0);

        members[0].MagicSkills.Add(new Magic(0, "Power Glow", 10f, 20, 3f, 1f, 1, 2));
        members[1].MagicSkills.Add(new Magic(0, "Fire Ball", 10f, 35, 3f, 4f, 0, 1));

        UIManager.instance.ShowMagicToggles();
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

    public void SelectSingleHero(int i)
    {
        foreach (Character c in selectChar)
            c.ToggleRingSelection(false);

        selectChar.Clear();

        selectChar.Add(members[i]);
        selectChar[0].ToggleRingSelection(true);
    }

    public void HeroSelectMagicSkills(int i)
    {
        if (selectChar.Count <= 0)
            return;

        selectChar[0].IsMagicMode = true;
        SelectChar[0].CurMagicCast = selectChar[0].MagicSkills[i];
    }
}
