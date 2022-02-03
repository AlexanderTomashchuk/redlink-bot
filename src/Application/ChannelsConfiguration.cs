using System;

namespace Application;

public class ChannelsConfiguration
{
    public string UaChannelId { get; init; }

    public string RuChannelId { get; init; }

    public string UsChannelId { get; init; }

    public string PlChannelId { get; init; }

    public string GetChannelIdByCountryCode(string countryCode) =>
        countryCode switch
        {
            "UA" => UaChannelId,
            "RU" => string.IsNullOrEmpty(RuChannelId) ? UaChannelId : RuChannelId,
            "US" => string.IsNullOrEmpty(UsChannelId) ? UaChannelId : UsChannelId,
            "PL" => string.IsNullOrEmpty(PlChannelId) ? UaChannelId : PlChannelId,
            _ => throw new ArgumentOutOfRangeException(nameof(countryCode), countryCode,
                "There is no channel defined for provided country code")
        };
}