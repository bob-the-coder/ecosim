using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using WebGrease.Css.Extensions;

namespace EcoSim.Controllers
{
    public class OutputController : Controller
    {
        // GET: Output
        public async Task<ActionResult> SearchResults(Guid id)
        {
            var node = await NodeManager.GetAsync(id).ConfigureAwait(false);
            var network = await NodeManager.GetAllAsync().ConfigureAwait(false);
            var bfsResult = await NetworkManager.GetBfsList(node).ConfigureAwait(false);
            var result = new List<string>();

            foreach (var dest in network)
            {
                var res = await NetworkManager.GetBfsPath(dest, bfsResult).ConfigureAwait(false);
                result.Add(res);
            }

            return View(result);
        }
    }
}