using Supabase;
using Shutool.Models;

namespace Shutool.Services
{
    public class SupabaseService
    {
        private readonly Client _client;

        // Inject the Supabase Client
        public SupabaseService(Client client)
        {
            _client = client;
        }

        // 1. Authentication Method
        public async Task<string> LoginUserAsync(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignIn(email, password);

                if (session != null && session.User != null)
                {
                    // Fetch the user's role from the public.users table to determine routing
                    var userRecord = await _client.From<UserModel>()
                                                  .Where(x => x.Id == session.User.Id)
                                                  .Single();

                    return userRecord?.Role ?? "unknown";
                }
                return null;
            }
            catch (Exception ex)
            {
                // In production, log the exact error. For now, output to console.
                Console.WriteLine($"Login Error: {ex.Message}");
                return null;
            }
        }

        // 2. Add future methods here (e.g., CreateRequestAsync, GetPendingRequestsAsync)
        public async Task<bool> SignUpUserAsync(string email, string password, string name)
        {
            try
            {
                // 1. Create the user in Supabase Auth
                var session = await _client.Auth.SignUp(email, password);

                if (session != null && session.User != null)
                {
                    // 2. Create the associated profile in the public.users table
                    var newUser = new UserModel
                    {
                        Id = session.User.Id,
                        Email = email,
                        Username = name,
                        Role = "rider", // Defaulting new signups to riders
                        AvatarId = 1
                    };

                    await _client.From<UserModel>().Insert(newUser);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Signup Error: {ex.Message}");
                return false;
            }
        }
        public async Task<(bool IsSuccess, string Message)> CreatePriorityRequestAsync(string reason)
        {
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return (false, "User not logged in.");

                // 1. Check the daily limit
                // We query the requests table for this user where the creation date is today
                var startOfDay = DateTime.UtcNow.Date;
                var todayRequests = await _client.From<RequestModel>()
                                                 .Where(r => r.RiderId == currentUser.Id)
                                                 .Get();

                // Filter locally for today's date to avoid timezone query issues
                var countToday = todayRequests.Models.Count(r => r.CreatedAt.Date == startOfDay);

                if (countToday >= 3)
                {
                    return (false, "You have reached your daily limit of 3 priority requests.");
                }

                // 2. Insert the new request
                var newRequest = new RequestModel
                {
                    RiderId = currentUser.Id,
                    Reason = reason,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _client.From<RequestModel>().Insert(newRequest);
                return (true, "Priority request sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request Error: {ex.Message}");
                return (false, "An error occurred while sending your request.");
            }
        }

        public async Task<List<DriverRequestDisplayModel>> GetPendingRequestsAsync()
        {
            var displayList = new List<DriverRequestDisplayModel>();

            try
            {
                // 1. Fetch all requests where Status is "pending"
                var pendingRequests = await _client.From<RequestModel>()
                                                   .Where(r => r.Status == "pending")
                                                   .Get();

                if (pendingRequests.Models.Count == 0) return displayList;

                // 2. Extract the unique Rider IDs
                var riderIds = pendingRequests.Models.Select(r => r.RiderId).Distinct().ToList();

                // 3. Fetch the user details for those riders
                var riders = await _client.From<UserModel>()
                                          .Where(u => riderIds.Contains(u.Id))
                                          .Get();

                // 4. Combine the data into our Display Model
                foreach (var req in pendingRequests.Models)
                {
                    var rider = riders.Models.FirstOrDefault(u => u.Id == req.RiderId);

                    displayList.Add(new DriverRequestDisplayModel
                    {
                        RequestId = req.Id,
                        RiderName = rider?.Username ?? "Unknown Student",
                        DateFormatted = req.CreatedAt.ToLocalTime().ToString("MMM dd - hh:mm tt"),
                        Status = req.Status
                    });
                }

                // Sort by oldest first (first come, first served)
                return displayList.OrderBy(d => d.DateFormatted).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fetch Error: {ex.Message}");
                return displayList;
            }
        }



        // Fetch only the logged-in rider's requests
        public async Task<List<RequestModel>> GetRiderHistoryAsync()
        {
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return new List<RequestModel>();

                var response = await _client.From<RequestModel>()
                                            .Where(r => r.RiderId == currentUser.Id)
                                            .Get();

                // Return ordered by newest first
                return response.Models.OrderByDescending(r => r.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"History Error: {ex.Message}");
                return new List<RequestModel>();
            }
        }

