using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer3.Contrib.Nhibernate.Entities;

// ReSharper disable once CheckNamespace
namespace IdentityServer3.Core.Models
{
    public static class EntitiesMap
    {
        public static IMapper Mapper { get; set; }

        static EntitiesMap()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Models.Scope, Contrib.Nhibernate.Entities.Scope>(MemberList.Source)
                    .ForSourceMember(x => x.Claims, opts => opts.Ignore())
                    .ForMember(x => x.ScopeClaims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<Models.ScopeClaim, Contrib.Nhibernate.Entities.ScopeClaim>(MemberList.Source);
                config.CreateMap<Models.Secret, ScopeSecret>(MemberList.Source);

                config.CreateMap<Models.Secret, ClientSecret>(MemberList.Source);
                config.CreateMap<Models.Client, Contrib.Nhibernate.Entities.Client>(MemberList.Source)
                    .ForMember(x => x.UpdateAccessTokenOnRefresh,
                        opt => opt.MapFrom(src => src.UpdateAccessTokenClaimsOnRefresh))
                    .ForMember(x => x.AllowAccessToAllGrantTypes,
                        opt => opt.MapFrom(src => src.AllowAccessToAllCustomGrantTypes))
                    .ForMember(x => x.ClientSecrets, opt => opt.MapFrom(src => src.ClientSecrets))
                    .ForMember(x => x.AllowedCustomGrantTypes,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.AllowedCustomGrantTypes.Select(
                                        x => new ClientCustomGrantType { GrantType = x })))
                    .ForMember(x => x.RedirectUris,
                        opt =>
                            opt.MapFrom(src => src.RedirectUris.Select(x => new ClientRedirectUri { Uri = x })))
                    .ForMember(x => x.PostLogoutRedirectUris,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.PostLogoutRedirectUris.Select(
                                        x => new ClientPostLogoutRedirectUri { Uri = x })))
                    .ForMember(x => x.IdentityProviderRestrictions,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.IdentityProviderRestrictions.Select(
                                        x => new ClientIdPRestriction { Provider = x })))
                    .ForMember(x => x.AllowedScopes,
                        opt => opt.MapFrom(src => src.AllowedScopes.Select(x => new ClientScope { Scope = x })))
                    .ForMember(x => x.AllowedCorsOrigins,
                        opt =>
                            opt.MapFrom(
                                src => src.AllowedCorsOrigins.Select(x => new ClientCorsOrigin { Origin = x })))
                    .ForMember(x => x.Claims,
                        opt =>
                            opt.MapFrom(
                                src => src.Claims.Select(x => new ClientClaim { Type = x.Type, Value = x.Value })));
            }).CreateMapper();
        }

        public static Contrib.Nhibernate.Entities.Scope ToEntity(this Models.Scope s)
        {
            if (s == null) return null;

            if (s.Claims == null)
            {
                s.Claims = new List<Models.ScopeClaim>();
            }
            if (s.ScopeSecrets == null)
            {
                s.ScopeSecrets = new List<Models.Secret>();
            }

            return Mapper.Map<Models.Scope, Contrib.Nhibernate.Entities.Scope>(s);
        }

        public static Contrib.Nhibernate.Entities.Client ToEntity(this Models.Client s)
        {
            if (s == null) return null;

            if (s.ClientSecrets == null)
            {
                s.ClientSecrets = new List<Secret>();
            }
            if (s.RedirectUris == null)
            {
                s.RedirectUris = new List<string>();
            }
            if (s.PostLogoutRedirectUris == null)
            {
                s.PostLogoutRedirectUris = new List<string>();
            }
            if (s.AllowedScopes == null)
            {
                s.AllowedScopes = new List<string>();
            }
            if (s.IdentityProviderRestrictions == null)
            {
                s.IdentityProviderRestrictions = new List<string>();
            }
            if (s.Claims == null)
            {
                s.Claims = new List<Claim>();
            }
            if (s.AllowedCustomGrantTypes == null)
            {
                s.AllowedCustomGrantTypes = new List<string>();
            }
            if (s.AllowedCorsOrigins == null)
            {
                s.AllowedCorsOrigins = new List<string>();
            }

            return Mapper.Map<Models.Client, Contrib.Nhibernate.Entities.Client>(s);
        }
    }
}
