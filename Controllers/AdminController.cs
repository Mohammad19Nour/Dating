using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating_App.Controllers;

public class AdminController :BaseApiController
{
    [Authorize(Policy = "RequiredAdminRole")]
    [HttpGet("users-with-roles")]
    public ActionResult GetUserWithRoles()
    {
        return Ok( "only admin can see this");
    }
}