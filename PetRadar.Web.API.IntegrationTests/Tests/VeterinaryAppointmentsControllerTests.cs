using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PetRadar.Core.Data.Entities.Enums;
using PetRadar.Core.Domain.Models;
using PetRadar.Web.API.IntegrationTests.DataSeeds;
using PetRadar.Web.API.IntegrationTests.Helpers;
using PetRadar.Web.API.ViewModels;
using System.Net.Mime;
using System.Text;
using Xunit;

namespace PetRadar.Web.API.IntegrationTests.Tests
{
    [TestCaseOrderer(PriorityOrderer.PriorityOrdererName, PriorityOrderer.PriorityOrdererAssemblyName)]
    public class VeterinaryAppointmentsControllerTests : IClassFixture<TestingWebAppFactory>
    {
        private const string BaseUrl = "/api/veterinaryappointments";
        private const string PetsBaseUrl = "/api/userpets";

        private readonly TestingWebAppFactory _factory;
        private readonly HttpClient _client;

        static long createdAppointmentId = 0;
        static long createdPetIdForNegativeTests = 0;

        public VeterinaryAppointmentsControllerTests(TestingWebAppFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        #region CRUD Operations with Priority

        [Fact, TestPriority(1)]
        public async Task GetAll_Returns_Appointments_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync(BaseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var appointments = JsonConvert.DeserializeObject<List<VeterinaryAppointmentViewModel>>(stringContent);

            Assert.NotNull(appointments);
        }

        [Fact, TestPriority(2)]
        public async Task Create_Returns_Created_Successfully()
        {
            // Arrange
            var appointmentDate = new DateTimeOffset(DateTime.UtcNow.AddDays(7), TimeSpan.Zero);

            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: appointmentDate,
                durationInMinutes: 30,
                reasonForVisit: "Annual checkup",
                notes: "Bring vaccination records",
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: 50.00m,
                addressText: "123 Vet Street"
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var stringContent = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, (int)result.StatusCode);

            var createdAppointment = JsonConvert.DeserializeObject<VeterinaryAppointmentViewModel>(stringContent);

            Assert.NotNull(createdAppointment);
            Assert.True(createdAppointment.Id > 0);
            Assert.Equal(createModel.PetId, createdAppointment.PetId);
            Assert.Equal(createModel.VeterinaryName, createdAppointment.VeterinaryName);
            Assert.Equal(createModel.AppointmentType.ToString(), createdAppointment.AppointmentType);
            Assert.Equal(createModel.AppointmentStatus.ToString(), createdAppointment.AppointmentStatus);
            Assert.Equal(createModel.ReasonForVisit, createdAppointment.ReasonForVisit);

            createdAppointmentId = createdAppointment.Id;
        }

        [Fact, TestPriority(3)]
        public async Task GetById_Returns_Appointment_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdAppointmentId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var appointment = JsonConvert.DeserializeObject<VeterinaryAppointmentViewModel>(stringContent);

            Assert.NotNull(appointment);
            Assert.Equal(createdAppointmentId, appointment.Id);
            Assert.Equal("Dr. Smith", appointment.VeterinaryName);
            Assert.Equal("Annual checkup", appointment.ReasonForVisit);
        }

        [Fact, TestPriority(4)]
        public async Task GetByPetId_Returns_Appointments_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/pet/{DefaultDataSet.DefaultPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var appointments = JsonConvert.DeserializeObject<List<VeterinaryAppointmentViewModel>>(stringContent);

            Assert.NotNull(appointments);
            Assert.True(appointments.Count > 0);
            Assert.Contains(appointments, a => a.Id == createdAppointmentId);
        }

        [Fact, TestPriority(5)]
        public async Task GetByUserId_Returns_Appointments_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/user/{DefaultDataSet.DefaultUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var appointments = JsonConvert.DeserializeObject<List<VeterinaryAppointmentViewModel>>(stringContent);

            Assert.NotNull(appointments);
            Assert.True(appointments.Count > 0);
            Assert.Contains(appointments, a => a.Id == createdAppointmentId);
        }

