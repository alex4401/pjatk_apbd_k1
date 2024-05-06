using APBD.Models;

namespace APBD;

public interface IPatientsDatabase
{
    public Task<Medicament?> GetMedicamentInfoByIdAsync(int id);
    public Task<IReadOnlyList<PrescriptionEntry>> GetPrescriptionsForMedicamentByIdAsync(int medId);
    public Task<bool> CheckPatientExistsAsync(int id);
    public Task<bool> DeletePatientInfoAsync(int id);
}