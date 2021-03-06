﻿using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage whitelabeling settings
	/// </summary>
	public class Whitelabel
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Whitelabel" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Whitelabel(IClient client, string endpoint = "/whitelabel")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get all domain whitelabels
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="excludeSubusers">if set to <c>true</c> [exclude subusers].</param>
		/// <param name="username">The username.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="WhitelabelDomain" />.
		/// </returns>
		/// <remarks>
		/// A domain whitelabel consists of a subdomain and domain that will be used to set the
		/// appropriate DKIM, SPF, and Return-Path. There is an option to allow SendGrid to manage
		/// security or the customers may manage their own DNS records. For customers using the
		/// manual security option, they will need to create the appropriate MX, DKIM, and SPF records
		/// with their hosting provider. With automatic security, the customer will just need to create a
		/// few CNAMEs to SendGrid, and SendGrid will manage the MX, DKIM and SPF records.
		/// </remarks>
		public async Task<WhitelabelDomain[]> GetAllDomainsAsync(int limit = 50, int offset = 0, bool excludeSubusers = false, string username = null, string domain = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains?exclude_subusers={1}&limit={2}&offset={3}&username={4}&domain={5}", _endpoint, excludeSubusers ? "true" : "false", limit, offset, username, domain);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var domains = JArray.Parse(responseContent).ToObject<WhitelabelDomain[]>();
			return domains;
		}

		/// <summary>
		/// Get a specific domain whitelabel
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> GetDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, domainId);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var domain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Create a new domain whitelabel
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="automaticSecurity">if set to <c>true</c> [automatic security].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> CreateDomainAsync(string domain, string subdomain, bool automaticSecurity = false, bool customSpf = false, bool isDefault = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains", _endpoint);
			var data = new JObject
			{
				{ "domain", domain },
				{ "subdomain", subdomain },
				{ "automatic_security", automaticSecurity },
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Update a whitelabel domain.
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="customSpf">if set to <c>true</c> [custom SPF].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> UpdateDomainAsync(long domainId, bool isDefault = false, bool customSpf = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, domainId);
			var data = new JObject
			{
				{ "custom_spf", customSpf },
				{ "default", isDefault }
			};
			var response = await _client.PatchAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Delete a whitelabel domain.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteDomainAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}", _endpoint, id);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Add an IP to a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> AddIpAddressToDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips", _endpoint, domainId);
			var data = new JObject
			{
				{ "ip", ipAddress }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Remove an IP from a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> DeleteIpAddressFromDomainAsync(long domainId, string ipAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/ips/{2}", _endpoint, domainId, ipAddress);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelDomain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return whitelabelDomain;
		}

		/// <summary>
		/// Validate a Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="DomainValidation" />.
		/// </returns>
		public async Task<DomainValidation> ValidateDomainAsync(long domainId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/validate", _endpoint, domainId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var domainValidation = JObject.Parse(responseContent).ToObject<DomainValidation>();
			return domainValidation;
		}

		/// <summary>
		/// Get Associated Domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		/// <remarks>
		/// Domain Whitelabels can be associated with subusers via parent accounts. This functionality
		/// allows subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the
		/// parent account must first create a Whitelabel and validate it. Then the parent may associate
		/// the Whitelabel in subuser management.
		/// </remarks>
		public async Task<WhitelabelDomain> GetAssociatedDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var domain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Disassociate Domain
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DisassociateDomainAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/subuser?username={1}", _endpoint, username);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Associate Domain
		/// </summary>
		/// <param name="domainId">The domain identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelDomain" />.
		/// </returns>
		public async Task<WhitelabelDomain> AssociateDomainAsync(long domainId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/domains/{1}/subuser", _endpoint, domainId);
			var data = new JObject
			{
				{ "username", username }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var domain = JObject.Parse(responseContent).ToObject<WhitelabelDomain>();
			return domain;
		}

		/// <summary>
		/// Get all IP whitelabels
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelabelIp" />.
		/// </returns>
		/// <remarks>
		/// A IP whitelabel consists of a subdomain and domain that will be used to generate a reverse
		/// DNS record for a given IP. Once SendGrid has verified that the customer has created the
		/// appropriate A record for their IP, SendGrid will create the appropriate reverse DNS record for
		/// the IP.
		/// </remarks>
		public async Task<WhitelabelIp[]> GetAllIpsAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var ips = JArray.Parse(responseContent).ToObject<WhitelabelIp[]>();
			return ips;
		}

		/// <summary>
		/// Get a specific IP whitelabel
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		public async Task<WhitelabelIp> GetIpAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, id);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var ip = JObject.Parse(responseContent).ToObject<WhitelabelIp>();
			return ip;
		}

		/// <summary>
		/// Create an IP
		/// </summary>
		/// <param name="ipAddress">The ip address.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelIp" />.
		/// </returns>
		public async Task<WhitelabelIp> CreateIpAsync(string ipAddress, string domain, string subdomain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips", _endpoint);
			var data = new JObject
			{
				{ "ip", ipAddress },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelIp = JObject.Parse(responseContent).ToObject<WhitelabelIp>();
			return whitelabelIp;
		}

		/// <summary>
		/// Delete an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}", _endpoint, ipId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Validate an IP
		/// </summary>
		/// <param name="ipId">The ip identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="IpValidation" />.
		/// </returns>
		public async Task<IpValidation> ValidateIpAsync(long ipId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/ips/{1}/validate", _endpoint, ipId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var ipValidation = JObject.Parse(responseContent).ToObject<IpValidation>();
			return ipValidation;
		}

		/// <summary>
		/// Get all Link whitelabels
		/// </summary>
		/// <param name="segmentPrefix">The segment prefix.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelabelLink" />.
		/// </returns>
		/// <remarks>
		/// A link whitelabel consists of a subdomain and domain that will be used to rewrite links in mail
		/// messages. Our customer will be asked to create a couple CNAME records for the links to be
		/// rewritten to and for us to verify that they are the domain owners.
		/// </remarks>
		public async Task<WhitelabelLink[]> GetAllLinksAsync(string segmentPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links?limit={1}&offset={2}&ip={3}", _endpoint, limit, offset, segmentPrefix);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var links = JArray.Parse(responseContent).ToObject<WhitelabelLink[]>();
			return links;
		}

		/// <summary>
		/// Get a specific Link whitelabel
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public async Task<WhitelabelLink> GetLinkAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, id);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Create a Link
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="subdomain">The subdomain.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public async Task<WhitelabelLink> CreateLinkAsync(string domain, string subdomain, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links", _endpoint);
			var data = new JObject
			{
				{ "default", isDefault },
				{ "domain", domain },
				{ "subdomain", subdomain }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelLink = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return whitelabelLink;
		}

		/// <summary>
		/// Update a Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public async Task<WhitelabelLink> UpdateLinkAsync(long linkId, bool isDefault, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			var data = new JObject
			{
				{ "default", isDefault }
			};
			var response = await _client.PatchAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var whitelabelLink = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return whitelabelLink;
		}

		/// <summary>
		/// Delete a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}", _endpoint, linkId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get the default link for a domain
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public async Task<WhitelabelLink> GetDefaultLinkAsync(string domain, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/default?domain={1}", _endpoint, domain);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Validate a link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="LinkValidation" />.
		/// </returns>
		public async Task<LinkValidation> ValidateLinkAsync(long linkId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/validate", _endpoint, linkId);
			var response = await _client.PostAsync(endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var linkValidation = JObject.Parse(responseContent).ToObject<LinkValidation>();
			return linkValidation;
		}

		/// <summary>
		/// Get Associated Link
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		/// <remarks>
		/// Link Whitelabels can be associated with subusers via parent accounts. This functionality allows
		/// subusers to send mail off their parent's Whitelabels. To associate a Whitelabel, the parent
		/// account must first create a Whitelabel and validate it. Then the parent may associate the
		/// Whitelabel in subuser management.
		/// </remarks>
		public async Task<WhitelabelLink> GetAssociatedLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}

		/// <summary>
		/// Disassociate Link
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DisassociateLinkAsync(string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/subuser?username={1}", _endpoint, username);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Associate Link
		/// </summary>
		/// <param name="linkId">The link identifier.</param>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="WhitelabelLink" />.
		/// </returns>
		public async Task<WhitelabelLink> AssociateLinkAsync(long linkId, string username = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/links/{1}/subuser", _endpoint, linkId);
			var data = new JObject
			{
				{ "username", username }
			};
			var response = await _client.PostAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var link = JObject.Parse(responseContent).ToObject<WhitelabelLink>();
			return link;
		}
	}
}