        // Delete a specific request
        public async Task<bool> DeleteRequestAsync(string requestId)
        {
            try
            {
                await _client.From<RequestModel>()
                             .Where(r => r.Id == requestId)
                             .Delete();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return false;
            }
        }


        // 1. Mark a request as completed and assign the driver
        public async Task<bool> CompleteRequestAsync(string requestId)
        {
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return false;

                await _client.From<RequestModel>()
                             .Where(r => r.Id == requestId)
                             .Set(x => x.Status, "completed")
                             .Set(x => x.DriverId, currentUser.Id) // Record who drove them
                             .Update();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return false;
            }
        }

        // 2. Fetch the logged-in driver's completed history
        public async Task<List<DriverRequestDisplayModel>> GetDriverHistoryAsync()
        {
            var displayList = new List<DriverRequestDisplayModel>();
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return displayList;

                // Fetch completed requests handled by THIS specific driver
                var history = await _client.From<RequestModel>()
                                           .Where(r => r.DriverId == currentUser.Id && r.Status == "completed")
                                           .Get();

                if (history.Models.Count == 0) return displayList;

                // Fetch rider usernames just like we did for the pending list
                var riderIds = history.Models.Select(r => r.RiderId).Distinct().ToList();
                var riders = await _client.From<UserModel>().Where(u => riderIds.Contains(u.Id)).Get();

                foreach (var req in history.Models)
                {
                    var rider = riders.Models.FirstOrDefault(u => u.Id == req.RiderId);
                    displayList.Add(new DriverRequestDisplayModel
                    {
                        RequestId = req.Id,
                        RiderName = rider?.Username ?? "Unknown Student",
                        DateFormatted = req.CreatedAt.ToLocalTime().ToString("MMM dd - hh:mm tt"),
                        Status = req.Status
                    });
                }

                return displayList.OrderByDescending(d => d.DateFormatted).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Driver History Error: {ex.Message}");
                return displayList;
            }
        }



        // 1. Fetch the Current User's Profile
        public async Task<UserModel> GetCurrentUserProfileAsync()
        {
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return null;

                var profile = await _client.From<UserModel>()
                                           .Where(u => u.Id == currentUser.Id)
                                           .Single();
                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile Fetch Error: {ex.Message}");
                return null;
            }
        }

        // 2. Update the Profile
        public async Task<bool> UpdateProfileAsync(string username, string studentId, int avatarId)
        {
            try
            {
                var currentUser = _client.Auth.CurrentUser;
                if (currentUser == null) return false;

                await _client.From<UserModel>()
                             .Where(u => u.Id == currentUser.Id)
                             .Set(x => x.Username, username)
                             .Set(x => x.StudentId, studentId)
                             .Set(x => x.AvatarId, avatarId)
                             .Update();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile Update Error: {ex.Message}");
                return false;
            }
        }


        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            try
            {
                // Since Row Level Security (RLS) is disabled for development, 
                // the admin can fetch everything directly.
                var response = await _client.From<UserModel>()
                                            .Order(x => x.Role, Postgrest.Constants.Ordering.Descending)
                                            .Get();

                return response.Models.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin Fetch Error: {ex.Message}");
                return new List<UserModel>();
            }
        }


        public async Task<List<AdminRequestDisplayModel>> GetAllRequestsLogAsync()
        {
            var displayList = new List<AdminRequestDisplayModel>();
            try
            {
                // 1. Fetch all requests and all users
                var requestsResponse = await _client.From<RequestModel>().Get();
                var usersResponse = await _client.From<UserModel>().Get();

                var requests = requestsResponse.Models;
                var users = usersResponse.Models;

                // 2. Map them together
                foreach (var req in requests)
                {
                    var rider = users.FirstOrDefault(u => u.Id == req.RiderId);
                    var driver = req.DriverId != null ? users.FirstOrDefault(u => u.Id == req.DriverId) : null;

                    displayList.Add(new AdminRequestDisplayModel
                    {
                        // Grab the first 5 characters of the ID for a cleaner UI
                        ShortId = req.Id.Substring(0, 5).ToUpper(),
                        TimeFormatted = req.CreatedAt.ToLocalTime().ToString("MMM dd, hh:mm tt"),
                        RequestedBy = rider?.Username ?? "Unknown",
                        HandledBy = driver?.Username ?? "Pending",
                        Status = req.Status
                    });
                }

                return displayList.OrderByDescending(d => d.TimeFormatted).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admin Log Error: {ex.Message}");
                return displayList;
            }
        }


    }
}