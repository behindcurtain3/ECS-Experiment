﻿using System;
using System.Collections.Generic;
using IsoECS.DataStructures.GamePlay;
using TecsDotNet;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Recipe : Prototype
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<RecipeStage> Stages { get; set; }

        public Recipe()
        {
            Stages = new List<RecipeStage>();
        }
    }

    [Serializable]
    public class RecipeStage
    {
        public List<RecipeInput> Inputs { get; set; }
        public List<RecipeOutput> Outputs { get; set; }
        public List<string> AddToDrawableComponent { get; set; }
        public List<string> RemoveFromDrawableComponent { get; set; }

        // a measure of time required in in-game minutes to complete this stage of the recipe
        // minutesPassed * (numEmployees / maxEmployees) = work done
        // so a workshop with 3 out of 4 employees would work at 75% efficiency
        public long WorkRequired { get; set; }

        public RecipeStage()
        {
            Inputs = new List<RecipeInput>();
            Outputs = new List<RecipeOutput>();
            AddToDrawableComponent = new List<string>();
            RemoveFromDrawableComponent = new List<string>();
        }
    }
}
