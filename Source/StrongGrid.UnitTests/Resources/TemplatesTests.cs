﻿using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace StrongGrid.Resources.UnitTests
{
	public class TemplatesTests
	{
		#region FIELDS

		private const string ENDPOINT = "/templates";

		private const string SINGLE_TEMPLATE_JSON = @"{
			'id': 'e8ac01d5-a07a-4a71-b14c-4721136fe6aa',
			'name': 'example template name',
			'versions': [
				{
					'id': 'de37d11b-082a-42c0-9884-c0c143015a47',
					'user_id': 1234,
					'template_id': 'd51480ba-ca3f-465c-bc3e-ceb71d73c38d',
					'active': 1,
					'name': 'example version',
					'html_content': '<%body%><strong>Click to Reset</strong>',
					'plain_content': 'Click to Reset<%body%>',
					'subject': '<%subject%>',
					'updated_at': '2014-05-22 20:05:21'
				}
			]
		}";
		private const string MULTIPLE_TEMPLATES_JSON = @"{
			'templates': [
				{
					'id': 'e8ac01d5-a07a-4a71-b14c-4721136fe6aa',
					'name': 'example template name',
					'versions': [
						{
							'id': '5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f',
							'template_id': '9c59c1fb-931a-40fc-a658-50f871f3e41c',
							'active': 1,
							'name': 'example version name',
							'updated_at': '2014-03-19 18:56:33'
						}
					]
				}
			]
		}";
		private const string SINGLE_TEMPLATE_VERSION_JSON = @"{
			'id': '8aefe0ee-f12b-4575-b5b7-c97e21cb36f3',
			'template_id': 'ddb96bbc-9b92-425e-8979-99464621b543',
			'active': 1,
			'name': 'example_version_name',
			'html_content': '<%body%>',
			'plain_content': '<%body%>',
			'subject': '<%subject%>',
			'updated_at': '2014-03-19 18:56:33'
		}";

		#endregion

		[Fact]
		public void Parse_Template_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<Template>(SINGLE_TEMPLATE_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe("e8ac01d5-a07a-4a71-b14c-4721136fe6aa");
			result.Name.ShouldBe("example template name");
			result.Versions.Length.ShouldBe(1);
		}

		[Fact]
		public void Parse_TemplateVersion_json()
		{
			// Arrange

			// Act
			var result = JsonConvert.DeserializeObject<TemplateVersion>(SINGLE_TEMPLATE_VERSION_JSON);

			// Assert
			result.ShouldNotBeNull();
			result.HtmlContent.ShouldBe("<%body%>");
			result.Id.ShouldBe("8aefe0ee-f12b-4575-b5b7-c97e21cb36f3");
			result.IsActive.ShouldBe(true);
			result.Name.ShouldBe("example_version_name");
			result.TemplateId.ShouldBe("ddb96bbc-9b92-425e-8979-99464621b543");
			result.TextContent.ShouldBe("<%body%>");
			result.UpdatedOn.ShouldBe(new DateTime(2014, 3, 19, 18, 56, 33, DateTimeKind.Utc));
		}

		[Fact]
		public void Create()
		{
			// Arrange
			var name = "My template";
			var scopes = new[] { "mail.send", "alerts.create", "alerts.read" };

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync(ENDPOINT, It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.CreateAsync(name, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void Get()
		{
			// Arrange
			var templateId = "e8ac01d5-a07a-4a71-b14c-4721136fe6aa";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{templateId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.GetAsync(templateId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(templateId);
			result.Name.ShouldBe("example template name");
		}

		[Fact]
		public void GetAll()
		{
			// Arrange
			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync(ENDPOINT, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(MULTIPLE_TEMPLATES_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.GetAllAsync(CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
			result.Length.ShouldBe(1);
		}

		[Fact]
		public void Delete()
		{
			// Arrange
			var templateId = "xxxxxxxx";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{templateId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			templates.DeleteAsync(templateId, CancellationToken.None).Wait(CancellationToken.None);

			// Assert
		}

		[Fact]
		public void Update()
		{
			// Arrange
			var templateId = "733ba07f-ead1-41fc-933a-3976baa23716";
			var name = "new_example_name";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{templateId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.UpdateAsync(templateId, name, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void CreateVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var name = "example_version_name";
			var subject = "<%subject%>";
			var htmlContent = "<%body%>";
			var textContent = "<%body%>";
			var isActive = true;

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{templateId}/versions", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_VERSION_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.CreateVersionAsync(templateId, name, subject, htmlContent, textContent, isActive, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void ActivateVersion()
		{
			// Arrange
			var templateId = "e3a61852-1acb-4b32-a1bc-b44b3814ab78";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PostAsync($"{ENDPOINT}/{templateId}/versions/{versionId}/activate", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_VERSION_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.ActivateVersionAsync(templateId, versionId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void GetVersion()
		{
			// Arrange
			var templateId = "d51480ca-ca3f-465c-bc3e-ceb71d73c38d";
			var versionId = "5997fcf6-2b9f-484d-acd5-7e9a99f0dc1f";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.GetAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_VERSION_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.GetVersionAsync(templateId, versionId, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void UpdateVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";
			var name = "updated_example_name";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.PatchAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(SINGLE_TEMPLATE_VERSION_JSON) })
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			var result = templates.UpdateVersionAsync(templateId, versionId, name, null, null, null, null, CancellationToken.None).Result;

			// Assert
			result.ShouldNotBeNull();
		}

		[Fact]
		public void DeleteVersion()
		{
			// Arrange
			var templateId = "ddb96bbc-9b92-425e-8979-99464621b543";
			var versionId = "8aefe0ee-f12b-4575-b5b7-c97e21cb36f3";

			var mockRepository = new MockRepository(MockBehavior.Strict);
			var mockClient = mockRepository.Create<IClient>();
			mockClient
				.Setup(c => c.DeleteAsync($"{ENDPOINT}/{templateId}/versions/{versionId}", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent))
				.Verifiable();

			var templates = new Templates(mockClient.Object, ENDPOINT);

			// Act
			templates.DeleteVersionAsync(templateId, versionId, CancellationToken.None).Wait();

			// Assert
		}
	}
}
