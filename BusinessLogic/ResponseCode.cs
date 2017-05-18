using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public enum ResponseCode
    {
        // Used for standard serialization
        NotSet = -1,

        // Success code constants
        Success,
        SuccessProfileUpdated,
        SuccessSignUp,
        SuccessSignIn,
        SuccessSignOut,
        SuccessYourPasswordWasReset,
        SuccessShiftCreated,
        SuccessShiftCancelled,
        SuccessShiftReadvertised,
        SuccessNotificationAlreadyDeleted,
        SuccessShiftOfferAccepted,
        SuccessShiftOfferCancelled,
        SuccessShiftOfferDeclined,
        SuccessShiftOfferCheckedIn,
        SuccessShiftOfferCheckedOut,
        SuccessShiftOfferConfirmed,
        SuccessFeedbackSent,

        // Error code constants
        ErrorNoInternet,
        ErrorUsernameTaken,
        ErrorAnErrorOccurred,
        ErrorSignInFail,
        ErrorInvalidUsernameOrPassword,
        ErrorSignUpFail,
        ErrorFailedToSendEmail,
        ErrorFailedToUploadYourVideo,
        ErrorFailedToUploadYourProfilePicture,
        ErrorFailedToChangeUserApprovalStatus,
        ErrorInvalidInput,
        ErrorNoAvailableSlots,
        ErrorNoOccupiedSlots,
        ErrorFailedToCreatePayslip,
        ErrorFailedToUpdateThumbsUp,
        ErrorFailedToUpdateEmployeeWorkSchedule,
        ErrorDatesAreNotValid,
        ErrorCreateShiftFailed,
        ErrorGeneratingShiftOffersFailed,
        ErrorAssigningLocationsFailed,
        ErrorCreateShiftTemplateFailed,
        ErrorCannotCancelShiftWithinTimeLimit,
        ErrorCannotCheckinBeforeTimeLimit,
        ErrorShiftWasCancelled,
        ErrorShiftHasAlreadyStarted,
        ErrorShiftOfferAlreadyAccepted,
        ErrorStripePaymentFailed,
        ErrorYouAreNotAvailable,
        ErrorCannotDeclineAlreadyRespondedShift
    }

    public static class CodeConstantsExtensions
    {
        public static bool IsEqualToResponseCode(this int value, ResponseCode target)
        {
            return target.IsEqualToInt(value);
        }

        #region private methods

        private static bool IsEqualToInt(this ResponseCode value, int target)
        {
            return (int)value == target;
        }

        #endregion
    }
}
