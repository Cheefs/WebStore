using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<User> _UserManager;
    private readonly SignInManager<User> _SignInManager;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<User> UserManager,
        SignInManager<User> SignInManager,
        ILogger<AccountController> Logger)
    {
        _UserManager = UserManager;
        _SignInManager = SignInManager;
        _Logger = Logger;
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new RegisterUserViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        var user = new User
        {
            UserName = Model.UserName,
        };

        _Logger.LogTrace($"Начало процесса регистрации нового пользователя {Model.UserName}");
        var timer = Stopwatch.StartNew();

        using var loggerScope = _Logger.BeginScope($"Регистрация нового пользователя {Model.UserName}");

        var creation_result = await _UserManager.CreateAsync(user, Model.Password);
        if (creation_result.Succeeded)
        {
            _Logger.LogInformation($"Пользователь {user} зарегистрирован за {timer.ElapsedMilliseconds} мс");

            await _UserManager.AddToRoleAsync(user, Role.Users);

            _Logger.LogInformation($"Пользователю {user} назначена роль {Role.Users}. {timer.ElapsedMilliseconds} мс");

            await _SignInManager.SignInAsync(user, false);

            _Logger.LogTrace($"Пользователь {user} пошёл в систему. {timer.ElapsedMilliseconds} мс");

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in creation_result.Errors)
            ModelState.AddModelError("", error.Description);

        var errorInfo = string.Join(", ", creation_result.Errors.Select(e => e.Description));
        _Logger.LogWarning($"Ошибка при регистрации пользователя {user} ({timer.ElapsedMilliseconds} мс):{errorInfo}");

        return View(Model);
    }

    [AllowAnonymous]
    public IActionResult Login(string? ReturnUrl) => View(new LoginViewModel { ReturnUrl = ReturnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        _Logger.LogTrace($"Начат вход в систему пользователя {Model.UserName}");

        using var loggerScope = _Logger.BeginScope($"Вход в систему пользователя {Model.UserName}");

        var timer = Stopwatch.StartNew();

        var login_result = await _SignInManager.PasswordSignInAsync(
            Model.UserName,
            Model.Password,
            Model.RememberMe,
            lockoutOnFailure: true);

        if (login_result.Succeeded)
        {
            _Logger.LogInformation($"Пользователь {Model.UserName} успешно вошёл в систему. {timer.ElapsedMilliseconds} мс");

            return LocalRedirect(Model.ReturnUrl ?? "/");
        }

        ModelState.AddModelError("", "Неверное имя пользователя, или пароль");

        _Logger.LogWarning($"Ошибка входа пользователя {Model.UserName} - неверное имя, или пароль. {timer.ElapsedMilliseconds} мс");

        return View(Model);
    }

    public async Task<IActionResult> Logout()
    {
        var user_name = User.Identity!.Name;

        await _SignInManager.SignOutAsync();

        _Logger.LogInformation("Пользователь {0} вышел из системы", user_name);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied(string? ReturnUrl)
    {
        ViewBag.ReturnUrl = ReturnUrl!;
        return View();
    }
}
