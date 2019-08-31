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
        private IEnumerable<CrewListItem> _crewItemContainers;
        private bool _astronautComplexSpawned;
        private StringBuilder _label = new StringBuilder();
        private ActiveFlightTracker _tracker;
        private bool _foundDeadKerbal;

   private void Start()
        {
            GameEvents.onGUIAstronautComplexSpawn.Add(AstronautComplexSpawned);
            GameEvents.onGUIAstronautComplexDespawn.Add(AstronautComplexDespawned);
            _tracker = ActiveFlightTracker.instance;
            Debug.Log("[MemorialWall]: MemorialWall has registered events");
        }

        private void AstronautComplexDespawned()
        {
            _astronautComplexSpawned = false;
            _foundDeadKerbal = false;
            Debug.Log("[MemorialWall]: Astronaut Complex despawned");
        }

        private void AstronautComplexSpawned()
        {
            _astronautComplexSpawned = true;
            Debug.Log("[MemorialWall]: Astronaut Complex spawned");
        }

        private void LateUpdate()
        {
            if (!_astronautComplexSpawned || _foundDeadKerbal) return;
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            _crewItemContainers = FindObjectsOfType<CrewListItem>();
            for (int i = 0; i < _crewItemContainers.Count(); i++)
            {
                CrewListItem crewContainer = _crewItemContainers.ElementAt(i);
                ProtoCrewMember p = crewContainer.GetCrewRef();
                if (p.rosterStatus != ProtoCrewMember.RosterStatus.Dead) continue;
                crewContainer.SetName(p.name + " (Deceased)");
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                if (!_foundDeadKerbal) Debug.Log("[MemorialWall]: Overwriting AstronautComplexGUI");
                _foundDeadKerbal = true;
                string flights = _tracker.GetNumberOfFlights(p.name).ToString();
                string hours = _tracker.ConvertUtToString(_tracker.GetRecordedMissionTimeSeconds(p.name));
                string worldFirsts = _tracker.GetNumberOfWorldFirsts(p.name).ToString();
                _label.Length = 0;
                _label.Append("Flights: " + flights + " | Hours: " + hours + " | World Firsts: " + worldFirsts);
                crewContainer.SetLabel(_label.ToString());
            }
        }
    }
}
