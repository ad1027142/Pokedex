using System;

namespace Pokedex
{
    public class Pokemon
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Types { get; set; } = string.Empty;
    }
}
