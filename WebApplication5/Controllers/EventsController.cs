using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventContext db = new EventContext();

        [OutputCache(Duration = 30, VaryByParam = "*", VaryByCustom = "RequestCookies", Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            var events = db.Events.Include(item => item.Category)
                .Where(item => item.IsPublished)
                .OrderBy(item => item.StartDate)
                .ToList();

            ViewBag.Categories = db.Categories.OrderBy(category => category.Name).ToList();
            return View(events);
        }

        [OutputCache(Duration = 30, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public ActionResult Filter(int? categoryId, decimal? minPrice, decimal? maxPrice, string search)
        {
            var query = db.Events.Include(item => item.Category).Where(item => item.IsPublished);

            if (categoryId.HasValue)
                query = query.Where(item => item.CategoryId == categoryId.Value);
            if (minPrice.HasValue)
                query = query.Where(item => item.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(item => item.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(item => item.Title.Contains(search));

            return PartialView("_EventList", query.OrderBy(item => item.StartDate).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var item = db.Events.Include(eventItem => eventItem.Category).SingleOrDefault(eventItem => eventItem.Id == id.Value);
            if (item == null || (!item.IsPublished && !Request.IsAuthenticated))
                return HttpNotFound();

            return View(item);
        }

        public ActionResult CheckSeats(int id)
        {
            var item = db.Events.Where(eventItem => eventItem.Id == id && eventItem.IsPublished)
                .Select(eventItem => new { eventItem.Id, eventItem.Title, FreeSeats = eventItem.Seats })
                .SingleOrDefault();

            if (item == null)
                return HttpNotFound();

            return Json(new
            {
                item.Id,
                item.Title,
                item.FreeSeats,
                Status = item.FreeSeats > 0 ? "места есть" : "мест нет"
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Create()
        {
            LoadCategories();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Create([Bind(Include = "Title,Description,StartDate,Price,Seats,IsPublished,CategoryId")] Event item)
        {
            ValidateCategory(item.CategoryId);
            if (ModelState.IsValid)
            {
                db.Events.Add(item);
                db.SaveChanges();
                InvalidatePublicCache();
                return RedirectToAction("Details", new { id = item.Id });
            }

            LoadCategories(item.CategoryId);
            return View(item);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var item = db.Events.Find(id.Value);
            if (item == null)
                return HttpNotFound();

            LoadCategories(item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,StartDate,Price,Seats,IsPublished,CategoryId")] Event item)
        {
            ValidateCategory(item.CategoryId);
            if (!db.Events.Any(eventItem => eventItem.Id == item.Id))
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                InvalidatePublicCache();
                return RedirectToAction("Details", new { id = item.Id });
            }

            LoadCategories(item.CategoryId);
            return View(item);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var item = db.Events.Include(eventItem => eventItem.Category).SingleOrDefault(eventItem => eventItem.Id == id.Value);
            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = db.Events.Find(id);
            if (item == null)
                return HttpNotFound();

            db.Events.Remove(item);
            db.SaveChanges();
            InvalidatePublicCache();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult TogglePublished(int id)
        {
            var item = db.Events.Find(id);
            if (item == null)
                return HttpNotFound();

            item.IsPublished = !item.IsPublished;
            db.SaveChanges();
            InvalidatePublicCache();
            return Json(new { success = true, isPublished = item.IsPublished });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        private void LoadCategories(int? selectedId = null)
        {
            ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(category => category.Name), "Id", "Name", selectedId);
        }

        private void ValidateCategory(int categoryId)
        {
            if (!db.Categories.Any(category => category.Id == categoryId))
                ModelState.AddModelError("CategoryId", "Выберите существующую категорию.");
        }

        private void InvalidatePublicCache()
        {
            System.Web.HttpResponse.RemoveOutputCacheItem(Url.Action("Index", "Events"));
            System.Web.HttpResponse.RemoveOutputCacheItem(Url.Action("Filter", "Events"));
        }
    }
}