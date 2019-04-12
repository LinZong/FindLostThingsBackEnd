using FindLostThingsBackEnd.Model.Request.User;
using FindLostThingsBackEnd.Persistence.Model;
using System.Collections.Generic;

namespace FindLostThingsBackEnd.Helper
{
    public static class ContactsHelper
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
    }
}
