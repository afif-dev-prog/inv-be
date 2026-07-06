using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;

namespace inventory_v2.Services
{
    public class StatusService : IStatusService
    {
        private readonly DataContext _context;
        private readonly IActivityService _activityService;

        public StatusService(DataContext context, IActivityService activityService)
        {
            _context = context;
            _activityService = activityService;
        }

        public async Task<ResponseMessage<List<Status>>> GetStatusList()
        {
            ResponseMessage<List<Status>> response = new ResponseMessage<List<Status>>();
            response.Data = new List<Status>();

            try
            {
                var statuses = await Task.Run(() => _context.Status.ToList());
                if (statuses != null && statuses.Count > 0)
                {
                    response.Data = statuses;
                    response.Count = statuses.Count;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No statuses found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseMessage<string>> AddStatus(Status status)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                status.Id = Guid.NewGuid().ToString();
                status.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.Status.AddAsync(status);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                response.Success = true;
                response.Message = "Status added successfully.";
                await _activityService.AddActivity(new Activity
                {
                    Description = $"Status '{status.Statuses}' added",
                    CreatedOn = status.CreatedOn,
                    ActionTaken = "ADD",
                    CreatedBy = status.CreatedBy
                });

                return response;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                return response;
            }

        }
    }
    public interface IStatusService
    {
        Task<ResponseMessage<List<Status>>> GetStatusList();
        Task<ResponseMessage<string>> AddStatus(Status status);
    }
}