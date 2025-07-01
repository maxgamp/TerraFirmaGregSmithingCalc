using System;

namespace TerraFirmaGregCalculator;

public static class SmithingMoveExtensions
{
    public static int GetMovePointValue<T>(this T? moveType) where T : Enum
    {
        var attributes = moveType?.GetType().GetMember(moveType.ToString())[0].GetCustomAttributes(typeof(SmithingMovePoints), false);

        if (attributes?.Length > 0)
        {
            var metaData = attributes[0] as SmithingMovePoints;

            ArgumentNullException.ThrowIfNull(metaData);

            return metaData.PointValue;
        }

        return 0;
    }
}
