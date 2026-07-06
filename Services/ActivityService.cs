using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;

namespace inventory_v2.Services
{
    public class ActivityService : IActivityService
    {
        private readonly DataContext _context;

        public ActivityService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<List<Activity>>> GetActivityList()
        {
            var response = new ResponseMessage<List<Activity>>();
            try
            {
                var activities = await Task.Run(() => _context.Activity.ToList());
                if (activities != null && activities.Count > 0)
                {
                    response.Data = activities;
                    response.Count = activities.Count;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No activity(s) found";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseMessage<string>> AddActivity(Activity activity)
        {
            var response = new ResponseMessage<string>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                activity.Id = Guid.NewGuid().ToString();
                activity.CreatedOn = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.Activity.AddAsync(activity);
                int result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    response.Success = false;
                    response.Message = "Failed to add activity.";

                    return response;
                }
                else
                {
                    response.Success = true;
                    response.Message = "Activity added successfully.";

                    return response;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

        }
    }

    public interface IActivityService
    {
        Task<ResponseMessage<List<Activity>>> GetActivityList();
        Task<ResponseMessage<string>> AddActivity(Activity activity);
    }
}