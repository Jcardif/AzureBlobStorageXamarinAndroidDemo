using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Backend.DataObjects;
using Backend.Models;

namespace Backend.Controllers
{
    public class AeroplaneController : TableController<Aeroplane>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Aeroplane>(context, Request);
        }

        // GET tables/Aeroplane
        public IQueryable<Aeroplane> GetAllAeroplane()
        {
            return Query(); 
        }

        // GET tables/Aeroplane/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Aeroplane> GetAeroplane(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Aeroplane/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Aeroplane> PatchAeroplane(string id, Delta<Aeroplane> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Aeroplane
        public async Task<IHttpActionResult> PostAeroplane(Aeroplane item)
        {
            Aeroplane current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Aeroplane/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAeroplane(string id)
        {
             return DeleteAsync(id);
        }
    }
}
