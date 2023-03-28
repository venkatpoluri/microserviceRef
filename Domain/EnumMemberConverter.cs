namespace TradingPartnerManagement.Domain;

using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

public class EnumMemberConverter<T> : EnumConverter {
    public EnumMemberConverter(Type type) : base(type) { }

    public override object ConvertFrom(ITypeDescriptorContext context, 
                                       CultureInfo culture, 
                                       object value) {
        var type = typeof(T);

        foreach (var field in type.GetFields()) {
            if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute &&
                value is string enumValue &&
                attribute.Value == enumValue) {
                return field.GetValue(null);
            }
        }
        return base.ConvertFrom(context, culture, value);
    }
} 