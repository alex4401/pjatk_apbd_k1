using APBD.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace APBD.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("/api/patients")]
public class PatientsController : Controller
{
    private readonly IPatientsDatabase _database;

    public PatientsController(IPatientsDatabase db)
    {
        _database = db;
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById([FromRoute] int id)
    {
        try
        {
            bool wasSuccessful = await _database.DeletePatientInfoAsync(id);
            if (!wasSuccessful)
            {
                return NotFound("No such patient exists.");
            }

            return Ok();
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
}