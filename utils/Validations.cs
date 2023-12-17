using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodQualityControl.Constants;

namespace BloodQualityControl.utils
{
    public class Validations
    {
        public static string ValidateBloodQuality(float minBloodQuality, float maxBloodQuality)
        {
            if (!(QualityConstants.MIN_BLOOD_QUALITY <= minBloodQuality && minBloodQuality <= QualityConstants.MAX_BLOOD_QUALITY))
            {
                return $"The given Minimum Blood Quality {minBloodQuality} is not in the range of 5-100";
            }

            if (!(QualityConstants.MIN_BLOOD_QUALITY <= maxBloodQuality && maxBloodQuality <= QualityConstants.MAX_BLOOD_QUALITY))
            {
                return $"The given Maximum Blood Quality {maxBloodQuality} is not in the range of 5-100";
            }

            if (minBloodQuality > maxBloodQuality)
            {
                return $"The given Minimum Blood Quality {minBloodQuality} is higher than the given Max Blood Quality {maxBloodQuality}";
            }
            return string.Empty;
        }
    }
}