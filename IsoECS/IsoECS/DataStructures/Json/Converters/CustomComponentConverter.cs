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

                case "CollisionComponent":
                    return new CollisionComponent();

                case "CollisionMapComponent":
                    return new CollisionMapComponent();

                case "DrawableComponent":
                    return new DrawableComponent();
                    
                case "FoundationComponent":
                    return new FoundationComponent();
                    
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
