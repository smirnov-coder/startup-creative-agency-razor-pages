using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StartupCreativeAgency.Domain.Entities
{
    /// <summary>
    /// Представляет собой контейнер для хранения контактных данных компании определённого типа: физический адрес, 
    /// телефон или адрес электронной почты.
    /// </summary>
    [Serializable]
    public class ContactInfo : ISerializable
    {
        /// <summary>
        /// Название типа контактных данных. Служит идентификатором сущности.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Заголовок для отображения в пользовательском интерфейсе.
        /// </summary>
        public string Caption { get; set; }
        
        /// <summary>
        /// Коллекция значений контактных данных заданного типа.
        /// </summary>
        public IList<string> Values { get; set; } = new List<string>();

        private ContactInfo() { }

        /// <summary>
        /// Создаёт новый экземпляр сущности <see cref="ContactInfo"/> с заданным значением идентификатора сущности.
        /// </summary>
        /// <param name="name">Название типа контактных данных. Идентификатор сущности.</param>
        public ContactInfo(string name) => Name = name;

        #region Serialization
        protected ContactInfo(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            Name = info.GetString(nameof(Name));
            Caption = info.GetString(nameof(Caption));
            Values = (List<string>)info.GetValue(nameof(Values), typeof(List<string>));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Caption), Caption);
            info.AddValue(nameof(Values), Values);
        }
        #endregion
    }
}
