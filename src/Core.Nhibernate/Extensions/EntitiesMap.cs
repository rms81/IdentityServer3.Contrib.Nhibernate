using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

// ReSharper disable once CheckNamespace
namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public static class EntitiesMap
    {
        public static IMapper Mapper { get; set; }

        static EntitiesMap()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Scope, IdentityServer3.Core.Models.Scope>(MemberList.Destination)
                    .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.ScopeClaims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<ScopeClaim, IdentityServer3.Core.Models.ScopeClaim>(MemberList.Destination);
                config.CreateMap<ScopeSecret, IdentityServer3.Core.Models.Secret>(MemberList.Destination)
                    .ForMember(dest => dest.Type, opt => opt.Condition(srs => !srs.IsSourceValueNull));

                config.CreateMap<ClientSecret, IdentityServer3.Core.Models.Secret>(MemberList.Destination)
                    .ForMember(dest => dest.Type, opt => opt.Condition(srs => !srs.IsSourceValueNull));
                config.CreateMap<Client, IdentityServer3.Core.Models.Client>(MemberList.Destination)
                    .ForMember(x => x.UpdateAccessTokenClaimsOnRefresh,
                        opt => opt.MapFrom(src => src.UpdateAccessTokenOnRefresh))
                    .ForMember(x => x.AllowAccessToAllCustomGrantTypes,
                        opt => opt.MapFrom(src => src.AllowAccessToAllGrantTypes))
                    .ForMember(x => x.AllowedCustomGrantTypes,
                        opt => opt.MapFrom(src => src.AllowedCustomGrantTypes.Select(x => x.GrantType)))
                    .ForMember(x => x.RedirectUris, opt => opt.MapFrom(src => src.RedirectUris.Select(x => x.Uri)))
                    .ForMember(x => x.PostLogoutRedirectUris,
                        opt => opt.MapFrom(src => src.PostLogoutRedirectUris.Select(x => x.Uri)))
                    .ForMember(x => x.IdentityProviderRestrictions,
                        opt => opt.MapFrom(src => src.IdentityProviderRestrictions.Select(x => x.Provider)))
                    .ForMember(x => x.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes.Select(x => x.Scope)))
                    .ForMember(x => x.AllowedCorsOrigins,
                        opt => opt.MapFrom(src => src.AllowedCorsOrigins.Select(x => x.Origin)))
                    .ForMember(x => x.Claims,
                        opt => opt.MapFrom(src => src.Claims.Select(x => new Claim(x.Type, x.Value))));
            }).CreateMapper();
        }

        public static IdentityServer3.Core.Models.Scope ToModel(this Scope s)
        {
            if (s == null) return null;
            return Mapper.Map<Scope, IdentityServer3.Core.Models.Scope>(s);
        }

        public static IdentityServer3.Core.Models.Client ToModel(this Client s)
        {
            if (s == null) return null;
            return Mapper.Map<Client, IdentityServer3.Core.Models.Client>(s);
        }
    }
}
