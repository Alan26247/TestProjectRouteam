using BillingService.Database;
using BillingService.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Authorize]
    [ApiController]
    public class CoinsController : ControllerBase
    {
        public CoinsController(AppDbContext db)
        {
            this.db = db;
        }

        private readonly AppDbContext db;



        /// <summary>
        /// получить валюту с максимальной историей перемещания
        /// </summary>
        /// <returns>возвращает валюту с максимальной историей перемещения</returns>
        [HttpGet]
        [Route("api/coin/max")]
        public ActionResult GetCoinMax()
        {
            try
            {
                int maxHistory = db.Coins.Max(c => c.history_length);

                IQueryable<Coin> coinsMax = db.Coins.Where(c => c.history_length == maxHistory);

                return Ok(coinsMax.FirstOrDefault());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// добавить валюту
        /// </summary>
        /// <param name="form">форма добавления валюты</param>
        /// <returns>возвращает результат</returns>
        [HttpPost]
        [Route("api/coins")]
        public async Task<ActionResult> AddCoins(FormAddCoins form)
        {
            try
            {
                // получаем отсортированных по имени пользователей
                User[] users = db.Users.OrderBy(u => u.name).ToArray();

                Guid guidGenerator = Guid.NewGuid();

                int countRemainingCoins = form.amount;
                int userIndex = 0;
                while (countRemainingCoins > 0)
                {
                    // создаем coin
                    Coin newCoin = new()
                    {
                        id = Guid.NewGuid().ToString(),
                        history_length = 1
                    };

                    db.Coins.Add(newCoin);

                    // создаем связь с пользователем
                    Owner newOwnerCoint = new()
                    {
                        user_id = users[userIndex].id,
                        coin_id = newCoin.id
                    };
                    db.Owners.Add(newOwnerCoint);

                    // инкрементируем пользователя
                    userIndex++;
                    if (userIndex >= users.Length) userIndex = 0;

                    // уменьшаем счетчик на 1
                    countRemainingCoins--;
                }

                // записываем все изменения
                await db.SaveChangesAsync();

                return Created("", "Валюта добавлена");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// переместить валюту
        /// </summary>
        /// <param name="form">форма перемещения валюты</param>
        /// <returns>возвращает результат</returns>
        [HttpPost]
        [Route("api/coins/move")]
        public async Task<ActionResult> MoveCoins(FormMoveCoins form)
        {
            try
            {
                // проверяем есть ли пользователи с данными id
                if(db.Users.Where(u => u.id == form.userIdFrom).FirstOrDefault() == null) 
                    return BadRequest(new { description = "Пользователь перечисляющий валюту не зарегистрирован" });

                if (db.Users.Where(u => u.id == form.userIdFrom).FirstOrDefault() == null)
                    return BadRequest(new { description = "Пользователь принимающий валюту не зарегистрирован" });

                IQueryable<Owner> ownerCoins = db.Owners.Where(u => u.user_id == form.userIdFrom);
                Coin[] coins = db.Coins.Where(c => ownerCoins.Where(u => u.coin_id == c.id).Any()).ToArray();

                // проверяем достаточно ли валюты у перечисляющего пользователя
                if (ownerCoins.Count() < form.amount)
                    return BadRequest(new { description = "Недостатосно средств у перечисляющего пользователя" });

                int amount = form.amount;
                foreach(Owner ownerCoin in ownerCoins)
                {
                    // если перечислять больше нечего выходим с цикла
                    if (amount < 1) break;

                    ownerCoin.user_id = form.userIdTo;

                    Coin currentCoin = coins.Where(c => c.id == ownerCoin.coin_id).First();

                    currentCoin.history_length++;

                    db.Coins.Update(currentCoin);

                    amount--;
                }

                await db.SaveChangesAsync();

                return Ok("Валюта переведена");
            }
            catch
            {
                throw;
            }
        }
    }
}
