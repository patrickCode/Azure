using System.Web.Mvc;
using ServiceStack.Redis;

namespace ProteinTrackerRedis.Controllers
{
    public class LeaderbordController : Controller
    {
        public ActionResult Index()
        {

            using (IRedisClient client = new RedisClient())
            {
                var leaderboard = client.GetAllWithScoresFromSortedSet("urn:leaderboard");
                ViewBag.leaders = leaderboard;
            }

            return View();
        }
    }
}