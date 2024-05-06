using Microsoft.AspNetCore.Mvc;

namespace APBD.Controllers;

public static class ControllerIseExtension
{
    public static StatusCodeResult InternalServerError(this ControllerBase self)
    {
        return self.StatusCode(StatusCodes.Status500InternalServerError);
    }
}