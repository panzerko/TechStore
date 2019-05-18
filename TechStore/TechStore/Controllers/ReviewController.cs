using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.DAL.Classes.UnitOfWork;
using TechStore.DAL.Models;

namespace TechStore.Controllers
{
    [Authorize(Roles = "admin, moderator")]
    public class ReviewController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public ReviewController(AppDbContext context)
        {
            this.unitOfWork = new UnitOfWork(context);
        }

        public IActionResult Index()
        {
            return View(unitOfWork.Reviews.GetAll().Where(r => r.IsVisibleForAll == false).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            GoodReview review = await unitOfWork.Reviews.Get(id);

            if (review != null)
            {
                await unitOfWork.Reviews.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            GoodReview review = await unitOfWork.Reviews.Get(id);

            if (review != null)
            {
                review.IsVisibleForAll = true;

                unitOfWork.Reviews.Update(review);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
