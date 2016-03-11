// <copyright file="Fruit.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

using System;
using System.Drawing;
using System.IO;
using Westwind.Utilities.Dynamic;

namespace ScrapProject
{
    /// <summary>A fruit class that derives from the expando object</summary>
    public class Fruit : Expando
    {
        /// <summary>Gets or sets the color of the fruit</summary>
        public string Color { get; set; }

        /// <summary>Gets or sets the name of the fruit</summary>
        public string Name { get; set; }
    }
}