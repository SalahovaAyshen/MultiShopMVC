using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key, s=>s.Value);
            
            return settings;
        }
        public async Task<List<Category>> GetCategory()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }
    }
}
