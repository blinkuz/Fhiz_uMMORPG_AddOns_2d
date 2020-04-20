﻿// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// =======================================================================================
// PLAYER
// =======================================================================================
public partial class Player
{
    protected int tabTargetIndex = 0;
    public float tabTargetMultiplier = 6;

    // -----------------------------------------------------------------------------------
    // TargetNearest
    // -----------------------------------------------------------------------------------
    [Client]
    private void TargetNearest()
    {
        if (Input.GetKeyDown(targetNearestKey)
        //(MobileControls)
#if _iMMOMOBILECONTROLS
		|| targetButtonPressed) {
		targetButtonPressed = false;
#else
        )
        {
#endif
            List<Entity> correctedTargets = new List<Entity>();

            int layerMask = ~(1 << 2);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRange * tabTargetMultiplier, layerMask);

            foreach (Collider2D hitCollider in hitColliders)
            {
                Entity target = hitCollider.GetComponentInParent<Entity>();

                if (target != null && target != this && CanAttack(target) && target.isAlive && !correctedTargets.Any(x => x == target))
                    correctedTargets.Add(target);
            }

            List<Entity> sortedTargets = correctedTargets.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();

            if (sortedTargets.Count > 0)
            {
                tabTargetIndex++;

                if (tabTargetIndex >= sortedTargets.Count)
                    tabTargetIndex = 0;

                SetIndicatorViaParent(sortedTargets[tabTargetIndex].transform);
                CmdSetTarget(sortedTargets[tabTargetIndex].netIdentity);
                sortedTargets.Clear();
            }
            else
            {
                tabTargetIndex = 0;
            }

            correctedTargets.Clear();
        }
    }

    // -----------------------------------------------------------------------------------
}