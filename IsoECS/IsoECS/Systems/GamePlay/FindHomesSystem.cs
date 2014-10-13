using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;

namespace IsoECS.Systems.GamePlay
{
    public class FindHomesSystem : ISystem
    {
        private int _updateRate = 2500;
        private int _updateCountdown;

        public void Update(List<Entity> entities, int dt)
        {
            // do this bc this system only needs to run every few seconds not each frame
            _updateCountdown -= dt;
            if (_updateCountdown <= 0)
            {
                _updateCountdown += _updateRate;

                List<Entity> citizens = entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });
                List<Entity> houses = entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); });

                // get all homes with vacanies
                List<Entity> vacantHomes = new List<Entity>();

                foreach (Entity housingEntity in houses)
                {
                    HousingComponent house = housingEntity.Get<HousingComponent>();

                    if (house.Tennants.Count < house.MaxOccupants)
                        vacantHomes.Add(housingEntity);
                }

                if (vacantHomes.Count == 0)
                {
                    return;
                }

                foreach (Entity citizenEntity in citizens)
                {
                    CitizenComponent citizen = citizenEntity.Get<CitizenComponent>();

                    // check if the citizen is homeless
                    if (citizen.HousingID == -1)
                    {
                        // if they are attempt to find a living place
                        // TODO: check for appropriate level of housing (rich, middle class, poor)
                        foreach (Entity potentialHome in vacantHomes)
                        {
                            HousingComponent home = potentialHome.Get<HousingComponent>();

                            if (home.Rent <= citizen.Money)
                            {
                                citizen.HousingID = (int)potentialHome.ID;
                                home.Tennants.Add((int)citizenEntity.ID);

                                Console.WriteLine(String.Format("{0} has moved into Entity#{1}", citizen.Name, potentialHome.ID));
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Init(List<Entity> entities)
        {
            _updateCountdown = _updateRate;
        }

        public void Shutdown(List<Entity> entities)
        {
        }
    }
}
