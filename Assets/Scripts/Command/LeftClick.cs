using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeftClick : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private RectTransform boxSelection;
    private Vector2 oldAnchoredPos;
    private Vector2 startPos;

    public static LeftClick instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");

        boxSelection = UIManager.instance.SelectionBox;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ClearEveryThing();
        }

        if (Input.GetMouseButton(0))
        {

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            UpdateSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox(Input.mousePosition);
            TrySelect(Input.mousePosition);
        }
    }

    private void SelectCharacter(RaycastHit hit)
    {
        Character hero = hit.collider.GetComponent<Character>();
        Debug.Log("Selected Char: " + hit.collider.gameObject);

        PartyManager.instance.SelectChar.Add(hero);
        hero.ToggleRingSelection(true);
        UIManager.instance.ShowMagicToggles();
        
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    SelectCharacter(hit);
                    break;
            }
        }
    }

    private void ClearRingSelection()
    {
        foreach (Character h in PartyManager.instance.SelectChar)
            h.ToggleRingSelection(false);
    }

    private void ClearEveryThing()
    {
        ClearRingSelection();
        PartyManager.instance.SelectChar.Clear();
    }

    private void UpdateSelectionBox(Vector3 mousePos)
    {
        if (!boxSelection.gameObject.activeInHierarchy)
            boxSelection.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        boxSelection.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

        width = Mathf.Abs(width);
        height = Mathf.Abs(height);

        boxSelection.sizeDelta = new Vector2(width, height);

        oldAnchoredPos = boxSelection.anchoredPosition;
    }

    private void ReleaseSelectionBox (Vector2 mousePos)
    {
        Vector2 corner1;
        Vector2 corner2;

        boxSelection.gameObject.SetActive(false);

        corner1 = oldAnchoredPos - (boxSelection.sizeDelta / 2);
        corner2 = oldAnchoredPos + (boxSelection.sizeDelta / 2);

        foreach (Character member in PartyManager.instance.Members)
        {
            Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            if ((unitPos.x > corner1.x && unitPos.x < corner2.x) && (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                PartyManager.instance.SelectChar.Add(member);
                member.ToggleRingSelection(true);
            }

        }
        boxSelection.sizeDelta = new Vector2(0, 0);

    }

}
