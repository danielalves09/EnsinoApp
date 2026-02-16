using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EnsinoApp.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ITempDataDictionary _tempData;

        public NotificationService(ITempDataDictionaryFactory tempDataFactory,
                                   Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _tempData = tempDataFactory.GetTempData(httpContextAccessor.HttpContext!);
        }

        public void Success(string message)
        {
            _tempData["ToastrSuccess"] = message;
        }

        public void Error(string message)
        {
            _tempData["ToastrError"] = message;
        }

        public void Warning(string message)
        {
            _tempData["ToastrWarning"] = message;
        }

        public void Info(string message)
        {
            _tempData["ToastrInfo"] = message;
        }
    }
}
