using eKYC.Application.Admin;
using eKYC.Domain.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eKYC.Api.Controllers;

/// <summary>Edit-session locks for the current user (table A_Object_Locks): take a record before editing, keep it alive, release it.</summary>
/// <remarks>
/// Pessimistic locking (Pattern 2) - blocks a second editor up front, while <c>row_version</c> (Pattern 1) still catches conflicts at
/// save time. A lock that is not refreshed for the configured time (<c>ObjectLocks:TtlMinutes</c>) counts as abandoned and can be taken
/// over, so a crashed browser never blocks a record for good. Administrators can also release locks in Administriranje.
/// </remarks>
[ApiController]
[Route("api/locks")]
[Authorize]
public sealed class ObjectLocksController : ControllerBase
{
    private readonly IA_Object_LocksService _service;

    public ObjectLocksController(IA_Object_LocksService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    /// <summary>Gets the lock timings the client needs.</summary>
    /// <remarks><c>TtlMinutes</c> is when an unrefreshed lock counts as abandoned; <c>HeartbeatSeconds</c> is how often an open editor should refresh its lock.</remarks>
    /// <response code="200">The settings.</response>
    [HttpGet("settings")]
    public ActionResult<ObjectLockSettings> GetSettings() => Ok(_service.GetSettings());

    /// <summary>Takes the lock on a record (or refreshes the caller's own lock).</summary>
    /// <remarks>
    /// <c>Object_Class</c> + <c>Object_Id</c> identify the record, e.g. <c>CL_Doc_Revisions</c> + the revision id. If another user holds it
    /// the answer is 409 and <c>currentRecord</c> is the lock in the way (holder name and age).
    /// </remarks>
    /// <response code="200">The caller now holds the lock; the body is the lock.</response>
    /// <response code="400">Missing or too long object class.</response>
    /// <response code="403">The caller has no active A_Usr record, so a lock cannot be attributed to anyone.</response>
    /// <response code="409">Someone else is editing the record.</response>
    [HttpPost]
    public async Task<IActionResult> Acquire(AcquireLockRequest request)
    {
        var result = await _service.AcquireAsync(request);
        if (result.Acquired)
        {
            return Ok(result.Lock);
        }

        return Conflict(new
        {
            error = "locked",
            message = $"Slog trenutno uređuje {result.Lock?.Usr_Nm_Fst} {result.Lock?.Usr_Nm_Lst} ({result.Lock?.Lgn_Nm}).".Replace("  ", " "),
            currentRecord = result.Lock,
        });
    }

    /// <summary>Keeps the caller's lock alive.</summary>
    /// <param name="id">The <c>Object_Lock_Id</c> returned when the lock was taken.</param>
    /// <response code="204">Refreshed.</response>
    /// <response code="409">The lock no longer belongs to the caller (it expired and was taken over, or an administrator released it).</response>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Heartbeat(int id) =>
        await _service.HeartbeatAsync(id)
            ? NoContent()
            : Conflict(new { error = "lock_lost", message = "Zaključavanje je izgubljeno." });

    /// <summary>Releases a lock (the holder's own, or any lock for an administrator).</summary>
    /// <param name="id">The <c>Object_Lock_Id</c>.</param>
    /// <response code="204">Released.</response>
    /// <response code="404">No such lock, or it is not the caller's.</response>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Release(int id) => await _service.ReleaseAsync(id) ? NoContent() : NotFound();
}
