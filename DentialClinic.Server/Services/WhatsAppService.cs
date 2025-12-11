using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.WhatsApp;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly ApplicationDbContext _context;
        public WhatsAppService(ApplicationDbContext context) => _context = context;

        // Settings
        public async Task<ServiceResponse<List<WhatsAppSettings>>> GetAllSettingsAsync()
        {
            var response = new ServiceResponse<List<WhatsAppSettings>>();
            try
            {
                response.Result = await _context.WhatsAppSettings
                    .Include(s => s.Branch)
                    .OrderBy(s => s.BranchId)
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppSettings>> GetSettingsByBranchAsync(int branchId)
        {
            var response = new ServiceResponse<WhatsAppSettings>();
            try
            {
                var settings = await _context.WhatsAppSettings
                    .Include(s => s.Branch)
                    .FirstOrDefaultAsync(s => s.BranchId == branchId && s.IsActive);
                if (settings == null) { response.Success = false; response.Message = "Settings not found"; return response; }
                response.Result = settings;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppSettings>> CreateOrUpdateSettingsAsync(WhatsAppSettings request)
        {
            var response = new ServiceResponse<WhatsAppSettings>();
            try
            {
                var existing = await _context.WhatsAppSettings
                    .FirstOrDefaultAsync(s => s.BranchId == request.BranchId);
                if (existing != null)
                {
                    existing.PhoneNumberId = request.PhoneNumberId;
                    existing.BusinessAccountId = request.BusinessAccountId;
                    existing.ApiBaseUrl = request.ApiBaseUrl;
                    existing.AccessToken = request.AccessToken;
                    existing.IsActive = request.IsActive;
                }
                else
                {
                    _context.WhatsAppSettings.Add(request);
                }
                await _context.SaveChangesAsync();
                response.Result = existing ?? request;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Screens
        public async Task<ServiceResponse<List<WhatsAppScreen>>> GetAllScreensAsync(int? branchId)
        {
            var response = new ServiceResponse<List<WhatsAppScreen>>();
            try
            {
                var query = _context.WhatsAppScreens
                    .Include(s => s.Branch)
                    .Include(s => s.Options)
                    .AsQueryable();
                if (branchId.HasValue) query = query.Where(s => s.BranchId == branchId);
                response.Result = await query.OrderBy(s => s.Code).ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppScreen>> GetScreenByIdAsync(int id)
        {
            var response = new ServiceResponse<WhatsAppScreen>();
            try
            {
                var screen = await _context.WhatsAppScreens
                    .Include(s => s.Branch)
                    .Include(s => s.Options)
                    .FirstOrDefaultAsync(s => s.ScreenId == id);
                if (screen == null) { response.Success = false; response.Message = "Screen not found"; return response; }
                response.Result = screen;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppScreen>> CreateScreenAsync(WhatsAppScreen request)
        {
            var response = new ServiceResponse<WhatsAppScreen>();
            try
            {
                _context.WhatsAppScreens.Add(request);
                await _context.SaveChangesAsync();
                var getResponse = await GetScreenByIdAsync(request.ScreenId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppScreen>> UpdateScreenAsync(int id, WhatsAppScreen request)
        {
            var response = new ServiceResponse<WhatsAppScreen>();
            try
            {
                var screen = await _context.WhatsAppScreens.FindAsync(id);
                if (screen == null) { response.Success = false; response.Message = "Screen not found"; return response; }
                screen.BranchId = request.BranchId;
                screen.Code = request.Code;
                screen.LanguageCode = request.LanguageCode;
                screen.Title = request.Title;
                screen.MessageText = request.MessageText;
                screen.ScreenType = request.ScreenType;
                screen.IsActive = request.IsActive;
                await _context.SaveChangesAsync();
                var getResponse = await GetScreenByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteScreenAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var screen = await _context.WhatsAppScreens.FindAsync(id);
                if (screen == null) { response.Success = false; response.Message = "Screen not found"; return response; }
                _context.WhatsAppScreens.Remove(screen);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Screen Options
        public async Task<ServiceResponse<List<WhatsAppScreenOption>>> GetOptionsByScreenAsync(int screenId)
        {
            var response = new ServiceResponse<List<WhatsAppScreenOption>>();
            try
            {
                response.Result = await _context.WhatsAppScreenOptions
                    .Include(o => o.Screen)
                    .Include(o => o.Treatment)
                    .Where(o => o.ScreenId == screenId)
                    .OrderBy(o => o.SortOrder)
                    .ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppScreenOption>> CreateOptionAsync(WhatsAppScreenOption request)
        {
            var response = new ServiceResponse<WhatsAppScreenOption>();
            try
            {
                _context.WhatsAppScreenOptions.Add(request);
                await _context.SaveChangesAsync();
                response.Result = await _context.WhatsAppScreenOptions
                    .Include(o => o.Screen)
                    .Include(o => o.Treatment)
                    .FirstOrDefaultAsync(o => o.OptionId == request.OptionId);
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppScreenOption>> UpdateOptionAsync(int id, WhatsAppScreenOption request)
        {
            var response = new ServiceResponse<WhatsAppScreenOption>();
            try
            {
                var option = await _context.WhatsAppScreenOptions.FindAsync(id);
                if (option == null) { response.Success = false; response.Message = "Option not found"; return response; }
                option.Label = request.Label;
                option.PayloadValue = request.PayloadValue;
                option.NextScreenCode = request.NextScreenCode;
                option.TreatmentId = request.TreatmentId;
                option.SortOrder = request.SortOrder;
                option.IsActive = request.IsActive;
                await _context.SaveChangesAsync();
                response.Result = await _context.WhatsAppScreenOptions
                    .Include(o => o.Screen)
                    .Include(o => o.Treatment)
                    .FirstOrDefaultAsync(o => o.OptionId == id);
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteOptionAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var option = await _context.WhatsAppScreenOptions.FindAsync(id);
                if (option == null) { response.Success = false; response.Message = "Option not found"; return response; }
                _context.WhatsAppScreenOptions.Remove(option);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Conversation State
        public async Task<ServiceResponse<WhatsAppConversationState>> GetConversationStateAsync(string fromNumber)
        {
            var response = new ServiceResponse<WhatsAppConversationState>();
            try
            {
                var state = await _context.WhatsAppConversationStates
                    .FirstOrDefaultAsync(s => s.FromNumber == fromNumber);
                if (state == null) { response.Success = false; response.Message = "State not found"; return response; }
                response.Result = state;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppConversationState>> UpdateConversationStateAsync(WhatsAppConversationState request)
        {
            var response = new ServiceResponse<WhatsAppConversationState>();
            try
            {
                var state = await _context.WhatsAppConversationStates
                    .FirstOrDefaultAsync(s => s.FromNumber == request.FromNumber);
                if (state != null)
                {
                    state.CurrentScreenCode = request.CurrentScreenCode;
                    state.LanguageCode = request.LanguageCode;
                    state.DataJson = request.DataJson;
                    state.UpdatedAt = DateTime.Now;
                }
                else
                {
                    request.UpdatedAt = DateTime.Now;
                    _context.WhatsAppConversationStates.Add(request);
                }
                await _context.SaveChangesAsync();
                response.Result = state ?? request;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        // Bookings
        public async Task<ServiceResponse<List<WhatsAppBooking>>> GetAllBookingsAsync(int? branchId)
        {
            var response = new ServiceResponse<List<WhatsAppBooking>>();
            try
            {
                var query = _context.WhatsAppBookings
                    .Include(b => b.Branch)
                    .Include(b => b.Patient)
                    .AsQueryable();
                if (branchId.HasValue) query = query.Where(b => b.BranchId == branchId);
                response.Result = await query.OrderByDescending(b => b.ReceivedAt).ToListAsync();
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppBooking>> GetBookingByIdAsync(int id)
        {
            var response = new ServiceResponse<WhatsAppBooking>();
            try
            {
                var booking = await _context.WhatsAppBookings
                    .Include(b => b.Branch)
                    .Include(b => b.Patient)
                    .FirstOrDefaultAsync(b => b.WhatsAppBookingId == id);
                if (booking == null) { response.Success = false; response.Message = "Booking not found"; return response; }
                response.Result = booking;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppBooking>> CreateBookingAsync(WhatsAppBooking request)
        {
            var response = new ServiceResponse<WhatsAppBooking>();
            try
            {
                request.ReceivedAt = DateTime.Now;
                _context.WhatsAppBookings.Add(request);
                await _context.SaveChangesAsync();
                var getResponse = await GetBookingByIdAsync(request.WhatsAppBookingId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<WhatsAppBooking>> UpdateBookingStatusAsync(int id, string status, int? patientId)
        {
            var response = new ServiceResponse<WhatsAppBooking>();
            try
            {
                var booking = await _context.WhatsAppBookings.FindAsync(id);
                if (booking == null) { response.Success = false; response.Message = "Booking not found"; return response; }
                booking.Status = status;
                if (patientId.HasValue) booking.PatientId = patientId;
                await _context.SaveChangesAsync();
                var getResponse = await GetBookingByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }
    }
}

