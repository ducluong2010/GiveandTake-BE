﻿namespace GiveandTake_API.Constants
{
    public class ApiEndPointConstant
    {
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public class Authentication
        {
            public const string LoginEndpoint = ApiEndpoint + "/login";
            public const string RegisterEndpoint = ApiEndpoint + "/register";
        }

        public class Account
        {
            public const string AccountsEndpoint = ApiEndpoint + "/accounts";
            public const string BannedAccountsEndpoint = AccountsEndpoint + "/banned";
            public const string EmailAccountsEndpoint = AccountsEndpoint + "/{email}/info";
            public const string AccountEndpoint = AccountsEndpoint + "/{id}";
            public const string PromoteToPremiumEndPoint = AccountsEndpoint + "/promote/{id}";
            public const string UpdatePremiumUntilEndPoint = AccountsEndpoint + "/updatepremiumuntil/{id}";
            public const string Update3MonthsPremiumUntilEndPoint = AccountsEndpoint + "/update3monthspremiumuntil/{id}";
            public const string Update6MonthsPremiumUntilEndPoint = AccountsEndpoint + "/update6monthspremiumuntil/{id}";
            public const string BanAccountEndPoint = AccountsEndpoint + "/ban/{id}";
            public const string UnbanAccountEndPoint = AccountsEndpoint + "/unban/{id}";
            public const string ChangePasswordEndPoint = AccountsEndpoint + "/{id}/password";
        }

        public class Category
        {
            public const string CategoriesEndPoint = ApiEndpoint + "/categories";
            public const string CategoryEndPoint = CategoriesEndPoint + "/{id}";
            public const string CategoryManaEndPoint = CategoriesEndPoint + "/manager";
        }

        public class Reward
        {
            public const string RewardsEndPoint = ApiEndpoint + "/rewards";
            public const string RewardEndPoint = RewardsEndPoint + "/{id}";
            public const string RewardStatusEndPoint = RewardsEndPoint + "/{id}/status";
        }

        public class Favorite
        {
            public const string FavoritesEndPoint = ApiEndpoint + "/favorites";
            public const string FavoriteEndPoint = FavoritesEndPoint + "/{id}";
            public const string FavoritesByAccountEndPoint = FavoritesEndPoint + "/account/{accountId}";
            public const string AddFavoriteEndPoint = FavoritesEndPoint + "/add";
            public const string DeleteFavoriteEndPoint = FavoritesEndPoint + "/delete/{id}";
            public const string FavoriteDonationsByCategoryEndPoint = FavoritesEndPoint + "/account/current";
            public const string AddMultipleFavoritesEndPoint = FavoritesEndPoint + "/multiple";
        }


        public class Rewarded
        {
            public const string RewardedEndPoint = ApiEndpoint + "/rewardeds";
            public const string RewardedByAccountEndPoint = RewardedEndPoint + "/account/{accountId}";
            public const string RewardedByIdEndPoint = RewardedEndPoint + "/{id}";
        }

        public class Donation
        {
            public const string DonationsEndPoint = ApiEndpoint + "/donations";
            public const string DonationsAppEndPoint = ApiEndpoint + "/donations/approved";
            public const string DonationEndPoint = DonationsEndPoint + "/{id}";
            public const string DonationStatusEndPoint = DonationsEndPoint + "/{id}/status";
            public const string ToggleDonationStatusEndPoint = DonationsEndPoint + "/{id}/toggle";
            public const string ToggleCancelEndPoint = DonationsEndPoint + "/{id}/togglecp";
            public const string ToggleApprovedEndPoint = DonationsEndPoint + "/{id}/toggleac";
            public const string CheckBannedAccountDonationsEndPoint = DonationsEndPoint + "/checkban";
            public const string CheckHidingDonationsEndPoint = DonationsEndPoint + "/checkactived";
            public const string SearchDonationsEndPoint = DonationsEndPoint + "/search";
            public const string DonationStaEndPoint = DonationsEndPoint + "/bystaff";
            public const string DonationCateEndPoint = DonationsEndPoint + "/bycate";
            public const string DonationAccEndPoint = DonationsEndPoint + "/byaccount";
            public const string DonationClaimEndPoint = DonationsEndPoint + "/byclaim";
            public const string DonationTypeEndPoint = DonationsEndPoint + "/bytype";
            public const string ToggleTypeEndPoint = DonationsEndPoint + "/{id}/toggletype";
            public const string ToggleType2EndPoint = DonationsEndPoint + "/{id}/typettoggle";
            public const string ToggleType3EndPoint = DonationsEndPoint + "/{id}/changetype";
            public const string ApprovedDonationByAccountAndTypeEndPoint = DonationsEndPoint + "/approved/type/{accountId}";
        }

        public class Transaction
        {
            public const string TransactionsEndPoint = ApiEndpoint + "/transactions";
            public const string TransactionEndPoint = TransactionsEndPoint + "/{id}";
            public const string TransactionStatusEndPoint = TransactionsEndPoint + "/{id}/status";
            public const string TransactionByAccountEndPoint = TransactionsEndPoint + "/account/{accountId}";
            public const string TransactionByAccountForCurrentUserEndPoint = TransactionsEndPoint + "/account/current";
            public const string CompletedTransactionsByDonationForSenderEndPoint = TransactionsEndPoint + "/donation/sender/completed";
            public const string FeedbackChangeEndPoint = TransactionsEndPoint + "/change/{transactionId}";



            public const string TransactionByDonationForAdminEndPoint = TransactionsEndPoint + "/donation/sender/{senderAccountId}";
            public const string TransactionByDonationForSenderEndPoint = TransactionsEndPoint + "/donation/sender";


            public const string CreateTransactionWithDetailEndPoint = TransactionsEndPoint + "/with-detail";
            public const string CompleteTransactionEndPoint = TransactionsEndPoint + "/{id}/complete";
            public const string CancelTransactionEndPoint = TransactionsEndPoint + "/{id}/cancel";
        }


        public class TransactionDetail
        {
            public const string TransactionDetailsEndPoint = ApiEndpoint + "/transaction-details";
            public const string TransactionDetailEndPoint = TransactionDetailsEndPoint + "/{id}";
            public const string TransactionDetailByTransactionEndPoint = TransactionDetailsEndPoint + "/transaction/{transactionId}";

            public const string QRCodeEndPoint = ApiEndpoint + "/qrcode";
            public const string GetQRCodeByTransactionId = QRCodeEndPoint + "/{transactionId}/{donationId}";
        }

        public class TradeTransactionDetail
        {
            public const string TradeTransactionDetailsEndPoint = ApiEndpoint + "/trade-transaction-details";
            public const string TradeTransactionDetailEndPoint = TradeTransactionDetailsEndPoint + "/{id}";
            public const string TradeTransactionDetailByTradeTransactionEndPoint = TradeTransactionDetailsEndPoint + "/trade-transaction/{tradeTransactionId}";

            public const string TradeQRCodeEndPoint = ApiEndpoint + "/trade-qrcode";
            public const string GetTradeQRCodeByTradeTransactionId = TradeQRCodeEndPoint + "/{tradeTransactionId}/{requestDonationId}";
        }

        public class DonationImage
        {
            public const string DonationImagesEndPoint = ApiEndpoint + "/product-images";
            public const string DonationImageEndPoint = DonationImagesEndPoint + "/{id}";
            public const string DonationAllImageEndPoint = DonationImagesEndPoint + "/donation/{donationId}";
            public const string ChangeThumbnailEndPoint = DonationImagesEndPoint + "/{id}/thumbnail";
        }

        public class Feedback
        {
            public const string FeedbacksEndPoint = ApiEndpoint + "/feedbacks";
            public const string FeedbackEndPoint = FeedbacksEndPoint + "/{id}";
            public const string FeedbackAccEndPoint = FeedbacksEndPoint + "/account/{id}";
            public const string FeedbackSenEndPoint = FeedbacksEndPoint + "/sender/{id}";
            public const string FeedbackWithoutPointsEndPoint = FeedbacksEndPoint + "/without-points";
        }

        public class Membership
        {
            public const string MembershipsEndPoint = ApiEndpoint + "/memberships";
            public const string MembershipEndPoint = MembershipsEndPoint + "/{id}";
        }

        public class Request
        {
            public const string RequestsEndPoint = ApiEndpoint + "/requests";
            public const string RequestEndPoint = RequestsEndPoint + "/{id}";
            public const string RequestByDonationEndPoint = RequestsEndPoint + "/donation/{donationId}";
            public const string RequestByAccountEndPoint = RequestsEndPoint + "/account/{accountId}";
            public const string CreateRequestEndPoint = RequestsEndPoint;
            public const string CancelRequestEndPoint = RequestsEndPoint + "/{id}/cancel";
            public const string DeleteRequestEndPoint = RequestsEndPoint + "/{id}";
        }

        public class Report
        {
            public const string ReportsEndPoint = ApiEndpoint + "/report";
            public const string ReportsUserPoint = ReportsEndPoint + "/user";
            public const string ReportsDonaPoint = ReportsEndPoint + "/donation";
            public const string ReportsTechPoint = ReportsEndPoint + "/tech";
            public const string ReportEndPoint = ReportsEndPoint + "/{id}";
            public const string ReportStaffEndPoint = ReportsEndPoint + "/staff";
            public const string ReportAccEndPoint = ReportsEndPoint + "/account";
            public const string ReportAByEndPoint = ReportsEndPoint + "/approvedBy";
            public const string ReportCreateEndPoint = ReportsEndPoint + "/create";
            public const string ReportUpdateEndPoint = ReportsEndPoint + "/update/{id}";
            public const string ReportDeleteEndPoint = ReportsEndPoint + "/delete/{id}";
            public const string ReportChangeEndPoint = ReportsEndPoint + "/changepending/{id}";
            public const string ReportCompleteEndPoint = ReportsEndPoint + "/changeprocess/{id}";
        }

        public class ReportType
        {
            public const string ReportTypesEndPoint = ApiEndpoint + "/reporttype";
            public const string ReportTypeEndPoint = ReportTypesEndPoint + "/{id}";
            public const string ReportTypeUpdateEndPoint = ReportTypesEndPoint + "/update";
            public const string ReportTypeDeleEndPoint = ReportTypesEndPoint + "/delete";
            public const string ChangeStatusEndPoint = ReportTypesEndPoint + "/changestatus";
        }

        public class Notification
        {
            public const string NotisEndPoint = ApiEndpoint + "/notification";
            public const string NotiEndPoint = NotisEndPoint + "/{id}";
            public const string NotiCreateEndPoint = NotisEndPoint + "/create";
            public const string NotiUpdateEndPoint = NotisEndPoint + "/update/{id}";
            public const string NotiDeleteEndPoint = NotisEndPoint + "/delete/{id}";
            public const string NotiChangeEndPoint = NotisEndPoint + "/change/{id}";
            public const string NotiAccEndPoint = NotisEndPoint + "/getbyacc/{id}";
            public const string NotiStaffEndPoint = NotisEndPoint + "/getbystaff/{id}";
            public const string NotiAppEndPoint = NotisEndPoint + "/getapproved/{id}";
            public const string NotiBonusEndPoint = NotisEndPoint + "/getbonus/{id}";
            public const string NotiPointEndPoint = NotisEndPoint + "/getpoint/{id}";
            public const string NotiRejectEndPoint = NotisEndPoint + "/getreject/{id}";
            public const string NotiAcceptEndPoint = NotisEndPoint + "/getaccept/{id}";
            public const string NotiCancelEndPoint = NotisEndPoint + "/getcancel/{id}";
        }

        public class TradeRequest
        {
            public const string TradeRequestsEndPoint = ApiEndpoint + "/trade-requests";
            public const string TradeRequestEndPoint = TradeRequestsEndPoint + "/{id}";
            public const string TradeRequestByAccountEndPoint = TradeRequestsEndPoint + "/account/{accountId}";
            public const string TradeRequestByDonationEndPoint = TradeRequestsEndPoint + "/donation/{requestDonationId}";
            public const string CreateTradeRequestEndPoint = TradeRequestsEndPoint + "/create";
            public const string CancelTradeRequestEndPoint = TradeRequestsEndPoint + "/{id}/cancel";
            public const string DeleteTradeRequestEndPoint = TradeRequestsEndPoint + "/{id}/delete";
        }

        public class TradeTransaction
        {
            public const string TradeTransactionsEndPoint = ApiEndpoint + "/trade-transactions";
            public const string TradeTransactionEndPoint = TradeTransactionsEndPoint + "/{id}";
            public const string AcceptTradeRequestEndPoint = TradeTransactionsEndPoint + "/accept/{tradeRequestId}/";
            public const string GetTradeTransactionStatusEndPoint = TradeTransactionsEndPoint + "/{id}/status";
            public const string GetTradeTransactionByAccountIdEndPoint = TradeTransactionsEndPoint + "/account/{accountId}";
            public const string RejectTradeRequestEndPoint = TradeTransactionsEndPoint + "/reject/{tradeRequestId}/";
            public const string CompleteTradeTransactionEndPoint = TradeTransactionsEndPoint + "/complete/{tradeTransactionId}/";
            public const string CancelTradeTransactionEndPoint = TradeTransactionsEndPoint + "/cancel/{tradeTransactionId}/";
            public const string GetTradeTransactionByDonationForSenderEndPoint = TradeTransactionsEndPoint + "/donation/sender/";
        }
    }
}
