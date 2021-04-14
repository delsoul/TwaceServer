using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TwaceServer.Shared.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public override string PhoneNumber { get; set; }

        /// <summary>
        /// Баллы
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Партнерский доход
        /// </summary>
        public int AffiliatePoints { get; set; }



        [JsonIgnore]
        public virtual List<Request> Requests { get; set; } = new List<Request>() { };

        [JsonIgnore]
        public virtual List<ApplicationUser> Friends { get; set; } = new List<ApplicationUser>() { };

        [JsonIgnore]
        public virtual ConfirmCode ConfirmCode { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }

        [JsonIgnore]
        public bool Banned { get; set; }

        #region overrides

        [JsonIgnore]
        public override bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public override bool TwoFactorEnabled { get; set; }

        public override bool PhoneNumberConfirmed { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        [JsonIgnore]
        public override bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public override int AccessFailedCount { get; set; }

        [JsonIgnore]
        public override string NormalizedUserName { get; set; }

        [JsonIgnore]
        public override string NormalizedEmail { get; set; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }

        #endregion
    }
}
