namespace Com.Kumulos.Abstractions
{
    public class Consts
    {
        public const string SDK_VERSION = "5.0.0";

        public const int SDK_TYPE = 7;

        public const int RUNTIME_TYPE = 2;

        public const int TARGET_TYPE_DEBUG = 1;
        public const int TARGET_TYPE_RELEASE = 2;

        public const string BUILD_SERVICE_BASE_URI = "https://api.kumulos.com";

        public const string CRASH_REPORT_FORMAT = "xamarin-csharp";
        public const string CRASH_REPORT_EVENT_TYPE = "k.crash.loggedException";

        public const string PUSH_SERVICE_BASE_URI = "https://push.kumulos.com/v1";
    }
}
