using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Data;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }
        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.Id == id).FirstOrDefault();
        }
        public ICollection<Pokemon> GetPokemonByCategory(int catgeoryId)
        {
            return _context.PokemonCategories.Where(e => e.CategoryId == catgeoryId).Select(c=>c.Pokemon).ToList();
        }
        public bool CategoryExists(int id)
        {
            return _context.Pokemon.Any(e => e.Id == id);
        }

    }
}