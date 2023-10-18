using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaqing : Characters
{
    private enum ElementalSKill
    {
        THROW,
        SLASH
    }

    private ElementalOrb elementalOrb;
    [SerializeField] GameObject ElementalOrbPrefab;
    ElementalSKill elementalSKill = ElementalSKill.THROW;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (elementalOrb == null && elementalSKill == ElementalSKill.SLASH)
        {
            elementalSKill = ElementalSKill.THROW;
        }
    }

    protected override void ElementalSkillHold()
    {
        switch (elementalSKill)
        {
            case ElementalSKill.THROW:
                UpdateCameraAim();
                break;
        }

    }

    protected override void ElementalBurstTrigger()
    {
    }

    protected override void ElementalSkillTrigger()
    {
        switch(elementalSKill)
        {
            case ElementalSKill.THROW:
                ElementalOrb Orb = Instantiate(ElementalOrbPrefab).GetComponent<ElementalOrb>();
                elementalOrb = Orb;
                StartCoroutine(elementalOrb.MoveToTargetLocation(GetRayPosition3D(transform.position, GetVirtualCamera().transform.forward, 10f), 1000f));

                elementalSKill = ElementalSKill.SLASH;
                break;
            case ElementalSKill.SLASH:
                transform.position = elementalOrb.transform.position;
                Destroy(elementalOrb.gameObject);

                elementalSKill = ElementalSKill.THROW;
                break;
        }
    }


}
