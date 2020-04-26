// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace ABCService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                 // machine to machine client (from quickstart 1)
                 new Client
                {
                    ClientId = "console client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                 },
                // mvc client using code flow + pkce
                new Client
                {
                    ClientId = "mvc client",
                    ClientName = "MVC Client",
                    ClientSecrets = { new Secret("mvc secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    //指定使用基于授权代码的授权类型的客户端是否必须发送校验密钥
                   // RequirePkce = true,
                    //RequireConsent = false, //禁用 确认授权那个consent 页面确认
                    //指定允许的URI以返回令牌或授权码
                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    //指定客户端的注销URI，以用于基于HTTP的前端通道注销
                    FrontChannelLogoutUri = "http://localhost:5003/signout-oidc",
                    //指定在注销后重定向到的允许URI
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },
                    AllowOfflineAccess = true, // offline_access
                    //token过期时间60s
                    AccessTokenLifetime=60,
                    AllowedScopes = {"api1", 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //IdentityServerConstants.StandardScopes.Email,
                        //IdentityServerConstants.StandardScopes.Phone,
                        //IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    }

                },
            };
    }
}