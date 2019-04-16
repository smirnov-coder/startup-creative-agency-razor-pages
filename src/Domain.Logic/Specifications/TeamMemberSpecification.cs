using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Domain.Logic.Specifications
{
    /// <summary>
    /// Спецификация пользователя доменной модели, отмеченного как член команды компании.
    /// </summary>
    public class TeamMemberSpecification : BaseSpecification<DomainUser>
    {
        /// <summary>
        /// Создаёт новый экземпляр спецификации.
        /// </summary>
        public TeamMemberSpecification() : base(x => x.Profile.DisplayAsTeamMember == true) { }
    }
}
