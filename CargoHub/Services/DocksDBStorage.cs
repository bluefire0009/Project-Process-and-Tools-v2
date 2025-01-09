using CargoHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    public class DocksDBStorage : IDocksStorage  
    {
        private readonly DatabaseContext db;

        public DocksDBStorage(DatabaseContext db)  
        {
            this.db = db;
        }

        public async Task<IEnumerable<Dock>> GetAllDocksAsync()
        {
            return await db.Docks
                .Include(d => d.Transfers)
                .Take(100) // Limit to 100 docks
                .ToListAsync();
        }

        public async Task<IEnumerable<Dock>> GetDocksWithPaginationAsync(int offset, int limit)
        {
            return await db.Docks
                .Include(d => d.Transfers)
                .Skip(offset) // Skip the first 'offset' items
                .Take(limit)  // Take the next 'limit' items
                .ToListAsync();
        }

        public async Task<Dock?> GetDockByIdAsync(int id)
        {
            return await db.Docks
                .Include(d => d.Transfers)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> CreateDockAsync(Dock dock)
        {
            var existingDock = await db.Docks.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dock.Id);
            if (existingDock != null)
            {
                return false;
            }

            await db.Docks.AddAsync(dock);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteDockAsync(int id)
        {
            var dock = await db.Docks.FirstOrDefaultAsync(d => d.Id == id);
            if (dock == null) return false;

            if (dock.isDeleted)
            {
                return false;
            }

            dock.isDeleted = true;
            await db.SaveChangesAsync();
            return true;
        }
    }
