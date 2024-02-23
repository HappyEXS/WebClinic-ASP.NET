using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using przychodnia.Models;

namespace przychodnia.Controllers
{
    public class SpecialityController : Controller
    {
        private readonly StorageContext storageContext;

        public SpecialityController(StorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public async Task<IActionResult> Index()
        {
            var specialities = await storageContext.Specialities.ToListAsync();
            return View(specialities);
        }
    }
}
