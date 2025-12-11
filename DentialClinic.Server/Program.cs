using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Repository;
using DentialClinic.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DentialClinic.Server.ModelBinders.CommaSeparatedListModelBinderProvider());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});




// Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DentialClinicCon")));

// Register services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserManagement, UserManagmentRepository>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddScoped<ICashbookService, CashbookService>();
builder.Services.AddScoped<IMoneyAccountService, MoneyAccountService>();

// Dental Clinic Services
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IBillingService, BillingService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();

//builder.Services.AddScoped<IUserService, Us>();
//builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();


app.Run();