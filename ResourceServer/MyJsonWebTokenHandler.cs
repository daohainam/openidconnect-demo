using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace OIDCDemo.ResourceServer
{
    public class MyJsonWebTokenHandler: JsonWebTokenHandler
    {
        public async override Task<TokenValidationResult> ValidateTokenAsync(SecurityToken token, TokenValidationParameters validationParameters)
        {
            var r = await base.ValidateTokenAsync(token, validationParameters);

            return r;
        }

        public override Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
        {
            return base.ValidateTokenAsync(token, validationParameters);    
        }
    }
}
