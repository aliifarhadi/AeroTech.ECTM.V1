namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficePrefillValueDto(string ParameterCode, string? Unit, double? SuggestedRawValue, int SampleCount);
}
