using FindLostThingsBackEnd.Middleware;
using FindLostThingsBackEnd.Persistence.Model;
using System;
using System.Linq;

namespace FindLostThingsBackEnd.Helper
{
    public static class ThingHelper
    {
        public static LostThingsRecord MergeLostThingsRecord(this LostThingsRecord origin,LostThingsRecord changed)
        {
            var CanModifyProperties = typeof(LostThingsRecord)
                                            .GetProperties()
                                            .Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(ShouldNotModifyAttribute)));
            foreach (var prop in CanModifyProperties)
            {
                prop.SetValue(origin, prop.GetValue(changed));
            }
            return origin;
        }

        public static bool EnsureNotModifyFieldsUnchanged(LostThingsRecord Old,LostThingsRecord New)
        {
            var CannotModifyProperties = typeof(LostThingsRecord)
                                            .GetProperties()
                                            .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ShouldNotModifyAttribute)));

            foreach (var prop in CannotModifyProperties)
            {
                Type FieldType = prop.GetType();

                var OldField = prop.GetValue(Old);
                var NewField = prop.GetValue(New);
                if (!OldField.Equals(NewField))
                    return false;
            }
            return true;
        }
    }
}
