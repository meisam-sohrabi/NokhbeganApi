namespace NokhbeganApi.Helper
{
    public class ReferralCode
    {
        public static string ReferralCodeGenerator()
        {
            var Code = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            return Code;
        }
    }
}
