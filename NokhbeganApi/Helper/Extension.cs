using NokhbeganApi.Helper;
using NokhbeganApi.Repository;
using NokhbeganApi.Service;


namespace NokhbeganApi.Customized
{
    public static class Extension
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {

            services.AddScoped<IUser, UserService>();
            services.AddScoped<IAdmin, AdminService>();
            services.AddScoped<IStudent, StudentService>();
            //services.AddScoped<IPay, PayService>();
            services.AddScoped<DataSeeder>();
            return services;
        }
    }
    public static class ZibalPaymentStatusHelper
    {
        public static string GetStatusMessage(string statusCode)
        {
            return statusCode switch
            {
                "-1" => "در انتظار پرداخت",
                "-2" => "خطای داخلی",
                "1" => "پرداخت شده - تاییدشده",
                "2" => "پرداخت شده - تاییدنشده",
                "3" => "لغو شده توسط کاربر",
                "4" => "شماره کارت نامعتبر",
                "5" => "موجودی حساب کافی نیست",
                "6" => "رمز وارد شده اشتباه است",
                "7" => "تعداد درخواست بیش از حد مجاز",
                "8" => "سقف پرداخت روزانه بیش از حد مجاز",
                "9" => "مبلغ پرداخت روزانه بیش از حد مجاز",
                "10" => "صادرکننده‌ی کارت نامعتبر است",
                "11" => "خطای سوئیچ",
                "12" => "کارت قابل دسترسی نیست",
                "15" => "تراکنش استرداد شده",
                "16" => "تراکنش در حال استرداد است",
                "18" => "تراکنش ریورس شده",
                _ => "وضعیت ناشناخته پرداخت"
            };

        }
        public static string GetVerifyResultMessage(string resultCode)
        {
            return resultCode switch
            {
                "100" => "پرداخت با موفقیت تأیید شد.",
                "201" => "پرداخت قبلاً تأیید شده است.",
                "102" => "مرچنت یافت نشد.",
                "103" => "مرچنت غیرفعال است.",
                "104" => "مرچنت نامعتبر است.",
                "202" => "پرداخت انجام نشده یا ناموفق بوده است.",
                "203" => "شناسه پیگیری نامعتبر است.",
                _ => "خطای ناشناخته در تأیید پرداخت."
            };
        }
        public static bool IsVerifySuccess(string resultCode)
        {
            return resultCode switch
            {
                "100" => true,
                "201" => true,
                _ => false
            };
        }
    }
}

