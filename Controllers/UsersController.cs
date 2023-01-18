using Alan.Helpers;
using BillingService.Database;
using BillingService.Forms;
using BillingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(AppDbContext db)
        {
            this.db = db;
        }

        private readonly AppDbContext db;



        /// <summary>
        /// получить всех пользователей
        /// </summary>
        /// <param name="form">форма добавления</param>
        /// <returns>возращает результат выполнения</returns>
        [HttpPost]
        [Route("api/users")]
        public async Task<ActionResult> GetUsers(FormGetUsers form)
        {
            try
            {
                UserModel response = new();

                // получаем пользователей и записи о владении валютой
                IQueryable<User> users;

                if (form.pageNumber == 1) users = db.Users.Take(form.pageSize);
                else users = db.Users.Skip((form.pageNumber - 1) * form.pageSize).Take(form.pageSize);

                // ищем все записи косаемые пользователей
                Owner[] owners = db.Owners.Where(o => users.Where(u => u.id == o.user_id).Any()).ToArray();

                // формируем массив пользователей
                List<UserWithCoinCounts> usersList = new();

                foreach(User user in users)
                {
                    UserWithCoinCounts item = new()
                    {
                        id = user.id,
                        name = user.name
                    };

                    int countCoin = owners.Where(o => o.user_id == user.id).Count();

                    item.coinsAmount = countCoin;

                    usersList.Add(item);
                }

                response.users = usersList.ToArray();

                // пагинация
                Pagination pagination = new ();
                pagination.pageNumber = form.pageNumber;
                pagination.pageSize = form.pageSize;
                pagination.totalCount = db.Users.Count();

                // делим количество пользователей на количество елементов на странице и округляем в большую сторону
                pagination.totalPages = (int)Math.Ceiling((float)pagination.totalCount / (float)form.pageSize);

                response.pagination = pagination;

                return Ok(response);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// добавить пользователя
        /// </summary>
        /// <param name="form">форма добавления</param>
        /// <returns>возращает результат выполнения</returns>
        [HttpPut]
        [Route("api/users")]
        public async Task<ActionResult> AddUser(FormAddUser form)
        {
            try
            {
                // защита от Sql иньекции
                if (!SqlInjectionProtect.Check(form.name))
                    return StatusCode(403, "Некорректные данные");

                // проверяем есть ли такой пользователь
                if (db.Users.Where(u => u.name == form.name).FirstOrDefault() != null)
                    return BadRequest("Пользователь с таким именем уже зарегистрирован");

                // создаем нового пользователя
                Guid guidGenerator = Guid.NewGuid();

                User newUser = new()
                {
                    id = Guid.NewGuid().ToString(),
                    name = form.name
                };

                db.Users.Add(newUser);

                await db.SaveChangesAsync();

                return Created("", form.name);
            }
            catch
            {
                throw;
            }
        }
    }
}
