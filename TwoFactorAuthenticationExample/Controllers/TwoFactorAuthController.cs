using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using TwoFactorAuthenticationExample.Models;

namespace TwoFactorAuthenticationExample.Controllers;

public class TwoFactorAuthController : Controller
{
    [HttpGet]
    public ActionResult Enable()
    {
        var user = CurrentUser.SignedInUser;
        TwoFactorAuthenticator twoFactor = new();
        var setupInfo = twoFactor.GenerateSetupCode("twoFactorAuth", user.Email, TwoFactorKey(user), false, 3);
        ViewBag.SetupCode = setupInfo.ManualEntryKey;
        ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
        return View();
    }

    [HttpPost]
    public ActionResult Enable(string inputCode)
    {
        var user = CurrentUser.SignedInUser;
        TwoFactorAuthenticator twoFactor = new();
        bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
        if (!isValid)
        {
            return Redirect("/TwoFactorAuth/Enable");
        }

        user.TwoFactorEnabled = true;
        return Redirect("/");
    }

    [HttpGet]
    public IActionResult Disable()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Disable(string inputCode)
    {
        var user = CurrentUser.SignedInUser;
        TwoFactorAuthenticator twoFactor = new();
        bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
        if (!isValid)
        {
            return Redirect("/TwoFactorAuth/Disable");
        }

        user.TwoFactorEnabled = false;
        return Redirect("/");
    }

    [HttpGet]
    public IActionResult Authorize()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Authorize(string inputCode)
    {
        var user = CurrentUser.PreviousUser;
        TwoFactorAuthenticator twoFactor = new();
        bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(user), inputCode);
        if (!isValid)
        {
            return Redirect("/TwoFactorAuth/Authorize");
        }

        CurrentUser.SignedInUser = CurrentUser.PreviousUser;
        return Redirect("/");
    }

    private static string TwoFactorKey(CurrentUser user)
    {
        return $"myverysecretkey+{user.Email}";
    }
}
