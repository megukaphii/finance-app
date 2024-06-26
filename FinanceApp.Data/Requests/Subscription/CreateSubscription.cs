﻿using FinanceApp.Data.Enums;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.RequestPatterns.Subscription;

namespace FinanceApp.Data.Requests.Subscription;

public class CreateSubscription : ISubscriptionFields
{
	public static string Flag => "<CreateSubscription>";

	public required RequestField<long> Counterparty { get; init; }
	public required RequestField<string> Name { get; init; }
	public required RequestField<decimal> Value { get; init; }
	public required RequestField<int> FrequencyCounter { get; init; }
	public required RequestField<Frequency> FrequencyMeasure { get; init; }
	public required RequestField<DateTime> StartDate { get; init; }
	public required RequestField<DateTime> EndDate { get; init; }

	public override string ToString() =>
		$"{Flag}[[{nameof(Counterparty)}: {Counterparty}], {nameof(Name)}: {Name}, {nameof(Value)}: {Value}, {nameof(FrequencyCounter)}: {FrequencyCounter}, {nameof(FrequencyMeasure)}: {FrequencyMeasure}, {nameof(StartDate)}: {StartDate}, {nameof(EndDate)}: {EndDate}]";
}

public class CreateSubscriptionResponse : IResponse
{
	public required long Id { get; init; }
	public required bool Success { get; init; }

	public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Id)}: {Id}";
}