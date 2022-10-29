using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Data;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepositroy : IOwnerRepository
    {
        private readonly DataContext _context;
        public OwnerRepositroy(DataContext context)
        {
            _context = context; 
        }

        public Owner GetOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }
        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return _context.PokemonOwners.Where(p => p.Pokemon.Id == pokeId).Select(o => o.Owner).ToList();
        }
        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }
        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(p => p.OwnerId == ownerId).Select(p => p.Pokemon).ToList();
        }
        public bool OwnerExists(int ownerId)
        {
            return _context.PokemonOwners.Any(p => p.OwnerId == ownerId);
        }
    }
}