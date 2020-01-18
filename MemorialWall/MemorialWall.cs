using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlightTracker;
using KSP.UI;
using UnityEngine;

namespace MemorialWall
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class MemorialWall : MonoBehaviour
    {
        private IEnumerable<CrewListItem> crewItemContainers;
        private bool astronautComplexSpawned;
        private readonly StringBuilder label = new StringBuilder();
        private FlightTrackerApi tracker;
        private bool foundDeadKerbal;

   private void Start()
        {
            GameEvents.onGUIAstronautComplexSpawn.Add(AstronautComplexSpawned);
            GameEvents.onGUIAstronautComplexDespawn.Add(AstronautComplexDespawned);
            tracker = FlightTrackerApi.Instance;
            Debug.Log("[MemorialWall]: MemorialWall has registered events");
        }

        private void AstronautComplexDespawned()
        {
            astronautComplexSpawned = false;
            foundDeadKerbal = false;
            Debug.Log("[MemorialWall]: Astronaut Complex despawned");
        }

        private void AstronautComplexSpawned()
        {
            astronautComplexSpawned = true;
            Debug.Log("[MemorialWall]: Astronaut Complex spawned");
        }

        private void LateUpdate()
        {
            if (!astronautComplexSpawned || foundDeadKerbal) return;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            crewItemContainers = FindObjectsOfType<CrewListItem>();
            for (int i = 0; i < crewItemContainers.Count(); i++)
            {
                CrewListItem crewContainer = crewItemContainers.ElementAt(i);
                ProtoCrewMember p = crewContainer.GetCrewRef();
                if (p.rosterStatus != ProtoCrewMember.RosterStatus.Dead) continue;
                crewContainer.SetName(p.name + " (Deceased)");
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
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
