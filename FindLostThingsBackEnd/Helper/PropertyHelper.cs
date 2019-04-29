using FindLostThingsBackEnd.Model.Request.User;
using FindLostThingsBackEnd.Persistence.Model;
using System.Collections.Generic;
using System.Linq;

namespace FindLostThingsBackEnd.Helper
{
    public static class PropertyHelper
    {
        public static List<string> FillNonNullFieldsToUserInfoInstance(AccountContacts contacts, UserInfo info)
        {
            List<string> UpdatedFields = new List<string>();
            var properties = typeof(AccountContacts).GetProperties();
            foreach (var p in properties)
            {
                string FieldName = p.Name;
                string CurrentFieldValue = p.GetValue(contacts) as string;
                if (!string.IsNullOrEmpty(CurrentFieldValue))
                {
                    UpdatedFields.Add(FieldName);
                    var InfoField = typeof(UserInfo).GetProperty(FieldName);
                    InfoField.SetValue(info, CurrentFieldValue);
                }
            }
            return UpdatedFields;
        }

        public static T DeepCopyProperties<T,TIgnoreAttribute>(T source,T dest)
        {
            var properties = typeof(T).GetProperties().Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(TIgnoreAttribute)));
            foreach (var prop in properties)
            {
                prop.SetValue(dest,prop.GetValue(source));
            }
            return source;
        }
    }
}
