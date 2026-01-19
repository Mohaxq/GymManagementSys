using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.AccountViewModel;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var user = _accountService.ValidUser(loginViewModel);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid user or password");
                return View(loginViewModel);
            }
            var result = _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false).Result;
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("InvalidLogin", "You are not allowed to login");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("InvalidLogin", "Your account is locked");
            }
            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(loginViewModel);

        }
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync().GetAwaiter().GetResult();
            return RedirectToAction("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
