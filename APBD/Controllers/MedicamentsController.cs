using APBD.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace APBD.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("/api/medicaments")]
public class MedicamentsController : Controller
{
    private readonly IPatientsDatabase _database;

    public MedicamentsController(IPatientsDatabase db)
    {
        _database = db;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            Medicament? info = await _database.GetMedicamentInfoByIdAsync(id);
            if (info == null)
            {
                return NotFound("No such prescription exists.");
            }

            IReadOnlyList<PrescriptionEntry> prescriptions
                = await _database.GetPrescriptionsForMedicamentByIdAsync(id);

            return Ok(new MedicamentByIdResult(info, prescriptions));
        }
        catch (Exception ex)
        {
#if DEBUG
            throw;
#else
            return this.InternalServerError();
#endif
        }
    }

    private record MedicamentByIdResult(
        Medicament Info,
        IReadOnlyList<PrescriptionEntry> Prescriptions
    );
}