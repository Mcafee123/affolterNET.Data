using System;
using affolterNET.Data.Interfaces;

namespace affolterNET.Data.Extensions
{
    public static class DtoBaseExtensions
    {
        public static Guid GetId(this IDtoBase dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null");
            }

            var idName = dto.GetIdName();
            var idProp = dto.GetType().GetProperty(idName);
            if (idProp == null)
            {
                throw new InvalidOperationException("Invalid Id Prop on Dto");
            }

            var id = idProp.GetMethod.Invoke(dto, new object[] { });
            return (Guid)id;
        }

        public static string GetString(this IDtoBase dto, string propertyname)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Dto cannot be null");
            }

            if (string.IsNullOrWhiteSpace(propertyname))
            {
                throw new ArgumentNullException(nameof(propertyname), "Propertyname cannot be empty");
            }

            var idProp = dto.GetType().GetProperty(propertyname);
            if (idProp == null)
            {
                throw new InvalidOperationException("Invalid Id Prop on Dto");
            }

            var id = idProp.GetMethod.Invoke(dto, new object[] { });
            return id?.ToString();
        }
    }
}