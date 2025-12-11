using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.WhatsApp;

namespace DentialClinic.Server.Interfaces
{
    public interface IWhatsAppService
    {
        // Settings
        Task<ServiceResponse<List<WhatsAppSettings>>> GetAllSettingsAsync();
        Task<ServiceResponse<WhatsAppSettings>> GetSettingsByBranchAsync(int branchId);
        Task<ServiceResponse<WhatsAppSettings>> CreateOrUpdateSettingsAsync(WhatsAppSettings request);
        
        // Screens
        Task<ServiceResponse<List<WhatsAppScreen>>> GetAllScreensAsync(int? branchId);
        Task<ServiceResponse<WhatsAppScreen>> GetScreenByIdAsync(int id);
        Task<ServiceResponse<WhatsAppScreen>> CreateScreenAsync(WhatsAppScreen request);
        Task<ServiceResponse<WhatsAppScreen>> UpdateScreenAsync(int id, WhatsAppScreen request);
        Task<ServiceResponse<bool>> DeleteScreenAsync(int id);
        
        // Screen Options
        Task<ServiceResponse<List<WhatsAppScreenOption>>> GetOptionsByScreenAsync(int screenId);
        Task<ServiceResponse<WhatsAppScreenOption>> CreateOptionAsync(WhatsAppScreenOption request);
        Task<ServiceResponse<WhatsAppScreenOption>> UpdateOptionAsync(int id, WhatsAppScreenOption request);
        Task<ServiceResponse<bool>> DeleteOptionAsync(int id);
        
        // Conversation State
        Task<ServiceResponse<WhatsAppConversationState>> GetConversationStateAsync(string fromNumber);
        Task<ServiceResponse<WhatsAppConversationState>> UpdateConversationStateAsync(WhatsAppConversationState request);
        
        // Bookings
        Task<ServiceResponse<List<WhatsAppBooking>>> GetAllBookingsAsync(int? branchId);
        Task<ServiceResponse<WhatsAppBooking>> GetBookingByIdAsync(int id);
        Task<ServiceResponse<WhatsAppBooking>> CreateBookingAsync(WhatsAppBooking request);
        Task<ServiceResponse<WhatsAppBooking>> UpdateBookingStatusAsync(int id, string status, int? patientId);
    }
}

