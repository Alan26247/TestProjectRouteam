using Alan.Identity;
using BillingService.Database;
using BillingService.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingService.Controllers
{
    [ApiController]
    public class TokensController : ControllerBase
    {
        public TokensController(IConfiguration configuration, AppDbContext db)
        {
            this.configuration = configuration;
            this.db = db;
        }

        private readonly IConfiguration configuration;
        private readonly AppDbContext db;



        /// <summary>
        /// создает токен
        /// </summary>
        /// <param name="form">форма запроса</param>
        /// <returns>возвращает токен</returns>
        [HttpPost]
        [Route("api/token")]
        public async Task<ActionResult> GetToken(FormAuthorization form)
        {
            try
            {
                User? user = db.Users.Where(u => u.name == form.name).FirstOrDefault();

                // если пользователя не существует то возвращаем 400
                if (user == null)
                    return BadRequest(new { description = "Данный пользователь в системе не зарегистрирован" });

                // конфигурируем данные
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.id));
                claims.Add(new Claim(ClaimTypes.Name, user.name));

                ClaimsIdentity identity = new(claims);

                // создаем токен
                string token = Jwt.CreateToken(identity,
                                configuration["Access:Issuer"], configuration["Access:Audience"],
                                TimeSpan.FromHours(double.Parse(configuration["Access:TokenLifeTimeHour"])), configuration["Access:Key"]);

                return Ok(new { token = token });
            }
            catch
            {
                throw;
            }
        }
    }
}
