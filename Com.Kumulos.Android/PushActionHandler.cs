using System;
using Android.Content;
using Com.Kumulos.Android;

namespace Com.Kumulos
{
    public class PushActionHandler : Java.Lang.Object, IPushActionHandlerInterface
    {
        void IPushActionHandlerInterface.Handle(Context context, PushMessage pushMessage, string actionId)
        {
            throw new NotImplementedException("You must create your own implementation subclass to handle button actions");
        }
    }
}
