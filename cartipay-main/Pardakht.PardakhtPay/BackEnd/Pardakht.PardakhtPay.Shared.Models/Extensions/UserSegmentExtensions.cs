using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Validators;

namespace Pardakht.PardakhtPay.Shared.Models.Extensions
{
    public static class UserSegmentExtensions
    {
        static readonly List<int> DecimalList = new List<int>()
        {
            //(int)UserSegmentType.ExpiredTransactionCount,
            (int)UserSegmentType.TotalDepositAmountPardakhtPay,
            (int)UserSegmentType.TotalDepositCountPardakhtPay,
            (int)UserSegmentType.TotalDepositAmountMerchant,
            (int)UserSegmentType.TotalDepositCountMerchant,
            (int)UserSegmentType.TotalWithdrawalAmountPardakhtPay,
            (int)UserSegmentType.TotalWithdrawalAmountMerchant,
            (int)UserSegmentType.TotalWithdrawalCountPardakhtPay,
            (int)UserSegmentType.TotalWithdrawalCountMerchant,
            //(int)UserSegmentType.WaitingPaymentTransactionCount,
            (int)UserSegmentType.ActivityScore,
            (int)UserSegmentType.TotalSportbookAmount,
            (int)UserSegmentType.TotalSportbookCount,
            (int)UserSegmentType.TotalCasinoAmount,
            (int)UserSegmentType.TotalCasinoCount
        };

        static readonly List<int> StringList = new List<int>()
        {
            (int)UserSegmentType.GroupName,
            (int)UserSegmentType.WebsiteName
        };

        static readonly List<int> DateList = new List<int>()
        {
            (int)UserSegmentType.RegistrationDate,
            (int)UserSegmentType.LastActivity
        };

        static readonly string[] DateFormats = new string[]
        {
            "dd.MM.yyyy",
            "yyyy-mm-dd",
            "dd.MM.yyyy HH:mm"
        };

        public static IValidator GetValidator(this UserSegment segment, Dictionary<int, object> values)
        {
            if (!values.ContainsKey(segment.UserSegmentTypeId))
            {
                throw new Exception($"Could not find actual value for {segment.UserSegmentType.ToString()}");
            }

            var actualValue = values[segment.UserSegmentTypeId];

            switch (segment.UserSegmentCompareType)
            {
                case UserSegmentCompareType.LessThan:
                    return GetLessThanValidator(segment, actualValue);
                case UserSegmentCompareType.LessThanAndEquals:
                    return GetLessThanAndEqualValidator(segment, actualValue);
                case UserSegmentCompareType.Equals:
                    return GetEqualValidator(segment, actualValue);
                case UserSegmentCompareType.NotEqual:
                    return GetNotEqualValidator(segment, actualValue);
                case UserSegmentCompareType.MoreThanAndEquals:
                    return GetMoreThanAndEqualValidator(segment, actualValue);
                case UserSegmentCompareType.MoreThan:
                    return GetMoreThanValidator(segment, actualValue);
                default:
                    throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
            }
        }

        private static IValidator GetEqualValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalEqualsValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (StringList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new StringEqualsValidator();
                validator.ExpectedValue = segment.Value;
                validator.ActualValue = actualValue == null ? string.Empty : actualValue.ToString();

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeEqualsValidator();

                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static IValidator GetLessThanValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalLessThanValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeLessThanValidator();

                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static IValidator GetLessThanAndEqualValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalLessThanOrEqualValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeLessThanAndEqualValidator();
                
                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static IValidator GetNotEqualValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalNotEqualValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeNotEqualValidator();

                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static IValidator GetMoreThanAndEqualValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalMoreThanAndEqualValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeMoreThanAndEqualValidator();

                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static IValidator GetMoreThanValidator(UserSegment segment, object actualValue)
        {
            if (DecimalList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DecimalMoreThanValidator();
                validator.ExpectedValue = Convert.ToDecimal(segment.Value);
                validator.ActualValue = Convert.ToDecimal(actualValue);

                return validator;
            }
            else if (DateList.Contains(segment.UserSegmentTypeId))
            {
                var validator = new DateTimeMoreThanValidator();

                SetDateTimeValidator(validator, segment, actualValue);

                return validator;
            }

            throw new Exception($"Could not find a validator for {segment.UserSegmentCompareType.ToString()}");
        }

        private static void SetDateTimeValidator(BaseDateTimeValidator validator, UserSegment segment, object actualValue)
        {
            validator.ExpectedValue = GetCurrentDateTime();
            if (actualValue == null)
            {
                validator.ActualValue = null;
            }
            else
            {
                validator.ActualValue = Convert.ToDateTime(actualValue);
            }

            if (int.TryParse(segment.Value, out int result))
            {
                validator.ExpectedDayCount = result;
            }
        }

        private static DateTime? GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
