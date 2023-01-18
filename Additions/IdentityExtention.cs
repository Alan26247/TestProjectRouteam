using System.Security.Claims;
using System.Security.Principal;

namespace Alan.Identity
{
    /// <summary>
    /// расширения для более удобной работы с identity
    /// </summary>
    public static class IdentityExtention
    {
        /// <summary>
        /// получить id пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает id</returns>
        public static int GetId(this IIdentity identity)
        {
            return identity.GetClaimValueAsInt(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// получить фамилию пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает фамилию пользователя</returns>
        public static string GetSurname(this IIdentity identity)
        {
            return identity.GetClaimValueAsString(ClaimTypes.Surname);
        }

        /// <summary>
        /// получить email пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает email пользователя</returns>
        public static string GetEmail(this IIdentity identity)
        {
            return identity.GetClaimValueAsString(ClaimTypes.Email);
        }

        /// <summary>
        /// получить отчество пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает отчество пользователя</returns>
        public static string GetPatronymic(this IIdentity identity)
        {
            return identity.GetClaimValueAsString("patronymic");
        }

        /// <summary>
        /// получить фамилию и инициалы
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает фамилию и инициалы</returns>
        public static string GetUserSNP(this IIdentity identity)
        {
            return identity.GetSurname() + " " + identity.Name[0] + "." +
                                            identity.GetPatronymic()[0] + ".";
        }

        /// <summary>
        /// получить роль пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает роль пользователя</returns>
        public static string GetRole(this IIdentity identity)
        {
            return identity.GetClaimValueAsString(ClaimTypes.Role);
        }

        /// <summary>
        /// получить день рождения пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает день рождения пользователя</returns>
        public static string GetDateOfBirth(this IIdentity identity)
        {
            return identity.GetClaimValueAsString(ClaimTypes.DateOfBirth);
        }

        /// <summary>
        /// получить пол пользователя
        /// </summary>
        /// <param name="identity">identity</param>
        /// <returns>возвращает пол пользователя</returns>
        public static string GetGender(this IIdentity identity)
        {
            return identity.GetClaimValueAsString(ClaimTypes.Gender);
        }

        /// <summary>
        /// получить значение требования по названию
        /// </summary>
        /// <param name="identity">identity</param>
        /// <param name="nameClaim">название требования</param>
        /// <returns>возвращает значение требования как строку либо пустую строку если требование не будет найдено</returns>
        public static string GetClaimValueAsString(this IIdentity identity, string nameClaim)
        {
            if (identity is not ClaimsIdentity claimsIdentity) return "";

            IEnumerable<Claim> claims = claimsIdentity.Claims;

            Claim? claim = claims.Where(c => c.Type == nameClaim).FirstOrDefault();

            if (claim != null) return claim.Value;
            else return "";
        }

        /// <summary>
        /// получить значение требования по названию
        /// </summary>
        /// <param name="identity">identity</param>
        /// <param name="nameClaim">название требования</param>
        /// <returns>возвращает значение как int требования либо 0 если требование не будет найдено</returns>
        public static int GetClaimValueAsInt(this IIdentity identity, string nameClaim)
        {
            if (identity is not ClaimsIdentity claimsIdentity) return 0;

            IEnumerable<Claim> claims = claimsIdentity.Claims;

            Claim? claim = claims.Where(c => c.Type == nameClaim).FirstOrDefault();

            if (claim != null) return int.Parse(claim.Value);
            else return 0;
        }
    }
}