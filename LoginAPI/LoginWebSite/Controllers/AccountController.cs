using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginWebSite.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LoginWebSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly LoginAPI.Protos.LoginService.LoginServiceClient loginClient;

        public AccountController(ILogger<AccountController> logger, LoginAPI.Protos.LoginService.LoginServiceClient loginClient)
        {
            _logger = logger;
            this.loginClient = loginClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) 
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel) 
        {
            Console.WriteLine("Logging User in");
            var res = await loginClient.LoginUserAsync(new LoginAPI.Protos.LoginRequest { Email = loginModel.Email, Password = loginModel.Password });

            if(res.ErrorCode == LoginAPI.Protos.ErrorCode.Success) 
            {
                var identity = new List<ClaimsIdentity>();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, loginModel.Email)
                };
                identity.Add(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme));
                var cp = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(cp, new AuthenticationProperties
                {
                    IsPersistent = loginModel.RememberMe,
                    AllowRefresh = true
                });

                if (res.Activated) 
                {
                    return RedirectToAction(nameof(Index));
                }
                else 
                {
                    return RedirectToAction(nameof(ConfirmUser), new ConfirmViewModel { activationFail = false });
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmUser() 
        {
            if (HttpContext.User.Identity.IsAuthenticated) 
            {
                return View();
            }
            else 
            {
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(ConfirmViewModel confirmViewModel) 
        {
            if (!HttpContext.User.Identity.IsAuthenticated) 
            {
                return RedirectToAction(nameof(Login));
            }
            if (!ModelState.IsValid) 
            {
                return View();
            }

            var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim == null) 
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }
            else 
            {
                var res = await loginClient.ActivateUserAsync(new LoginAPI.Protos.ActivationRequest
                {
                    ActivationKey = confirmViewModel.activationKey,
                    Email = emailClaim.Value
                });
                if(res.ErrorCode != LoginAPI.Protos.ErrorCode.Success) 
                {
                    if(res.ErrorCode == LoginAPI.Protos.ErrorCode.FailUnknown) 
                    {
                        Console.WriteLine("Unknown Error Occured");
                    }
                    return View();
                }
                else 
                {
                    return RedirectToAction(nameof(ActivationSuccess));
                }
            }
        }
        
        [HttpGet]
        public IActionResult ActivationSuccess() 
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) 
        {
            if (ModelState.IsValid) 
            {
                var res = await loginClient.CreateUserAsync(new LoginAPI.Protos.CreateUserRequest
                {
                    Email = model.Email,
                    Name = model.UserName,
                    Password = model.Password
                });
                if(res.ErrorCode == LoginAPI.Protos.ErrorCode.FailDuplicateEmail) 
                {
                    return View(/*Add Model Indicating Email already exists*/);
                }
                else if(res.ErrorCode == LoginAPI.Protos.ErrorCode.Success)
                {
                    return RedirectToAction(nameof(ConfirmUser));
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel) 
        {
            if (ModelState.IsValid) 
            {
                var res = await loginClient.SendPasswordOTPAsync(new LoginAPI.Protos.SendPasswordOTPRequest { Email = forgotPasswordModel.Email });
            }
            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet]
        public IActionResult ResetPassword() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordModel) 
        {
            if (!ModelState.IsValid) 
            {
                return RedirectToAction(nameof(Login));
            }

            var res = await loginClient.ResetPasswordAsync(new LoginAPI.Protos.ResetPasswordRequest 
            { 
                Email = resetPasswordModel.email, 
                NewPassword = resetPasswordModel.newPassword,
                Otp = resetPasswordModel.otp 
            });
            if(res.ErrorCode == LoginAPI.Protos.ErrorCode.Success) 
            {
                return RedirectToAction(nameof(Login));
            }
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Logout() 
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private string GetEmailFromContext() 
        {
            var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null) 
            {
                return emailClaim.Value;
            }
            return null;
        }
    }
}
