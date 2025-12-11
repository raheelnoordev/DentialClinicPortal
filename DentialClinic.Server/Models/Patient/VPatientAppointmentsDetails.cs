using System.ComponentModel.DataAnnotations.Schema;

namespace DentialClinic.Server.Models.Patient
{
    [Table("v_patient_appointments_details")]
    public class VPatientAppointmentsDetails
    {
        // Appointment info
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Column("appointment_time")]
        public DateTime? AppointmentTime { get; set; }

        [Column("appointment_status")]
        public string? AppointmentStatus { get; set; }

        [Column("appointment_source")]
        public string? AppointmentSource { get; set; }

        [Column("treatment_id")]
        public int? TreatmentId { get; set; }

        [Column("whatsapp_booking_id")]
        public int? WhatsappBookingId { get; set; }

        [Column("appointment_reason")]
        public string? AppointmentReason { get; set; }

        [Column("appointment_created_at")]
        public DateTime? AppointmentCreatedAt { get; set; }

        // Branch info
        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("branch_name")]
        public string? BranchName { get; set; }

        // Patient info
        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("mrn")]
        public string? MRN { get; set; }

        [Column("patient_name")]
        public string? PatientName { get; set; }

        [Column("patient_phone")]
        public string? PatientPhone { get; set; }

        [Column("patient_whatsapp")]
        public string? PatientWhatsapp { get; set; }

        [Column("patient_cnic")]
        public string? PatientCnic { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("patient_address")]
        public string? PatientAddress { get; set; }

        // Doctor info
        [Column("doctor_id")]
        public int? DoctorId { get; set; }

        [Column("doctor_name")]
        public string? DoctorName { get; set; }

        [Column("doctor_speciality")]
        public string? DoctorSpeciality { get; set; }

        [Column("doctor_default_fee")]
        public decimal? DoctorDefaultFee { get; set; }

        [Column("doctor_is_active")]
        public bool DoctorIsActive { get; set; }

        // Treatment info
        [Column("treatment_name")]
        public string? TreatmentName { get; set; }

        [Column("treatment_description")]
        public string? TreatmentDescription { get; set; }

        [Column("treatment_default_price")]
        public decimal? TreatmentDefaultPrice { get; set; }

        [Column("treatment_default_duration_minutes")]
        public int? TreatmentDefaultDurationMinutes { get; set; }

        [Column("treatment_is_active")]
        public bool TreatmentIsActive { get; set; }

        // Created By info
        [Column("created_by_user_id")]
        public int? CreatedByUserId { get; set; }

        [Column("patient_created_by_user_name")]
        public string? PatientCreatedByUserName { get; set; }

        [Column("appointment_created_by_user_id")]
        public int? AppointmentCreatedByUserId { get; set; }

        [Column("appointment_created_by_user_name")]
        public string? AppointmentCreatedByUserName { get; set; }
    }
}

