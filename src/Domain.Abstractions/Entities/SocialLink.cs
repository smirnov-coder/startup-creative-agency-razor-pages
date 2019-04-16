using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Содержит информацию о странице в социальной сети.
    /// </summary>
    [Serializable]
    public class SocialLink : BaseEntity<int>, ISerializable
    {
        /// <summary>
        /// Название социальной сети.
        /// </summary>
        public string NetworkName { get; set; }
        
        /// <summary>
        /// Адрес страницы в социальной сети.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="SocialLink"/> со значением идентификатора по умолчанию и 
        /// заданными значениями названия социальной сети и адреса страницы в данной социальной сети.
        /// </summary>
        /// <param name="networkName">Название социальной сети.</param>
        /// <param name="url">Адрес страницы в социальной сети.</param>
        public SocialLink(string networkName, string url)
        {
            NetworkName = networkName;
            Url = url;
        }

        #region Serialization
        protected SocialLink(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            NetworkName = info.GetString(nameof(NetworkName));
            Url = info.GetString(nameof(Url));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(NetworkName), NetworkName);
            info.AddValue(nameof(Url), Url);
        }
        #endregion
    }
}
