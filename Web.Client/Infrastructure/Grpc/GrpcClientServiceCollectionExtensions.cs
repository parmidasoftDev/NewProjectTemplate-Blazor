﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Havit.Blazor.Components.Web;
using Havit.GoranG3.Contracts;
using Havit.GoranG3.Web.Client.Infrastructure.Grpc;
using Havit.GoranG3.Web.Client.Infrastructure.Interceptors;
using Havit.GoranG3.Web.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.ClientFactory;
using ProtoBuf.Grpc.Configuration;

namespace Havit.GoranG3.Web.Client.Infrastructure.Grpc
{
	public static class GrpcClientServiceCollectionExtensions
	{
		private static string backendUrl;

		public static void AddGrpcClientInfrastructure(this IServiceCollection services)
		{
			services.AddTransient<AuthorizationGrpcClientInterceptor>();
			services.AddTransient<ServerExceptionsGrpcClientInterceptor>();
			services.AddTransient<GrpcWebHandler>(provider => new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
		}

		public static IHttpClientBuilder AddGrpcClientProxy<TService>(this IServiceCollection services)
			where TService : class
		{
			return services
				.AddCodeFirstGrpcClient<TService>((provider, options) =>
				{
					options.Address = new Uri(GetBackendUrl(provider));
				})
				.ConfigurePrimaryHttpMessageHandler<GrpcWebHandler>()
				.AddInterceptor<ServerExceptionsGrpcClientInterceptor>();
		}

		public static IHttpClientBuilder AddGrpcClientProxyWithAuth<TService>(this IServiceCollection services)
			where TService : class
		{
			return AddGrpcClientProxy<TService>(services)
				.AddHttpMessageHandler(provider =>
				{
					return provider.GetRequiredService<AuthorizationMessageHandler>()
						.ConfigureHandler(authorizedUrls: new[] { GetBackendUrl(provider) }); // scopes: new[] { "example.read", "example.write" }
				})
				.AddInterceptor<AuthorizationGrpcClientInterceptor>();
		}

		private static string GetBackendUrl(IServiceProvider provider)
		{
			if (backendUrl == null)
			{
				var config = provider.GetRequiredService<IConfiguration>();
				backendUrl = config["BackendUrl"];

				// If no address is set then fallback to the current webpage URL
				if (string.IsNullOrEmpty(backendUrl))
				{
					var navigationManager = provider.GetRequiredService<NavigationManager>();
					backendUrl = navigationManager.BaseUri;
				}
			}

			return backendUrl;
		}
	}
}
