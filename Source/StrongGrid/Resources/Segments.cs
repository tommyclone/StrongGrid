﻿using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html
	/// </remarks>
	public class Segments
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Segments" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Segments(IClient client, string endpoint = "/contactdb/segments")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public async Task<Segment> CreateAsync(string name, long listId, IEnumerable<SearchCondition> conditions, CancellationToken cancellationToken = default(CancellationToken))
		{
			conditions = conditions ?? Enumerable.Empty<SearchCondition>();

			var data = new JObject
			{
				{ "name", name },
				{ "list_id", listId },
				{ "conditions", JArray.FromObject(conditions.ToArray()) }
			};
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var segment = JObject.Parse(responseContent).ToObject<Segment>();
			return segment;
		}

		/// <summary>
		/// Gets all asynchronous.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Segment" />.
		/// </returns>
		public async Task<Segment[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "segments": [
			//     {
			//       "id": 1,
			//       "name": "Last Name Miller",
			//       "list_id": 4,
			//       "conditions": [
			//         {
			//           "field": "last_name",
			//           "value": "Miller",
			//           "operator": "eq",
			//           "and_or": ""
			//         }
			//       ],
			//       "recipient_count": 1
			//     }
			//   ]
			// }
			// We use a dynamic object to get rid of the 'segments' property and simply return an array of segments
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.segments;

			var segments = dynamicArray.ToObject<Segment[]>();
			return segments;
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public async Task<Segment> GetAsync(long segmentId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, segmentId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var segment = JObject.Parse(responseContent).ToObject<Segment>();
			return segment;
		}

		/// <summary>
		/// Updates the asynchronous.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public async Task<Segment> UpdateAsync(long segmentId, string name = null, long? listId = null, IEnumerable<SearchCondition> conditions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			conditions = conditions ?? Enumerable.Empty<SearchCondition>();

			var data = new JObject();
			if (!string.IsNullOrEmpty(name)) data.Add("name", name);
			if (listId.HasValue) data.Add("list_id", listId.Value);
			if (conditions.Any()) data.Add("conditions", JArray.FromObject(conditions.ToArray()));

			var response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, segmentId), data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);
			var segment = JObject.Parse(responseContent).ToObject<Segment>();
			return segment;
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="deleteMatchingContacts">if set to <c>true</c> [delete matching contacts].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DeleteAsync(long segmentId, bool deleteMatchingContacts = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}?delete_contacts={2}", _endpoint, segmentId, deleteMatchingContacts ? "true" : "false"), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Gets the recipients asynchronous.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public async Task<Contact[]> GetRecipientsAsync(long segmentId, int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}/recipients?page_size={2}&page={3}", _endpoint, segmentId, recordsPerPage, page), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "recipients": [
			//    {
			//      "created_at": 1422395108,
			//      "email": "e@example.com",
			//      "first_name": "Ed",
			//      "id": "YUBh",
			//      "last_clicked": null,
			//      "last_emailed": null,
			//      "last_name": null,
			//      "last_opened": null,
			//      "updated_at": 1422395108
			//    }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'recipients' property and simply return an array of recipients
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.recipients;

			var recipients = dynamicArray.ToObject<Contact[]>();
			return recipients;
		}
	}
}
