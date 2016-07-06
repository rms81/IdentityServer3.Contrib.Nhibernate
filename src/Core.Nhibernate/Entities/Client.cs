using System;
using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdentityServer3.Contrib.Nhibernate.Entities
{
    public class Client : BaseEntity<Guid>
    {
        public virtual bool Enabled { get; set; }

        public virtual string ClientId { get; set; }

        public virtual ISet<ClientSecret> ClientSecrets { get; } = new HashSet<ClientSecret>();

        public virtual string ClientName { get; set; }

        public virtual string ClientUri { get; set; }

        public virtual string LogoUri { get; set; }

        public virtual bool RequireConsent { get; set; }

        public virtual bool AllowRememberConsent { get; set; }

        public virtual bool AllowAccessTokensViaBrowser { get; set; }

        public virtual Flows Flow { get; set; }

        public virtual bool AllowClientCredentialsOnly { get; set; }

        public virtual ISet<ClientRedirectUri> RedirectUris { get; } = new HashSet<ClientRedirectUri>();

        public virtual ISet<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; } = new HashSet<ClientPostLogoutRedirectUri>();

        public virtual string LogoutUri { get; set; }

        public virtual bool LogoutSessionRequired { get; set; }

        public virtual bool RequireSignOutPrompt { get; set; }

        public virtual bool AllowAccessToAllScopes { get; set; }

        public virtual ISet<ClientScope> AllowedScopes { get; } = new HashSet<ClientScope>();

        public virtual int IdentityTokenLifetime { get; set; }

        public virtual int AccessTokenLifetime { get; set; }

        public virtual int AuthorizationCodeLifetime { get; set; }

        public virtual int AbsoluteRefreshTokenLifetime { get; set; }

        public virtual int SlidingRefreshTokenLifetime { get; set; }

        public virtual TokenUsage RefreshTokenUsage { get; set; }

        public virtual bool UpdateAccessTokenOnRefresh { get; set; }

        public virtual TokenExpiration RefreshTokenExpiration { get; set; }

        public virtual AccessTokenType AccessTokenType { get; set; }

        public virtual bool EnableLocalLogin { get; set; }

        public virtual ISet<ClientIdPRestriction> IdentityProviderRestrictions { get; } = new HashSet<ClientIdPRestriction>();

        public virtual bool IncludeJwtId { get; set; }

        public virtual ISet<ClientClaim> Claims { get; } = new HashSet<ClientClaim>();

        public virtual bool AlwaysSendClientClaims { get; set; }

        public virtual bool PrefixClientClaims { get; set; }

        public virtual bool AllowAccessToAllGrantTypes { get; set; }

        public virtual ISet<ClientCustomGrantType> AllowedCustomGrantTypes { get; } = new HashSet<ClientCustomGrantType>();

        public virtual ISet<ClientCorsOrigin> AllowedCorsOrigins { get; } = new HashSet<ClientCorsOrigin>();
    }
}
