using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EgyptMenu.Models;

namespace EgyptMenu.Controllers
{
    public class restorantsController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/restorants
        public List<RestaurantDto> Getrestorants()
        {
            List<restorant> restorants = db.restorants.ToList();
            List<RestaurantDto> Restaurants = new List<RestaurantDto>();
            foreach (var item in restorants)
            {
                RestaurantDto res = new RestaurantDto()
                {
                    id = item.id,
                    name = item.name,
                    is_featured = item.is_featured,
                    active = item.active,
                    address = item.address,
                    city_id = item.city_id,
                    cover = item.cover,
                    created_at = item.created_at,
                    radius = item.radius,
                    description = item.description,
                    fee = item.fee,
                    lat = item.lat,
                    lng = item.lng,
                    logo = item.logo,
                    minimum = item.minimum,
                    phone = item.phone,
                    static_fee = item.static_fee,
                    subdomain = item.subdomain,
                    updated_at = item.updated_at,
                    user_id = item.user_id
                };
                Restaurants.Add(res);
            }
            return Restaurants;
        }

        // GET: api/restorants/5
        [ResponseType(typeof(restorant))]
        public async Task<IHttpActionResult> Getrestorant(decimal id)
        {
            restorant restorant = await db.restorants.FindAsync(id);
            if (restorant == null)
            {
                return NotFound();
            }

            return Ok(restorant);
        }

        // PUT: api/restorants/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrestorant(decimal id, restorant restorant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restorant.id)
            {
                return BadRequest();
            }

            db.Entry(restorant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!restorantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/restorants
        [ResponseType(typeof(restorant))]
        public async Task<IHttpActionResult> Postrestorant(restorant restorant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.restorants.Add(restorant);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = restorant.id }, restorant);
        }

        // DELETE: api/restorants/5
        [ResponseType(typeof(restorant))]
        public async Task<IHttpActionResult> Deleterestorant(decimal id)
        {
            restorant restorant = await db.restorants.FindAsync(id);
            if (restorant == null)
            {
                return NotFound();
            }

            db.restorants.Remove(restorant);
            await db.SaveChangesAsync();

            return Ok(restorant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool restorantExists(decimal id)
        {
            return db.restorants.Count(e => e.id == id) > 0;
        }
    }
}