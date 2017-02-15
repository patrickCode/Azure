using System;
using Common.Table;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;

namespace Consumer.Web.Controllers
{
    [RoutePrefix("api/servicerequests")]
    public class ServiceRequestController : ApiController
    {
        private readonly Sender _sender;
        private readonly TableProvider<ServiceRequestEntity> _serviceRequestTableProvider;
        private readonly StatusChecker _statusChecker;
        public ServiceRequestController()
        {
            var storageAccountName = ConfigurationManager.AppSettings["Microsoft.Storage.AccountName"];
            var storageAccountKey = ConfigurationManager.AppSettings["Microsoft.Storage.AccountKey"];
            _serviceRequestTableProvider = new TableProvider<ServiceRequestEntity>(storageAccountName, storageAccountKey, "ServiceRequest");
            var statusTableProvider = new TableProvider<StatusTableEntity>(storageAccountName, storageAccountKey, "Status");

            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var topicName = ConfigurationManager.AppSettings["Microsoft.ServiceBus.TopicName"];
            _sender = new Sender(connectionString, topicName, statusTableProvider);

            _statusChecker = new StatusChecker(statusTableProvider);
        }

        [HttpPost]
        public async Task<Guid> CreateServiceRequest(ServiceRequest serviceReqeust)
        {
            var tracker = Guid.NewGuid();
            serviceReqeust.Tracker = tracker;
            await _sender.SendRequestAsync(serviceReqeust);
            return tracker;
        }

        [Route("{trackingGuid}/status")]
        [HttpGet]
        public StatusInfo GetStatus(string trackingGuid)
        {
            return _statusChecker.GetStatus(trackingGuid);
        }
    }
}