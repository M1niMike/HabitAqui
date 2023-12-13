using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers
{
    public class ManagerArea : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        //private readonly ILogger<RegisterModel> _logger;

        public ManagerArea(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _userStore = userStore;
        }


        //######################################

        //MANAGER AREA

        //######################################


        public async Task<IActionResult> ManagerList()
        {


            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;

            // Obtenha a lista de gestores associados à empresa
            var managers = await _context.Managers
                .Include(m => m.Company).Include(m => m.ApplicationUser)
                .Where(m => m.CompanyId == company.Id)
                .ToListAsync();

            // Agora, você tem a lista de gestores e funcionários associados à mesma empresa
            // Faça o que for necessário com as listas (por exemplo, passá-las para a exibição)

            return View(managers);
        }



        public IActionResult CreateManager()
        {

            return View(); // ou qualquer outra ação
        }


        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManager([Bind("Manager,Employee,FirstName,LastName,UserName,Password,Available")] ManagerAreaViewModel model)
        {

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;


            // Cria um novo usuário
            var newManager = new ApplicationUser
            {
                UserName = $"gestor_{model.FirstName}{company.EmailDomain}.com",
                Email = $"gestor_{model.FirstName}{company.EmailDomain}.com",
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            // Verifica se o usuário já existe pelo e-mail
            var user = await _userManager.FindByEmailAsync(newManager.Email);

            try
            {
                if (ModelState.IsValid)
                {
                   

                    if (user == null)
                    {
                        

                        await _userStore.SetUserNameAsync(newManager, newManager.UserName, CancellationToken.None);
                        var identityResult = await _userManager.CreateAsync(newManager, model.Password);

                        if (identityResult.Succeeded)
                        {
                            // Adiciona o usuário à role "Manager"
                            await _userManager.AddToRoleAsync(newManager, Roles.Manager.ToString());

                            var managerClass = new Manager
                            {
                                Name = $"{model.FirstName} {model.LastName}",
                                CompanyId = manager.CompanyId,  // Usa o mesmo CompanyId do manager atual
                                ApplicationUserId = newManager.Id  // Associa ao novo usuário
                            };

                            _context.Managers.Add(managerClass);
                            await _context.SaveChangesAsync();

                            Console.WriteLine("Contador: " + manager.Company.Managers.Count());

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Exibir todos os erros do modelo
                            foreach (var error in identityResult.Errors)
                            {
                                Console.WriteLine("Erro ao criar usuário: " + error.Description);
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        // Trata o caso em que o usuário já existe
                        Console.WriteLine("Um usuário com o mesmo e-mail já existe.");
                        ModelState.AddModelError(string.Empty, "Um usuário com o mesmo e-mail já existe.");
                    }
                }
                else
                {
                    // Model não é válido - exibir todos os erros
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção: " + ex.Message);
            }

            return View(model);
        }

        



        // GET: Homes/Edit/5
        public async Task<IActionResult> ManagerEdit(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }


            return View(manager);
        }


        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerEdit(int id, [Bind("Id,Name,Available,CompanyId,ApplicationUserId")] Manager updatedManager)
        {
            if (id != updatedManager.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(updatedManager.ApplicationUser));
            ModelState.Remove(nameof(updatedManager.Company));

            try 
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingManager = await _context.Managers.FindAsync(id);
                        if (existingManager == null)
                        {
                            return NotFound();
                        }

                        // Atualiza apenas as propriedades desejadas
                        existingManager.Name = updatedManager.Name;
                        existingManager.Available = updatedManager.Available;

                        _context.Update(existingManager);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!ManagerExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        throw;
                    }
                    return RedirectToAction(nameof(ManagerList));
                }
                else
                {
                    // Model não é válido - exibir todos os erros
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção: " + ex.Message);
            }
            return View(updatedManager);

        }



        private bool ManagerExists(int id)
        {
            return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
        }




        //######################################

        //EMPLOYEE AREA

        //######################################


        public async Task<IActionResult> EmployeeList()
        {


            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;

            // Obtenha a lista de gestores associados à empresa
            var employees = await _context.Employees
                .Include(e => e.Company).Include(m => m.ApplicationUser)
                .Where(e => e.CompanyId == company.Id)
                .ToListAsync();


            return View(employees);
        }



        public IActionResult CreateEmployee()
        {

            return View(); // ou qualquer outra ação
        }


        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee([Bind("Manager,Employee,FirstName,LastName,UserName,Password,Available")] ManagerAreaViewModel model)
        {

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;

            // Cria um novo usuário
            var newEmployee = new ApplicationUser
            {
                UserName = $"{model.FirstName}{company.EmailDomain}.com",
                Email = $"{model.FirstName}{company.EmailDomain}.com",
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            // Verifica se o usuário já existe pelo e-mail
            var user = await _userManager.FindByEmailAsync(newEmployee.Email);


            try
            {

                if (ModelState.IsValid)
                {

                    if (user == null)
                    {


                        await _userStore.SetUserNameAsync(newEmployee, newEmployee.UserName, CancellationToken.None);
                        var identityResult = await _userManager.CreateAsync(newEmployee, model.Password);

                        if (identityResult.Succeeded)
                        {
                            // Adiciona o usuário à role "Manager"
                            await _userManager.AddToRoleAsync(newEmployee, Roles.Employee.ToString());

                            var employeeClass = new Employee
                            {
                                Name = $"{model.FirstName} {model.LastName}",
                                CompanyId = manager.CompanyId,  // Usa o mesmo CompanyId do manager atual
                                Available = true,
                                ApplicationUserId = newEmployee.Id  // Associa ao novo usuário
                            };

                            _context.Employees.Add(employeeClass);
                            await _context.SaveChangesAsync();

                            Console.WriteLine("Contador: " + manager.Company.Employees.Count());

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Exibir todos os erros do modelo
                            foreach (var error in identityResult.Errors)
                            {
                                Console.WriteLine("Erro ao criar usuário: " + error.Description);
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        // Trata o caso em que o usuário já existe
                        Console.WriteLine("Um usuário com o mesmo e-mail já existe.");
                        ModelState.AddModelError(string.Empty, "Um usuário com o mesmo e-mail já existe.");
                    }
                }
                else
                {
                    // Model não é válido - exibir todos os erros
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção: " + ex.Message);
            }

            return View(model);
        }



        // GET: Homes/Edit/5
        public async Task<IActionResult> EmployeeEdit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }


            return View(employee);
        }


        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeEdit(int id, [Bind("Id,Name,Available,CompanyId,ApplicationUserId")] Employee updatedEmployee)
        {
            if (id != updatedEmployee.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(updatedEmployee.ApplicationUser));
            ModelState.Remove(nameof(updatedEmployee.Company));

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingEmployee = await _context.Employees.FindAsync(id);
                        if (existingEmployee == null)
                        {
                            return NotFound();
                        }

                        // Atualiza apenas as propriedades desejadas
                        existingEmployee.Name = updatedEmployee.Name;
                        existingEmployee.Available = updatedEmployee.Available;

                        _context.Update(existingEmployee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!ManagerExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        throw;
                    }
                    return RedirectToAction(nameof(EmployeeList));
                }
                else
                {
                    // Model não é válido - exibir todos os erros
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção: " + ex.Message);
            }
            return View(updatedEmployee);

        }



        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
