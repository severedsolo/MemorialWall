using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI;
using FlightTracker;
using System.Reflection;

namespace MemorialBoard
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MemorialWall : MonoBehaviour
    {
        IEnumerable<CrewListItem> crewItemContainers;
        bool astronautComplexSpawned = false;
        StringBuilder label = new StringBuilder();
        ActiveFlightTracker tracker;
        bool foundDeadKerbal = false;

        void Start()
        {
            GameEvents.onGUIAstronautComplexSpawn.Add(AstronautComplexSpawned);
            GameEvents.onGUIAstronautComplexDespawn.Add(AstronautComplexDespawned);
            tracker = ActiveFlightTracker.instance;
            Debug.Log("[MemorialBoard]: MemorialBoard has registered events");
        }

        private void AstronautComplexDespawned()
        {
            astronautComplexSpawned = false;
            foundDeadKerbal = false;
            Debug.Log("[MemorialBoard]: Astronaut Complex despawned");
        }

        private void AstronautComplexSpawned()
        {
            astronautComplexSpawned = true;
            Debug.Log("[MemorialBoard]: Astronaut Complex spawned");
        }

        private void LateUpdate()
        {
            if (astronautComplexSpawned && !foundDeadKerbal)
            {
                crewItemContainers = FindObjectsOfType<CrewListItem>();
                CrewListItem crewContainer;
                for (int i = 0; i < crewItemContainers.Count(); i++)
                {
                    crewContainer = crewItemContainers.ElementAt(i);
                    ProtoCrewMember p = crewContainer.GetCrewRef();
                    if (p.rosterStatus != ProtoCrewMember.RosterStatus.Dead) continue;
                    crewContainer.SetName(p.name + " (Deceased)");
                    if (!foundDeadKerbal) Debug.Log("[MemorialWall]: Overwriting AstronautComplexGUI");
                    foundDeadKerbal = true;
                    string flights = tracker.GetNumberOfFlights(p.name).ToString();
                    string hours = tracker.ConvertUtToString(tracker.GetRecordedMissionTimeSeconds(p.name));
                    string worldFirsts = tracker.GetNumberOfWorldFirsts(p.name).ToString();
                    label.Length = 0;
                    label.Append("Flights: " + flights + " | Hours: " + hours + " | World Firsts: " + worldFirsts);
                    crewContainer.SetLabel(label.ToString());
                }
            }
        }
    }
}
