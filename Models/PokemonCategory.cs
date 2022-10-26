using System;
namespace PokemonReviewApp.Models
{
    public class PokemonCategory
    {
        public int Id {get;set;}
        public string Name { get; set;}
        public DateTime BirthDate {get; set;}
        public Pokemon Pokemon { get; set; }
        public Category Category { get; set; }
    }
}