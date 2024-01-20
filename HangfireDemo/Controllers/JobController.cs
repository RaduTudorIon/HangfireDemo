using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;

        public JobController(ILogger<JobController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            //BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
            BackgroundJob.Enqueue<Jobs.TestJob>(x => x.WriteLog("Hello world from Hangfire!"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            //BackgroundJob.Schedule(() => Console.WriteLine("Hello world from Hangfire!"), dateTimeOffset);
            BackgroundJob.Schedule<Jobs.TestJob>(x => x.WriteLog("Hello world from Hangfire!"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            //var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job 2 triggered!"), dateTimeOffset);
            //var job2Id = BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation job 1 triggered!"));
            //var job3Id = BackgroundJob.ContinueJobWith(job2Id, () => Console.WriteLine("Continuation job 2 triggered!"));
            var jobId = BackgroundJob.Schedule<Jobs.TestJob>(x => x.WriteLog("Scheduled Job 2 triggered!"), dateTimeOffset);
            var job2Id = BackgroundJob.ContinueJobWith<Jobs.TestJob>(jobId, x => x.WriteLog("Continuation job 1 triggered!"));
            var job3Id = BackgroundJob.ContinueJobWith<Jobs.TestJob>(job2Id, x => x.WriteLog("Continuation job 2 triggered!"));

            return Ok();
        }

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {
            //RecurringJob.AddOrUpdate("RecurringJob1", () => Console.WriteLine("Recurring Job Triggered"), "*/30 * * * * *");
            RecurringJob.AddOrUpdate<Jobs.TestJob>("RecurringJob1", x => x.WriteLog("Recurring Job Triggered"), "*/30 * * * * *");
            return Ok();
        }
    }
}
