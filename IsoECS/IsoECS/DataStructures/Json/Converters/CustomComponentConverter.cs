using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TecsDotNet.Json.Converters;
using TecsDotNet;
using Newtonsoft.Json.Linq;
using IsoECS.Components.GamePlay;
using Newtonsoft.Json;
using IsoECS.Components;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures.Json.Converters
{
    public class CustomComponentConverter : ComponentConverter
    {
        protected override Component Create(Type objectType, JObject jObject)
        {
            string type = (string)jObject["Type"];
            Component c = new Component();

            switch (type)
            {
                case "BuildableComponent":
                    return new BuildableComponent();

                case "CitizenComponent":
                    return new CitizenComponent();

                case "CityInformationComponent":
                    return new CityInformationComponent();

                case "CityServicesComponent":
                    return new CityServicesComponent();

                case "CollisionComponent":
                    return new CollisionComponent();

                case "CollisionMapComponent":
                    return new CollisionMapComponent();

                case "DrawableComponent":
                    return new DrawableComponent();
                    /*
                    

                    c = drawable;
                    break;
                     */
                    
                case "FoundationComponent":
                    return new FoundationComponent();
                    /*
                    FoundationComponent floor = JsonConvert.DeserializeObject<FoundationComponent>(jObject.ToString());
                    
                    switch (floor.PlanType)
                    {
                        case "Normal":
                            // nothing
                            break;
                        case "Fill":
                            Point start = floor.Plan[0].Offset;
                            Point end = floor.Plan[1].Offset;

                            floor.Plan.Clear(); // clear the plan, the for loops will fill it
                            for (int xx = start.X; xx <= end.X; xx++)
                            {
                                for (int yy = start.Y; yy <= end.Y; yy++)
                                {
                                    //floor.Plan.Add(new LocationValue() { Offset = new Point(xx, yy) });
                                }
                            }

                            break;
                    } 
                    c = floor;
                    break;
                    */
                case "FoundationPlannerComponent":
                    return new FoundationPlannerComponent();

                case "GameDateComponent":
                    return new GameDateComponent();

                case "HousingComponent":
                    return new HousingComponent();

                case "Inventory":
                    return new Inventory();

                case "IsometricMapComponent":
                    return new IsometricMapComponent();

                case "ProductionComponent":
                    return new ProductionComponent();

                case "PositionComponent":
                    return new PositionComponent();

                case "RoadComponent":
                    return new RoadComponent();

                case "RoadPlannerComponent":
                    return new RoadPlannerComponent();

                case "SpawnerComponent":
                    return new SpawnerComponent();

                case "StockpileComponent":
                    return new StockpileComponent();
            }

            return c;
        }
    }
}
