﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StrongGrid.Utilities
{
	/// <summary>
	/// Defines the contract for a strategy to retry an operation.
	/// </summary>
	public interface IRetryStrategy
	{
		/// <summary>
		/// Checks if we should retry an operation.
		/// </summary>
		/// <param name="attempt">The number of attempts carried out so far. That is, after the first attempt (for
		/// the first retry), attempt will be set to 1, after the second attempt it is set to 2, and so on.</param>
		/// <param name="response">The Http response of the previous request</param>
		/// <returns>
		///   <c>true</c> if another attempt should be made; otherwise, <c>false</c>.
		/// </returns>
		bool ShouldRetry(int attempt, HttpResponseMessage response);

		/// <summary>
		/// Gets a TimeSpan value which defines how long to wait before trying again after an unsuccessful attempt
		/// </summary>
		/// <param name="attempt">The number of attempts carried out so far. That is, after the first attempt (for
		/// the first retry), attempt will be set to 1, after the second attempt it is set to 2, and so on.</param>
		/// <param name="response">The Http response of the previous request</param>
		/// <returns>
		/// A TimeSpan value which defines how long to wait before the next attempt.
		/// </returns>
		TimeSpan GetNextDelay(int attempt, HttpResponseMessage response);
	}
}