        [Fact, TestPriority(6)]
        public async Task Update_Returns_NoContent_Successfully()
        {
            // Arrange
            var updateModel = new VeterinaryAppointmentUpdateModel(
                veterinaryName: "Dr. Johnson",
                appointmentType: AppointmentTypeEnum.Vaccination,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: null,
                durationInMinutes: 45,
                reasonForVisit: "Vaccination booster",
                notes: "Updated notes",
                diagnosis: "Healthy",
                treatment: "Vaccination administered",
                prescriptions: null,
                cost: 75.00m,
                addressText: "456 Clinic Ave"
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAppointmentId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task GetById_Returns_Updated_Appointment_Successfully()
        {
            // Arrange & Act
            var result = await _client.GetAsync($"{BaseUrl}/{createdAppointmentId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, (int)result.StatusCode);

            var stringContent = await result.Content.ReadAsStringAsync();
            var appointment = JsonConvert.DeserializeObject<VeterinaryAppointmentViewModel>(stringContent);

            Assert.NotNull(appointment);
            Assert.Equal(createdAppointmentId, appointment.Id);
            Assert.Equal("Dr. Johnson", appointment.VeterinaryName);
            Assert.Equal("Vaccination", appointment.AppointmentType);
            Assert.Equal("Vaccination booster", appointment.ReasonForVisit);
            Assert.Equal("Healthy", appointment.Diagnosis);
            Assert.Equal("Vaccination administered", appointment.Treatment);
        }

        [Fact, TestPriority(8)]
        public async Task Delete_Returns_NoContent_Successfully()
        {
            // Arrange & Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{createdAppointmentId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, (int)result.StatusCode);
        }

        #endregion

        #region Negative Scenarios (No Priority)

        [Fact]
        public async Task GetById_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetByPetId_Returns_NotFound_With_Invalid_PetId()
        {
            // Arrange
            var invalidPetId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/pet/{invalidPetId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetByUserId_Returns_NotFound_With_Invalid_UserId()
        {
            // Arrange
            var invalidUserId = 999999;

            // Act
            var result = await _client.GetAsync($"{BaseUrl}/user/{invalidUserId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_NotFound_With_Invalid_PetId()
        {
            // Arrange
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: 999999,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Missing_Required_Fields()
        {
            // Arrange
            var createModel = new
            {
                PetId = DefaultDataSet.DefaultPetId
                // Missing AppointmentType, AppointmentStatus, AppointmentDate, ReasonForVisit
            };

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Empty_Body()
        {
            // Arrange
            var jsonModel = JsonConvert.SerializeObject(new { });

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_VeterinaryName_Exceeding_MaxLength()
        {
            // Arrange
            var longName = new string('A', 101); // Exceeds 100 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: longName,
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_ReasonForVisit_Exceeding_MaxLength()
        {
            // Arrange
            var longReason = new string('A', 101); // Exceeds 100 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: longReason,
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Notes_Exceeding_MaxLength()
        {
            // Arrange
            var longNotes = new string('A', 501); // Exceeds 500 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: longNotes,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Diagnosis_Exceeding_MaxLength()
        {
            // Arrange
            var longDiagnosis = new string('A', 501); // Exceeds 500 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: longDiagnosis,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Treatment_Exceeding_MaxLength()
        {
            // Arrange
            var longTreatment = new string('A', 501); // Exceeds 500 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: null,
                treatment: longTreatment,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_Prescriptions_Exceeding_MaxLength()
        {
            // Arrange
            var longPrescriptions = new string('A', 501); // Exceeds 500 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: longPrescriptions,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_With_AddressText_Exceeding_MaxLength()
        {
            // Arrange
            var longAddress = new string('A', 201); // Exceeds 200 character limit
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Smith",
                appointmentType: AppointmentTypeEnum.Checkup,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(7),
                durationInMinutes: null,
                reasonForVisit: "Checkup",
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: longAddress
            );

            var jsonModel = JsonConvert.SerializeObject(createModel);

            // Act
            var result = await _client.PostAsync(BaseUrl, new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;
            var updateModel = new VeterinaryAppointmentUpdateModel(
                veterinaryName: "Dr. Updated",
                appointmentType: null,
                appointmentStatus: null,
                appointmentDate: null,
                durationInMinutes: null,
                reasonForVisit: null,
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{invalidId}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_With_VeterinaryName_Exceeding_MaxLength()
        {
            // Arrange - First create an appointment to update
            var createModel = new VeterinaryAppointmentCreateModel(
                petId: DefaultDataSet.DefaultPetId,
                veterinaryName: "Dr. Temp",
                appointmentType: AppointmentTypeEnum.Consultation,
                appointmentStatus: AppointmentStatusEnum.Scheduled,
                appointmentDate: DateTimeOffset.UtcNow.AddDays(14),
                durationInMinutes: null,
                reasonForVisit: "Consultation",
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var createJson = JsonConvert.SerializeObject(createModel);
            var createResult = await _client.PostAsync(BaseUrl, new StringContent(createJson, Encoding.UTF8, MediaTypeNames.Application.Json));
            var createContent = await createResult.Content.ReadAsStringAsync();
            var createdAppointment = JsonConvert.DeserializeObject<VeterinaryAppointmentViewModel>(createContent);

            // Arrange update with invalid name
            var longName = new string('A', 101); // Exceeds 100 character limit
            var updateModel = new VeterinaryAppointmentUpdateModel(
                veterinaryName: longName,
                appointmentType: null,
                appointmentStatus: null,
                appointmentDate: null,
                durationInMinutes: null,
                reasonForVisit: null,
                notes: null,
                diagnosis: null,
                treatment: null,
                prescriptions: null,
                cost: null,
                addressText: null
            );

            var jsonModel = JsonConvert.SerializeObject(updateModel);

            // Act
            var result = await _client.PutAsync($"{BaseUrl}/{createdAppointment!.Id}", new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)result.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"{BaseUrl}/{createdAppointment.Id}");
        }

        [Fact]
        public async Task Delete_Returns_NotFound_With_Invalid_Id()
        {
            // Arrange
            var invalidId = 999999;

            // Act
            var result = await _client.DeleteAsync($"{BaseUrl}/{invalidId}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, (int)result.StatusCode);
        }

        #endregion
    }
}
