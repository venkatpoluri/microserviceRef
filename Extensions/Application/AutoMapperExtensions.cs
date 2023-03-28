
using System;
using AutoMapper;
using AutoMapper.Internal;
namespace TradingPartnerManagement.Extensions.Application;

public static class AutoMapperExtensions
{
    class Resolver : IMemberValueResolver<object, object, object, object>
    {
        public object Resolve(object source, object destination, object sourceMember, object destinationMember, ResolutionContext context)
        {
            return sourceMember ?? destinationMember;
        }
    }

    public static void IgnoreSourceWhenNull(this IMapperConfigurationExpression cfg)
    {
        cfg.Internal().ForAllPropertyMaps(pm =>
                     {
                         if (pm.SourceType == null)
                             return false;
                         var isNullable = pm.SourceType.IsGenericType && (pm.SourceType.GetGenericTypeDefinition() == typeof(Nullable<>));
                         return isNullable || pm.SourceType.IsClass;
                     }, (pm, c) =>
                      {
                          c.MapFrom<object, object, object, object>(new Resolver(), pm.SourceMember.Name);
                      });
    }
}

        