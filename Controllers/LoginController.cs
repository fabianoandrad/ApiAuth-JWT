using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiAuth.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using ApiAuth.Services;
using ApiAuth.Repositories;

namespace ApiAuth.Controllers
{
    [Route("v1/account")]
    public class LoginController : Controller
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] User model)
        {
            // Busca usuário no banco e verifica se informações ok
            var user = UserRepository.Get(model.Username, model.Password);

            // Se usuário não existe retorna NotFound
            if (user == null)
                return NotFound(new { msg = "Usuário ou senha inválidos" });

            // Gera o token do usuário
            var token = TokenService.GenerateToken(user);

            // Oculta a senha do usuário
            user.Password = "";

            // Retorna usuário e seu token
            return new
            {
                user = user,
                token = token
            };
        }

        // Permiti logar sem autorização
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        // Somente usuários autenticados
        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => $"Autenticado - {User.Identity.Name}";

        // Somente usuário employee ou manager autorizado
        [HttpGet]
        [Route("employee")]
        [Authorize(Roles = "employee, manager")]
        public string Employee() => $"Funcionário logado - {User.Identity.Name}";

        // Somente usuário manager autorizado
        [HttpGet]
        [Route("manager")]
        [Authorize(Roles = "manager")]
        public string Manager() => "Gerente";

    }
}
